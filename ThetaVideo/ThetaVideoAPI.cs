using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWWWHelpers;
using Newtonsoft.Json;
using System.IO;

/*
 * 
 * Simple class to upload to ThetaVideo API
 * 
 * Call PostVideo(string filename) with the locally stored video you want to upload
 * 
 * subscribe a delegate for progress returned to VideoProgressInt - returns 0 when uploaded;
 * Call CheckProgress with videoid to check progress
 * 
 */
namespace ThetaVideoAPIUnity
{
    [RequireComponent(typeof(WWWHelpers))]
    public class ThetaVideoAPI : MonoBehaviour
    {
        // exposed in editor - fill it out!
        public string THETA_ID;
        public string THETA_SECRET;

        WWWHelpers wh;

        List<string> filePaths = new List<string>();
        // primary key is url and then uploadid
        Dictionary<string, VideoConnectorData> dicURLs = new Dictionary<string, VideoConnectorData>();

        public VOIDINT VideoProgressInt;

        private void Awake()
        {
            if (string.IsNullOrEmpty(THETA_ID) || string.IsNullOrEmpty(THETA_SECRET)) Debug.LogError("Showstopping ERROR!!! THETA_ID and THETA_SECRET must be filled out.");

            wh = GetComponent<WWWHelpers>();
        }
          
        public void PostVideo(string filepath)
        {
            filePaths.Add(filepath);

            POST_SignedURL();
             
        }
        public void CheckProgress(string videoid)
        {
            GET_ProgressInt(videoid);

            if (VideoProgressInt == null) print(">>> VideoProgressInt is not set for video progress return");
        }
        public void CheckProgressRaw(string videoid,VOIDSTRING ProgressReturned)
        {
            GET_Progress(videoid, ProgressReturned);
        } 

        // Step 1 POST fetch signed URL
        protected void POST_SignedURL(VOIDSTRING ErrorReturned = null)
        {
            string url = "https://api.thetavideoapi.com/upload";

            wh.PostRaw(url, null, GotSignedURL);
        }

        void GotSignedURL(string json)
        {
            Video_ResponseUpload v = JsonConvert.DeserializeObject<Video_ResponseUpload>(json);
            string url = v.body.uploads[0].presigned_url;
            string uploadid = v.body.uploads[0].id;


            // upload last filePaths
            if (filePaths.Count > 0)
            { 
                // store for lookup after video response, passed to transcode
                string filepath = filePaths[0];
                dicURLs[uploadid] = new VideoConnectorData(dicURLs[filepath],uploadid); dicURLs.Remove(filepath);
                dicURLs.Add(url, new VideoConnectorData(filepath, url, uploadid, null));
                PUT_Video(url,File.ReadAllBytes(filePaths[0]), GotVideoBlank);
            }
        }

        // Step 2 PUT video to signed URL 
        protected void PUT_Video(string url,byte[] video, VOIDSTRING ProgressReturned, VOIDSTRING ErrorReturned = null)
        {
            wh.PutBinary(url, video, GotVideoBlank);
        }

        void GotVideoBlank(string url)
        { 
            if (string.IsNullOrEmpty(url))
            {
                VideoProgressInt?.Invoke(0);

                // transcode it
                POST_Transcode(dicURLs[url].uploadid);

                // remove video from list - let's try uploading more videos if any remain
                string filepath = dicURLs[url].filepath;
                filePaths.Remove(filepath);  
                File.Delete(filepath);
                if (filePaths.Count > 0) PostVideo(filePaths[0]);
            }
        }

        // Step 3 POST transcode upload
        protected void POST_Transcode(string uploadid)
        {
            string url = "https://api.thetavideoapi.com/video";

            wh.PostRaw(url, JsonConvert.SerializeObject(new Transcode_RequestData(uploadid)),TranscodeReturned);
        }

        void TranscodeReturned(string json)
        {
            Video_ResponseVideo v = JsonConvert.DeserializeObject<Video_ResponseVideo>(json);
            string uploadid = v.body.videos[0].source_upload_id;
            string videoid = v.body.videos[0].id;
            // update our dictionary
            dicURLs[uploadid].videoid = videoid;
        }

        // Step 4 - called asynchronously GET progress from video_id
        protected void GET_Progress(string videoid,VOIDSTRING ProgressReturned ) {
            string url = "https://api.thetavideoapi.com/video/"+videoid;
              
            wh.Get(url, ProgressReturned); 
        } 
        protected void GET_ProgressInt(string videoid)
        {
            string url = "https://api.thetavideoapi.com/video/" + videoid;

            wh.Get(url, ManagedReturned);
        }

        void ManagedReturned(string json)
        {
            Video_Response_Video v = JsonConvert.DeserializeObject<Video_Response_Video>(json);
            VideoProgressInt?.Invoke(v.progress);
        }

    }


}
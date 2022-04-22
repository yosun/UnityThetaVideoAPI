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
 * 1) Call PostVideo(string filename) with the locally stored video you want to upload
   2) (Optional) Subscribe a delegate for progress returned to VideoProgressInt
         - returns -1 when uploaded
         - returns -2 when transcode begins
         - all other progress values are from API
         - returns 100 when uploaded;
   3) Call CheckProgress (with optional videoid if not the last uploaded/transcode requested video) 
 *    Upon completion: populates playback_uri and player_url in dicURLs[uploadid]
 * 
 * 4) (Optionally) pass delegate for upload status to AssignCallbackUploadProgress
 */
namespace ThetaVideoAPIUnity
{
    [RequireComponent(typeof(WWWHelpers))]
    public class ThetaVideoAPI : MonoBehaviour
    {
        // exposed in editor - fill it out!
        public string THETA_ID;
        public string THETA_SECRET;
        Dictionary<string, string> dicAuthHeaders = new Dictionary<string, string>();

        WWWHelpers wh;

        List<string> filePaths = new List<string>();
        // primary key url and then uploadid then videoid  
        public Dictionary<string, VideoConnectorData> dicURLs = new Dictionary<string, VideoConnectorData>();


        public VOIDINT VideoProgressInt;

        public string lastVideoID;
        public VideoConnectorData lastVCD;

        public void AssignCallbackUploadProgress(VOIDINT up)
        {
            wh.UploadProgress += up;
        }

        private void Awake()
        {
            if (string.IsNullOrEmpty(THETA_ID) || string.IsNullOrEmpty(THETA_SECRET)) Debug.LogError("Showstopping ERROR!!! THETA_ID and THETA_SECRET must be filled out.");

            wh = GetComponent<WWWHelpers>();

            // create headers from key
            dicAuthHeaders.Add("x-tva-sa-id", THETA_ID);
            dicAuthHeaders.Add("x-tva-sa-secret", THETA_SECRET);
            dicAuthHeaders.Add("Content-Type", "application/json");
        }
          
        public void PostVideo(string filepath)
        {
            print("PostVideo");
            filePaths.Add(filepath);
            
            POST_SignedURL();
             
        }
        public void CheckProgress()
        {
            if(!string.IsNullOrEmpty(lastVideoID))
                CheckProgress(lastVideoID);
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
            print("POST_SignedURL");
            string url = "https://api.thetavideoapi.com/upload";

            wh.PostRaw(url, null, GotSignedURL,null, dicAuthHeaders);
        }

        void GotSignedURL(string json)
        {
            print("GotSignedURL");
            Video_ResponseUpload v = JsonConvert.DeserializeObject<Video_ResponseUpload>(json);
            string url = v.body.uploads[0].presigned_url;
            string uploadid = v.body.uploads[0].id;
             
            // upload last filePaths
            if (filePaths.Count > 0)
            {
                // store for lookup after video response, passed to transcode
                string filepath = filePaths[0];

                dicURLs.Add(url, new VideoConnectorData(filepath, url, uploadid, null));
                print("Processing " + filePaths[0]);
                PUT_Video(url, File.ReadAllBytes(filePaths[0]), GotVideoBlank);
            }
            else Debug.LogError("No filePaths to upload");
        }

        // Step 2 PUT video to signed URL 
        protected void PUT_Video(string url,byte[] video, VOIDSTRING ProgressReturned, VOIDSTRING ErrorReturned = null)
        {
            print("PUT_Video");
            wh.PutBinary(url, video,null,null, GotVideoBlank);
        }

        void GotVideoBlank(string url)
        { 
             
            print("GotVideoBlank");
            VideoProgressInt?.Invoke(-1);

            string uploadid = dicURLs[url].uploadid;

            // transcode it
            POST_Transcode(dicURLs[url].uploadid);

            // update dic to uploadid, remove old entry
            dicURLs.Add(uploadid, new VideoConnectorData(dicURLs[url], uploadid));
            dicURLs.Remove(url);

            // remove video from list - let's try uploading more videos if any remain
            string filepath = dicURLs[uploadid].filepath;
            filePaths.Remove(filepath);  
            File.Delete(filepath);
             
            if (filePaths.Count > 0) PostVideo(filePaths[0]);
             
        }

        // Step 3 POST transcode upload
        protected void POST_Transcode(string uploadid)
        {
            print("POST_Transcode");
            string url = "https://api.thetavideoapi.com/video"; 
            wh.PostRaw(url, JsonConvert.SerializeObject(new Transcode_RequestData(uploadid)),TranscodeReturned,null,dicAuthHeaders);
        }

        void TranscodeReturned(string json)
        {
            print("TranscodeReturned "+json);
            Video_ResponseVideo v = JsonConvert.DeserializeObject<Video_ResponseVideo>(json,new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string uploadid = v.body.videos[0].source_upload_id;
            string videoid = v.body.videos[0].id; 

            // update our dictionary
            lastVideoID = videoid; 
            dicURLs[uploadid].videoid = videoid;
            dicURLs.Add(videoid, new VideoConnectorData(dicURLs[uploadid]));
            dicURLs.Remove(uploadid);

            VideoProgressInt?.Invoke(-2);
        }

        // Step 4 - called asynchronously GET progress from video_id
        protected void GET_Progress(string videoid,VOIDSTRING ProgressReturned ) {
            print("GET_Progress "+videoid);
            string url = "https://api.thetavideoapi.com/video/"+videoid;
              
            wh.Get(url, ProgressReturned,null,dicAuthHeaders); 
        } 
        protected void GET_ProgressInt(string videoid)
        { 
            GET_Progress(videoid, ManagedReturned);
        }

        void ManagedReturned(string json)
        {
            print(json);
            Video_ResponseVideo v0 = JsonConvert.DeserializeObject<Video_ResponseVideo>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            Video_Response_Video v = v0.body.videos[0];
             
            if (v.progress >= 100 ||  !string.IsNullOrEmpty(v.playback_uri))
            {
                if (!string.IsNullOrEmpty(v.playback_uri)) dicURLs[v.id].playback_uri = v.playback_uri;
                if (!string.IsNullOrEmpty(v.player_uri)) dicURLs[v.id].player_uri = v.player_uri;

                lastVCD = dicURLs[v.id];
                v.progress = 100;
            }

            VideoProgressInt?.Invoke(v.progress);

        }

    }


}
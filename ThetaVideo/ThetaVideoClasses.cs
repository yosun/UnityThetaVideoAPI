using System; 
using System.Collections.Generic; 
 
namespace ThetaVideoAPIUnity
{
    // general video data connector
    [System.Serializable]
    public class VideoConnectorData
    {
        public string filepath;
        public string signedurl;
        public string uploadid;
        public string videoid;

        public VideoConnectorData(string f,string s,string u,string v)
        {
            filepath = f; signedurl = s; uploadid = u; videoid = v;
        }

        public VideoConnectorData(VideoConnectorData V,string u)
        {
            filepath = V.filepath;
            signedurl = V.signedurl;
            uploadid = u;
        }
    }

    public class Transcode_RequestData
    {
        public string source_upload_id { get; set; }
        public string playback_policy { get; set; }

        public Transcode_RequestData(string id,string policy)
        {
            source_upload_id = id;
            playback_policy = policy;
        }

        public Transcode_RequestData(string id)
        {
            source_upload_id = id;
            playback_policy = "public";
        }
    }


    // step 1 response
    public class Video_ResponseUpload
    {
        public string status { get; set; }
        public Video_BodyUpload body { get; set; }
    }

    // step 3 and 4 response
    public class Video_ResponseVideo
    {
        public string status { get; set; }
        public Video_BodyVideo body { get; set; }
    }


    // body list definitions:  Video and Upload

    public class Video_BodyUpload
    {
        public List<Video_Response_Upload> uploads { get; set; }
    }

    public class Video_BodyVideo
    {
        public List<Video_Response_Video> videos { get; set; }
    }
 
    [System.Serializable]
    public class Video_Response_Upload
    {
        public string id { get; set; }
        public string service_account_id { get; set; }
        public string presigned_url { get; set; }
        public DateTime presigned_url_expiration { get; set; }
        public bool presigned_url_expired { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
    }

    [System.Serializable]
    public class Video_Response_Video
    {
        public string id { get; set; }
        public string playback_uri { get; set; }
        public string player_uri { get; set; }
        public object nft_collection { get; set; }
        public object create_time { get; set; }
        public object update_time { get; set; }
        public string service_account_id { get; set; }
        public object file_name { get; set; }
        public string state { get; set; }
        public string sub_state { get; set; }
        public string source_upload_id { get; set; }
        public object source_uri { get; set; }
        public string playback_policy { get; set; }
        public int progress { get; set; }
        public object error { get; set; }
        public string duration { get; set; }
        public int resolution { get; set; }
        public string metadata { get; set; }
    }

}
# UnityThetaVideoAPI
Theta Token Video API wrappers for Unity https://docs.thetatoken.org/docs/theta-video-api-overview


No dependencies self-contained classes to upload and check progress from Theta Token Video API
 
## The world's most simple class to upload to ThetaVideo API

Test it out using the demo scene TheThetaVideosBarebonesTester. The entire Demo folder can be removed. 
  

See https://devpost.com/software/unity-theta-video-api-sdk for more 

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

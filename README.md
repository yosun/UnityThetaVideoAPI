# UnityThetaVideoAPI
Theta Token Video API wrappers for Unity https://docs.thetatoken.org/docs/theta-video-api-overview


No dependencies self-contained classes to upload and check progress from Theta Token Video API
 
## The world's most simple class to upload to ThetaVideo API
  

See https://devpost.com/software/unity-theta-video-api-sdk for more 

/*
 * 
 * Simple class to upload to ThetaVideo API
 * 
 * 1) Call PostVideo(string filename) with the locally stored video you want to upload
   2) (Optional) Subscribe a delegate for progress returned to VideoProgressInt - returns 0 when uploaded;
   3) Call CheckProgress (with optional videoid if not the last uploaded/transcode requested video) 
 *    Upon completion: populates playback_uri and player_url in dicURLs[uploadid]
 * 
 */

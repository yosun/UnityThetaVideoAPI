# UnityThetaVideoAPI
Theta Token Video API wrappers for Unity https://docs.thetatoken.org/docs/theta-video-api-overview


No dependencies self-contained classes to upload and check progress from Theta Token Video API

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ThetaVideoAPIUnity;

/*
 * This is a barebones demo and Unity scene for testing things out
 */
public class TheThetaVideosBarebonesTester : MonoBehaviour
{
    string filePathToUpload;
    public InputField inputFilepath;

    public ThetaVideoAPI thetaVideoAPI;

    bool videoIsDone;
    public Text txtProgress;

    public GameObject panelIsDone;

    private void Start()
    {
        Camera.main.backgroundColor = new Color(.15f  ,  .64f  ,  .9f); // set to theta color
        thetaVideoAPI.VideoProgressInt += VideoProgressReturned;
        thetaVideoAPI.AssignCallbackUploadProgress(VideoUploadProgress);

        print("Please make sure your video filepath is in "+Application.persistentDataPath);
    }

    void VideoUploadProgress(int n)
    {
        txtProgress.text = "Uploading: "+ n.ToString() + "%";
    }

    void VideoProgressReturned(int n)
    {
        print("VideoProgressReturned " + n);
        if (n == 100) { videoIsDone = true; panelIsDone.SetActive(true); }
        else if(n>=0) txtProgress.text = "Transcoding: "+ n.ToString() + "%";
    }

    public void UpdateFilepath()
    {
        UpdateFilepath(inputFilepath.text);
    }

    public void UpdateFilepath(string filepath)
    {
        filePathToUpload = Path.Combine( Application.persistentDataPath,filepath );
    }

    public void UploadVideo()
    {
        videoIsDone = false;
        thetaVideoAPI.PostVideo(filePathToUpload); 
        StartCoroutine(KeepChecking());
    }

    IEnumerator KeepChecking()
    {
        txtProgress.text = "0%";
        while (!videoIsDone) {
            yield return new WaitForSeconds(5f);

            thetaVideoAPI.CheckProgress();
        }

    }

    public void ViewPlaybackVideo()
    {
        Application.OpenURL(thetaVideoAPI.lastVCD.playback_uri);
    }

    public void ViewPlayerURL()
    {
        Application.OpenURL(thetaVideoAPI.lastVCD.player_uri);
    }

}

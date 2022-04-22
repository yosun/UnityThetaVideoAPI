using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Delegates for times when you just need to know the response or error
// call each with a string method to handle StringReturned and ErroReturned (optional)
namespace UnityWWWHelpers
{
    public delegate void VOIDSTRING(string s);
    public delegate void VOIDINT(int s);

    public class WWWHelpers : MonoBehaviour
    {
        public VOIDINT UploadProgress;

        public void Get(string url, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned = null, Dictionary<string, string> headers = null)
        {
            StartCoroutine(ActuallyGet(url, StringReturned, ErrorReturned));
        }

        IEnumerator ActuallyGet(string url, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null,Dictionary<string,string> headers=null)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> kvp in headers)
                {
                    www.SetRequestHeader(kvp.Key, kvp.Value);
                }
            }
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if(ErrorReturned==null)
                    Debug.Log("WWWHelpers Get ERROR: " + www.url + " " + www.error);
                else ErrorReturned?.Invoke(www.error);
            }
            else
            {
                StringReturned?.Invoke(www.downloadHandler.text);
            }
        }

        public void PostForm(string url, WWWForm form, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned = null)
        {
            StartCoroutine(ActuallyPostForm(url,form,StringReturned,ErrorReturned));
        }

        IEnumerator ActuallyPostForm(string url, WWWForm form, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null)
        {
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (ErrorReturned == null)
                    Debug.Log("WWWHelpers PostForm ERROR: " + www.url + " " + www.error);
                else ErrorReturned?.Invoke(www.error);
            }
            else
            {
                StringReturned?.Invoke(www.downloadHandler.text);
            }
        }

        public void PostRaw(string url, string raw, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned = null, Dictionary<string, string> headers = null)
        {
            StartCoroutine(ActuallyPostRaw(url,raw,StringReturned,ErrorReturned,headers));
        }

        IEnumerator ActuallyPostRaw(string url, string raw, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null, Dictionary<string, string> headers=null)
        {
            UnityWebRequest www = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
            
            if (headers != null)
            {
                foreach(KeyValuePair<string,string> kvp in headers)
                {
                    www.SetRequestHeader(kvp.Key, kvp.Value);
                }
            }else www.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(raw)) www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(raw));
            else www.uploadHandler = null;
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (ErrorReturned == null)
                    Debug.Log("WWWHelpers PostRaw ERROR: " + www.url + " " + www.error);
                else ErrorReturned?.Invoke(www.error);
            }
            else
            {
                StringReturned?.Invoke(www.downloadHandler.text);
            }
        }

        public void PutBinary(string url, byte[] bytes, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned = null,VOIDSTRING ReturnURLPlease=null)
        {
            StartCoroutine(ActuallyPutBinary(url,bytes,StringReturned,ErrorReturned, ReturnURLPlease));
        }

        IEnumerator ActuallyPutBinary(string url, byte[] bytes, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null, VOIDSTRING ReturnURLPlease = null)
        {
            UnityWebRequest www = UnityWebRequest.Put(url, UnityWebRequest.kHttpVerbPUT);
            www.SetRequestHeader("Content-Type", "application/octet-stream");
            www.uploadHandler = new UploadHandlerRaw(bytes);
            UnityWebRequestAsyncOperation op = www.SendWebRequest();

            while (!op.isDone || www.uploadProgress < 1f)
            {
                UploadProgress?.Invoke((int)(100*www.uploadProgress));
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (ErrorReturned == null)
                    Debug.Log("WWWHelpers PutBinary ERROR: " + www.url + " " + www.error);
                else ErrorReturned?.Invoke(www.error);
            }
            else
            {
                if(string.IsNullOrEmpty(www.downloadHandler.text))
                    ReturnURLPlease?.Invoke(url);
                else 
                    StringReturned?.Invoke(www.downloadHandler.text);
            }
        }

    }


}
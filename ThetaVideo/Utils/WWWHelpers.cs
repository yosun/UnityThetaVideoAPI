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

        public IEnumerator Get(string url, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
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

        public IEnumerator PostForm(string url, WWWForm form, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null)
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

        public IEnumerator PostRaw(string url, string raw, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null)
        {
            UnityWebRequest www = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(raw));
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

        public IEnumerator PutBinary(string url, byte[] bytes, VOIDSTRING StringReturned, VOIDSTRING ErrorReturned=null)
        {
            UnityWebRequest www = UnityWebRequest.Put(url, UnityWebRequest.kHttpVerbPUT);
            www.SetRequestHeader("Content-Type", "application/octet-stream");
            www.uploadHandler = new UploadHandlerRaw(bytes);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                if (ErrorReturned == null)
                    Debug.Log("WWWHelpers PutBinary ERROR: " + www.url + " " + www.error);
                else ErrorReturned?.Invoke(www.error);
            }
            else
            {
                StringReturned?.Invoke(www.downloadHandler.text);
            }
        }

    }


}
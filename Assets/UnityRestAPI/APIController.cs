using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class APIController : MonoBehaviour
{
    private static APIController _instance;
    public static APIController Instance => _instance;
    private Coroutine sendRequestCoroutine;
    private void Awake()
    {
        _instance = this;
    }
    
    public void StartSendRequest(UnityWebRequest uwr, UnityAction<string> onSuccess = null, UnityAction<string> onFailed = null)
    {
        if (sendRequestCoroutine != null)
            StopCoroutine(sendRequestCoroutine);

        sendRequestCoroutine = StartCoroutine(SendRequest(uwr, onSuccess, onFailed));
    }

    public static UnityWebRequest SetupGetRequest(string url, string tokenType, string token)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.SetRequestHeader("Authorization", tokenType + " " + token);
        return uwr;
    }

    public static UnityWebRequest SetupPostRequest(string url, WWWForm form, string tokenType = null, string token = null)
    {
        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        uwr.SetRequestHeader("Authorization", tokenType + " " + token);
        return uwr;
    }

    public static IEnumerator SendRequest(UnityWebRequest uwr, UnityAction<string> onComplete = null, UnityAction<string> onFailed = null)
    {
#if UNITY_EDITOR
        Debug.Log($"[API] Send Request to : {uwr.url}");
#endif
        yield return uwr.SendWebRequest();
#if UNITY_EDITOR
        Debug.Log($"[API] ResponseCode : {uwr.responseCode}");
        Debug.Log($"[API] Result : {uwr.result}");
        Debug.Log($"[API] Error : {uwr.error}");
        Debug.Log($"[API] downloaderHandler Text : {uwr.downloadHandler.text}");
        Debug.Log($"[API] downloaderHandler IsDone : {uwr.downloadHandler.isDone}");
#endif

        if (uwr.responseCode == 200)
        {
            onComplete?.Invoke(uwr.downloadHandler.text);
        }
        else
        {
            if (uwr.downloadHandler != null)
                onFailed?.Invoke(uwr.downloadHandler.text);
            else
                onFailed?.Invoke("{message:'" + uwr.error + "'}");
        }
    }
}
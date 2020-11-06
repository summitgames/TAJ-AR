
using System.Collections;
using UnityEngine;
using System;

#if UNITY_2017_3_OR_NEWER
using UnityEngine.Networking;
#endif

public class WWWRequestHandler : MonoBehaviour
{
	public static WWWRequestHandler Create(string name = "")
	{
		return new GameObject("[ WWWRequestHandler " + name + " ]").AddComponent<WWWRequestHandler>();
	}

	public void Request(string apiUrl, Action<bool, string> onComplete)
	{
		StartCoroutine(_CallApi(apiUrl, onComplete));
	}

    private IEnumerator _CallApi(string apiUrl, Action<bool, string> onComplete)
    {
#if UNITY_2017_3_OR_NEWER

        using (UnityWebRequest uwr = UnityWebRequest.Get(apiUrl))
        {
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                onComplete(false, "");
                Debug.Log("(UnityWebRequest) Error during call API: " + apiUrl + ", Error: " + uwr.error);
            }
            else if (uwr.isDone)
            {
                onComplete(true, uwr.downloadHandler.text);
            }
            else
            {
                Debug.Log("(UnityWebRequest) Error during call API: " + apiUrl);
                onComplete(false, "");
            }

            Destroy(gameObject);
        }

#else

        WWW www = new WWW(apiUrl);
        yield return www;

        if (www.error == null)
        {
            onComplete(true, www.text);
        }
        else
        {
            onComplete(false, "");
            Debug.Log("(WWW) Error during call API: " + apiUrl + ", Error: " + www.error);
        }

        www.Dispose();
        www = null;
        Destroy(gameObject);

#endif
    }

}

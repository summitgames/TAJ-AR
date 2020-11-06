// Created by SwanDEV 2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if UNITY_2017_3_OR_NEWER
using UnityEngine.Networking;
#endif

#if !USE_BUILD_IN_JSON
using Newtonsoft.Json;
#endif

public class GiphyManager : MonoBehaviour
{
	public enum Rating
	{
		None = 0,
		Y,
		G,
		PG,
		PG_13,
		R
	}

	//lang - specify default country for regional content; format is 2-letter ISO 639-1 language code
	public enum Language
	{
		None = 0,
		en, //English
		es, //Spanish
		pt, //Portuguese
		id, //Indonesian
		fr, //French
		ar, //Arabic
		tr, //Turkish
		th, //Thai
		vi, //Vietnamese
		de, //German
		it, //Italian
		ja, //Japanese
		ru, //Russian
		ko, //Korean
		pl, //Polish
		nl, //Dutch
		ro, //Romanian
		hu, //Hungarian
		sv, //Swedish
		cs, //Czech
		hi, //Hindi
		bn, //Bengali
		da, //Danish
		fa, //Farsi
		tl, //Filipino
		fi, //Finnish
		iw, //Hebrew
		ms, //Malay
		no, //Norwegian
		uk, //Ukrainian
		CN, //Chinese Simplified
		TW, //Chinese Traditional
	}

    #region ------- GIF API Variables  -------

    //----- Apply your gif channel & api keys here: https://developers.giphy.com/ -----//
    /// <summary>
    /// Your user name on Giphy.
    /// </summary>
    [Header("[ Giphy Channel ]")]
    [Tooltip("Your user name on Giphy.")]
	public string m_GiphyUserName = "";

    /// <summary>
    /// Your API key associated with your app on Giphy (for normal Gif API and Sticker API)
    /// </summary>
    [Tooltip("Your API key associated with your app on Giphy (for normal Gif API and Sticker API)")]
    public string m_GiphyApiKey = "";

    /// <summary>
    /// Your upload API key associated with your app on Giphy (for Upload API, you can request a production key from Giphy if need)
    /// </summary>
    [Tooltip("Your upload API key associated with your app on Giphy (for Upload API, you can request a production key from Giphy if need)")]
    public string m_GiphyUploadApiKey = "";


	[Header("[ Giphy API URL ]")]
	public string m_NormalGifApi = "https://api.giphy.com/v1/gifs";
	public string m_StickerApi = "https://api.giphy.com/v1/stickers";
	public string m_UploadApi = "https://upload.giphy.com/v1/gifs";


    /// <summary>
    /// Number of results to return, maximum 100. Default: 25
    /// </summary>
    [Header("[ Optional-Settings ]")]
    [Tooltip("Number of results to return, maximum 100. Default: 25")]
    public int m_ResultLimit = 10;

    /// <summary>
    /// Results offset, default: 0
    /// </summary>
    [Tooltip("Results offset, default: 0")]
    public int m_ResultOffset = 0;

    /// <summary>
    /// Limit results to those rated GIFs (y,g, pg, pg-13 or r)
    /// </summary>
    [Tooltip("Limit results to those rated GIFs (y,g, pg, pg-13 or r)")]
    public Rating m_Rating = Rating.None;

    /// <summary>
    /// Language use with GIFs Search API and Stickers Search API, specify default country for regional content. Default: en
    /// </summary>
    [Tooltip("Language use with GIFs Search API and Stickers Search API, specify default country for regional content. Default: en")]
    public Language m_Language = Language.None;

    /// <summary>
    /// An url attach to the GIF Info>SOURCE field, for the uploaded GIF (e.g. Your website/The page on which this GIF was found)
    /// </summary>
	[Header("[ Optional-Promotion ]")]
    [Tooltip("An url attach to the GIF Info>SOURCE field, for the uploaded GIF (e.g. Your website/The page on which this GIF was found)")]
    public string m_Source_Post_Url = "";

    #endregion


    private string _FullJsonResponseText = "Full Json Response Text"; 
	public string FullJsonResponseText
	{
		get{
			return _FullJsonResponseText;
		}
		set{
			string json = value.Replace("\\", "");
			Debug.Log(json);

			_FullJsonResponseText = value;
		}
	}

	private static GiphyManager _instance = null;
	public static GiphyManager Instance
	{
		get{
			if(_instance == null)
			{
				_instance = new GameObject("[GiphyManager]").AddComponent<GiphyManager>();
			}
			return _instance;
		}
	}

	public void SetChannelAuthentication(string userName, string apiKey = "", string uploadApiKey = "")
	{
		m_GiphyUserName = userName;
		m_GiphyApiKey = apiKey;
		m_GiphyUploadApiKey = uploadApiKey;
	}

	private bool HasUserName
	{
		get{
			bool hasUserName = !string.IsNullOrEmpty(m_GiphyUserName);
			if(!hasUserName) 
			{
#if UNITY_EDITOR
				Debug.LogWarning("Giphy User Name is required if you use a production api key.");
#endif
			}
			return true;
		}
	}

	private bool HasApiKey
	{
		get{
			bool hasApiKey = !string.IsNullOrEmpty(m_GiphyApiKey);
			if(!hasApiKey)
            {
                Debug.LogWarning("Giphy API Key is required!");
            }

            return hasApiKey;
		}
	}

	private bool HasUploadApiKey
	{
		get{
			bool hasApiKey = !string.IsNullOrEmpty(m_GiphyUploadApiKey);
			if(!hasApiKey)
            {
                Debug.LogWarning("Giphy Upload API Key is required!");
            }

            return hasApiKey;
		}
	}

	void Start()
	{
		if(_instance == null)
		{
			_instance = this;
		}
	}
		
	void Update()
	{
#if UNITY_2017_3_OR_NEWER
        if (uwrUpload != null)
        {
            if (_onUploadProgress != null)
            {
                _onUploadProgress(uwrUpload.uploadProgress);
            }
        }
#else
        if (wwwUpload != null)
        {
            if (_onUploadProgress != null)
            {
                _onUploadProgress(wwwUpload.uploadProgress);
            }
        }
#endif
    }

#region ------- Upload GIF API -------
#if UNITY_2017_3_OR_NEWER
    private UnityWebRequest uwrUpload;
#else
    private WWW wwwUpload;
#endif
    private Action<float> _onUploadProgress;

	/// <summary>
	/// Upload a specified GIF with its filePath to Giphy.
	/// </summary>
	/// <param name="filePath">File path.</param>
	/// <param name="onComplete">On complete.</param>
	/// <param name="onProgress">On progress.</param>
	public void Upload(string filePath, Action<GiphyUpload.Response> onComplete, Action<float> onProgress = null, Action onFail = null)
	{
		if(!HasUploadApiKey || !HasUserName) return;
		StartCoroutine(_Upload(filePath, null, onComplete, onProgress, onFail));
	}

	/// <summary>
	/// Upload a specified GIF with its filePath to Giphy. 
	/// And add tag(s) to the GIF that allows it to be searched by browsing/searching.
	/// </summary>
	/// <param name="filePath">File path.</param>
	/// <param name="tags">Tags.</param>
	/// <param name="onComplete">On complete.</param>
	/// <param name="onProgress">On progress.</param>
	public void Upload(string filePath, List<string> tags, Action<GiphyUpload.Response> onComplete, Action<float> onProgress = null, Action onFail = null)
	{
		if(!HasUploadApiKey || !HasUserName) return;
		StartCoroutine(_Upload(filePath, tags, onComplete, onProgress, onFail));
	}

    private IEnumerator _Upload(string filePath, List<string> tags, Action<GiphyUpload.Response> onComplete, Action<float> onProgress = null, Action onFail = null)
    {
        //string url = m_UploadApi;
        string url = m_UploadApi + "?api_key=" + m_GiphyUploadApiKey;// + (string.IsNullOrEmpty(m_GiphyUserName)? "":"&username=" + m_GiphyUserName);

        _onUploadProgress = onProgress;

        string tagsStr = "";
        if (tags != null && tags.Count > 0)
        {
            foreach (string hTag in tags)
            {
                if (!string.IsNullOrEmpty(hTag)) tagsStr += hTag + ",";
            }
            if (!string.IsNullOrEmpty(tagsStr)) tagsStr = tagsStr.Substring(0, tagsStr.Length - 1);
        }

        byte[] bytes = File.ReadAllBytes(filePath);

        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("file", bytes);
        postForm.AddField("api_key", m_GiphyUploadApiKey);
        postForm.AddField("username", m_GiphyUserName);
        if (!string.IsNullOrEmpty(tagsStr)) postForm.AddField("tags", tagsStr);
        if (!string.IsNullOrEmpty(m_Source_Post_Url)) postForm.AddField("source_post_url", m_Source_Post_Url);

#if UNITY_EDITOR
        Debug.Log("UserName: " + m_GiphyUserName + " | Upload Api Key: " + m_GiphyUploadApiKey +
            " | Api Key: " + m_GiphyApiKey + " | tags: " + tagsStr + " | m_Source_Post_Url: " + m_Source_Post_Url);
#endif

#if UNITY_2017_3_OR_NEWER

        uwrUpload = UnityWebRequest.Post(url, postForm);
        yield return uwrUpload.SendWebRequest();
        if (uwrUpload.isNetworkError || uwrUpload.isHttpError)
        {
            if (onFail != null) onFail();
            Debug.LogWarning("Error during upload: " + uwrUpload.error + "\nResult: " + uwrUpload.downloadHandler.text);
        }
        else if (uwrUpload.isDone)
        {
            FullJsonResponseText = uwrUpload.downloadHandler.text;
#if USE_BUILD_IN_JSON
            GiphyUpload.Response uploadResponse = JsonUtility.FromJson<GiphyUpload.Response>(w.downloadHandler.text);
#else
            GiphyUpload.Response uploadResponse = JsonConvert.DeserializeObject<GiphyUpload.Response>(uwrUpload.downloadHandler.text);
#endif
            onComplete(uploadResponse);
        }
        else
        {
            if (onFail != null) onFail();
            Debug.LogWarning("Fail to upload!");
        }

        if (uwrUpload != null)
        {
            uwrUpload.Dispose();
            uwrUpload = null;
        }

#else

        wwwUpload = new WWW(url, postForm);
        wwwUpload.threadPriority = ThreadPriority.High;

        yield return wwwUpload;
        if (!string.IsNullOrEmpty(wwwUpload.error))
        {
            if (onFail != null) onFail();
            Debug.Log("Error during upload: " + wwwUpload.error + "\n" + wwwUpload.text);
        }
        else
        {
            FullJsonResponseText = wwwUpload.text;
#if USE_BUILD_IN_JSON
            GiphyUpload.Response uploadResponse = JsonUtility.FromJson<GiphyUpload.Response>(wwwUpload.text);
#else
            GiphyUpload.Response uploadResponse = JsonConvert.DeserializeObject<GiphyUpload.Response>(wwwUpload.text);
#endif
            onComplete(uploadResponse);
        }

        if (wwwUpload != null)
        {
            wwwUpload.Dispose();
            wwwUpload = null;
        }

#endif

    }
#endregion

#region ------- Normal GIF API --------
    /// <summary>
    /// Returns a GIF given that GIF's unique ID
    /// </summary>
    /// <param name="giphyGifId">Giphy GIF identifier.</param>
    /// <param name="onComplete">On complete.</param>
    public void GetById(string giphyGifId, Action<GiphyGetById.Response> onComplete, Action onFail = null)
    {
        if (!HasApiKey || !HasUserName) return;

        if (!string.IsNullOrEmpty(giphyGifId))
		{
			string url = m_NormalGifApi +"/" + giphyGifId + "?api_key=" + m_GiphyApiKey;

            StartCoroutine(_LoadRoutine(url, (text) =>
            {
                if (!string.IsNullOrEmpty(text))
                {
#if USE_BUILD_IN_JSON
                    GiphyGetById.Response response = JsonUtility.FromJson<GiphyGetById.Response>(text);
#else
                    GiphyGetById.Response response = JsonConvert.DeserializeObject<GiphyGetById.Response>(text);
#endif
                    if (onComplete != null) onComplete(response);
                }
                else
                {
                    if (onFail != null) onFail();
                }
            }));
		}
		else
		{
			Debug.LogWarning("GIF id is empty!");
		}
	}

    /// <summary>
    /// Returns an array of GIFs given that GIFs' unique IDs
    /// </summary>
    /// <param name="giphyGifIds">Giphy GIF identifiers.</param>
    /// <param name="onComplete">On complete.</param>
    public void GetByIds(List<string> giphyGifIds, Action<GiphyGetByIds.Response> onComplete, Action onFail = null)
    {
        if (!HasApiKey || !HasUserName) return;

        string giphyGifIdsStr = "";
		foreach(string id in giphyGifIds)
		{
			if(!string.IsNullOrEmpty(id)) giphyGifIdsStr += id + ",";
		}

		if(!string.IsNullOrEmpty(giphyGifIdsStr))
		{
			giphyGifIdsStr = giphyGifIdsStr.Substring(0, giphyGifIdsStr.Length-1);
		
			string url = m_NormalGifApi + "?ids=" + giphyGifIdsStr + "&api_key=" + m_GiphyApiKey;

            StartCoroutine(_LoadRoutine(url, (text) =>
            {
                if (!string.IsNullOrEmpty(text))
                {
#if USE_BUILD_IN_JSON
                    GiphyGetByIds.Response response = JsonUtility.FromJson<GiphyGetByIds.Response>(text);
#else
                    GiphyGetByIds.Response response = JsonConvert.DeserializeObject<GiphyGetByIds.Response>(text);
#endif
                    if (onComplete != null) onComplete(response);
                }
                else
                {
                    if (onFail != null) onFail();
                }
            }));
		}
		else
		{
			Debug.LogWarning("GIF ids is empty!");
		}
	}

    /// <summary>
    /// Search all GIPHY GIFs for a word or phrase. Punctuation will be stripped and ignored.
    /// </summary>
    /// <param name="keyWords">Key words.</param>
    /// <param name="onComplete">On complete.</param>
    public void Search(List<string> keyWords, Action<GiphySearch.Response> onComplete, Action onFail = null)
	{
        if (!HasApiKey || !HasUserName) return;

        string keyWordsStr = "";
		foreach(string k in keyWords)
		{
			keyWordsStr += k + "+";
		}
		keyWordsStr = keyWordsStr.Substring(0, keyWordsStr.Length-1);

		string url = m_NormalGifApi + "/search?q=" + keyWordsStr + "&api_key=" + m_GiphyApiKey;
		if(m_ResultLimit > 0) url += "&limit=" + m_ResultLimit;
		if(m_ResultOffset > 0) url += "&offset=" + m_ResultOffset;
		if(m_Rating != Rating.None) url += "&rating=" + m_Rating.ToString().ToUpper();
        if (m_Language != Language.None) url += "&lang=" + _GetLanguageString(m_Language);

        StartCoroutine(_LoadRoutine(url, (text) =>
        {
            if (!string.IsNullOrEmpty(text))
            {
#if USE_BUILD_IN_JSON
                GiphySearch.Response response = JsonUtility.FromJson<GiphySearch.Response>(text);
#else
                GiphySearch.Response response = JsonConvert.DeserializeObject<GiphySearch.Response>(text);
#endif
                if (onComplete != null) onComplete(response);
            }
            else
            {
                if(onFail != null) onFail();
            }
        }));
	}

	/// <summary>
	/// Get a random GIF from Giphy
	/// </summary>
	/// <param name="onComplete">On complete.</param>
	public void Random(Action<GiphyRandom.Response> onComplete, Action onFail = null)
	{
		_Random(null, onComplete, onFail);
	}

	/// <summary>
	/// Get a random GIF by tag.
	/// </summary>
	/// <param name="hTag">Tag: the GIF tag to limit randomness by</param>
	/// <param name="onComplete">On complete.</param>
	public void Random(string hTag, Action<GiphyRandom.Response> onComplete, Action onFail = null)
	{
		_Random(hTag, onComplete, onFail);
	}

    private void _Random(string hTag, Action<GiphyRandom.Response> onComplete, Action onFail = null)
	{
        if (!HasApiKey || !HasUserName) return;

        string url = m_NormalGifApi + "/random?api_key=" + m_GiphyApiKey;
		if(!string.IsNullOrEmpty(hTag)) url += "&tag=" + hTag;
		if(m_Rating != Rating.None) url += "&rating=" + m_Rating.ToString().ToUpper();

        StartCoroutine(_LoadRoutine(url, (text) =>
        {
            if (!string.IsNullOrEmpty(text))
            {
#if USE_BUILD_IN_JSON
                GiphyRandom.Response response = JsonUtility.FromJson<GiphyRandom.Response>(text);
#else
                GiphyRandom.Response response = JsonConvert.DeserializeObject<GiphyRandom.Response>(text);
#endif
                if (onComplete != null) onComplete(response);
            }
            else
            {
                if (onFail != null) onFail();
            }
        }));
	}

    /// <summary>
    /// The translate API draws on search, but uses the GIPHY special sauce to handle translating from one vocabulary to another. 
    /// In this case, words and phrases to GIFs. The result is Random even for the same term.
    /// </summary>
    /// <param name="term">The term to be translated.</param>
    /// <param name="onComplete">On complete callback.</param>
    public void Translate(string term, Action<GiphyTranslate.Response> onComplete, Action onFail = null)
	{
		if(!string.IsNullOrEmpty(term))
		{
			string url = m_NormalGifApi + "/translate?api_key=" + m_GiphyApiKey + "&s=" + term;
			if(m_Rating != Rating.None) url += "&rating=" + m_Rating.ToString().ToUpper();

            StartCoroutine(_LoadRoutine(url, (text) =>
            {
                if (!string.IsNullOrEmpty(text))
                {
#if USE_BUILD_IN_JSON
                    GiphyTranslate.Response response = JsonUtility.FromJson<GiphyTranslate.Response>(text);
#else
                    GiphyTranslate.Response response = JsonConvert.DeserializeObject<GiphyTranslate.Response>(text);
#endif
                    if (onComplete != null) onComplete(response);
                }
                else
                {
                    if (onFail != null) onFail();
                }
            }));
		}
		else
		{
			Debug.LogWarning("Search term is empty!");
		}
	}

    /// <summary>
    /// Fetch GIFs currently trending online. Hand curated by the GIPHY editorial team. 
    /// The data returned mirrors the GIFs showcased on the GIPHY homepage.
    /// </summary>
    /// <param name="onComplete">On complete.</param>
    public void Trending(Action<GiphyTrending.Response> onComplete, Action onFail = null)
	{
		string url = m_NormalGifApi + "/trending?api_key=" + m_GiphyApiKey;
		if(m_ResultLimit > 0) url += "&limit=" + m_ResultLimit;
		if(m_ResultOffset > 0) url += "&offset=" + m_ResultOffset;
		if(m_Rating != Rating.None) url += "&rating=" + m_Rating.ToString().ToUpper();

        StartCoroutine(_LoadRoutine(url, (text) =>
        {
            if (!string.IsNullOrEmpty(text))
            {
#if USE_BUILD_IN_JSON
                GiphyTrending.Response response = JsonUtility.FromJson<GiphyTrending.Response>(text);
#else
                GiphyTrending.Response response = JsonConvert.DeserializeObject<GiphyTrending.Response>(text);
#endif
                if (onComplete != null) onComplete(response);
            }
            else
            {
                if (onFail != null) onFail();
            }
        }));
	}
#endregion


#region -------- Sticker GIF API --------
	private string _GetLanguageString(Language lang)
	{
		string langStr = "";
		switch(lang)
		{
		case Language.None:
			//Do nothing
			break;

		case Language.CN:
			langStr = "zh-CN";
			break;

		case Language.TW:
			langStr = "zh-TW";
			break;

		default:
			langStr = lang.ToString().ToLower();
			break;
		}
		return langStr;
	}

    public void Search_Sticker(List<string> keyWords, Action<GiphyStickerSearch.Response> onComplete, Action onFail = null)
	{
        if (!HasApiKey || !HasUserName) return;

        string keyWordsStr = "";
		foreach(string k in keyWords)
		{
			keyWordsStr += k + "+";
		}
		keyWordsStr = keyWordsStr.Substring(0, keyWordsStr.Length-1);

		string url = m_StickerApi + "/search?q=" + keyWordsStr + "&api_key=" + m_GiphyApiKey;
		if(m_ResultLimit > 0) url += "&limit=" + m_ResultLimit;
		if(m_ResultOffset > 0) url += "&offset=" + m_ResultOffset;
		if(m_Rating != Rating.None) url += "&rating=" + m_Rating.ToString().ToUpper();
		if(m_Language != Language.None) url += "&lang=" + _GetLanguageString(m_Language);

        StartCoroutine(_LoadRoutine(url, (text) =>
        {
            if (!string.IsNullOrEmpty(text))
            {
#if USE_BUILD_IN_JSON
                GiphyStickerSearch.Response searchResponse = JsonUtility.FromJson<GiphyStickerSearch.Response>(text);
#else
                GiphyStickerSearch.Response searchResponse = JsonConvert.DeserializeObject<GiphyStickerSearch.Response>(text);
#endif
                if (onComplete != null) onComplete(searchResponse);
            }
            else
            {
                if (onFail != null) onFail();
            }
        }));
	}

	public void Random_Sticker(Action<GiphyStickerRandom.Response> onComplete, Action onFail = null)
	{
		_Random_Sticker(null, onComplete, onFail);
	}

	public void Random_Sticker(string hTag, Action<GiphyStickerRandom.Response> onComplete, Action onFail = null)
	{
		_Random_Sticker(hTag, onComplete, onFail);
	}

    private void _Random_Sticker(string hTag, Action<GiphyStickerRandom.Response> onComplete, Action onFail = null)
	{
        if (!HasApiKey || !HasUserName) return;

        string url = m_StickerApi + "/random?api_key=" + m_GiphyApiKey;
		if(!string.IsNullOrEmpty(hTag)) url += "&tag=" + hTag;
		if(m_Rating != Rating.None) url += "&rating=" + m_Rating.ToString().ToUpper();

        StartCoroutine(_LoadRoutine(url, (text) =>
        {
            if (!string.IsNullOrEmpty(text))
            {
#if USE_BUILD_IN_JSON
                GiphyStickerRandom.Response searchResponse = JsonUtility.FromJson<GiphyStickerRandom.Response>(text);
#else
                GiphyStickerRandom.Response searchResponse = JsonConvert.DeserializeObject<GiphyStickerRandom.Response>(text);
#endif
                if (onComplete != null) onComplete(searchResponse);
            }
            else
            {
                if (onFail != null) onFail();
            }
        }));
	}

    public void Translate_Sticker(string term, Action<GiphyStickerTranslate.Response> onComplete, Action onFail = null)
	{
        if (!HasApiKey || !HasUserName) return;

        if (!string.IsNullOrEmpty(term))
		{
			string url = m_StickerApi + "/translate?api_key=" + m_GiphyApiKey + "&s=" + term;

            StartCoroutine(_LoadRoutine(url, (text) =>
            {
                if (!string.IsNullOrEmpty(text))
                {
#if USE_BUILD_IN_JSON
                    GiphyStickerTranslate.Response searchResponse = JsonUtility.FromJson<GiphyStickerTranslate.Response>(text);
#else
                    GiphyStickerTranslate.Response searchResponse = JsonConvert.DeserializeObject<GiphyStickerTranslate.Response>(text);
#endif
                    if (onComplete != null) onComplete(searchResponse);
                }
                else
                {
                    if (onFail != null) onFail();
                }
            }));
		}
		else
		{
			Debug.LogWarning("Search term is empty!");
		}
	}

    public void Trending_Sticker(Action<GiphyStickerTrending.Response> onComplete, Action onFail = null)
	{
        if (!HasApiKey || !HasUserName) return;

        string url = m_StickerApi + "/trending?api_key=" + m_GiphyApiKey;
		if(m_ResultLimit > 0) url += "&limit=" + m_ResultLimit;
        if(m_ResultOffset > 0) url += "&offset=" + m_ResultOffset;
		if(m_Rating != Rating.None) url += "&rating=" + m_Rating.ToString().ToUpper();

        StartCoroutine(_LoadRoutine(url, (text) =>
        {
            if (!string.IsNullOrEmpty(text))
            {
#if USE_BUILD_IN_JSON
                GiphyStickerTrending.Response searchResponse = JsonUtility.FromJson<GiphyStickerTrending.Response>(text);
#else
                GiphyStickerTrending.Response searchResponse = JsonConvert.DeserializeObject<GiphyStickerTrending.Response>(text);
#endif
                if (onComplete != null) onComplete(searchResponse);
            }
            else
            {
                if (onFail != null) onFail();
            }
        }));
	}
#endregion


#region -------- Load Routine --------
    private IEnumerator _LoadRoutine(string url, Action<string> onReceive)
    {
#if UNITY_2017_3_OR_NEWER

        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                onReceive("");
                Debug.LogWarning("Error during UnityWebRequest loading: " + uwr.error);
            }
            else if (uwr.isDone)
            {
                onReceive(uwr.downloadHandler.text);
                FullJsonResponseText = uwr.downloadHandler.text;
            }
            else
            {
                Debug.LogWarning("Error during UnityWebRequest loading.");
                onReceive("");
            }
        }

#else

        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            FullJsonResponseText = www.text;
            onReceive(www.text);
        }
        else
        {
            onReceive("");
            Debug.LogWarning("Error during WWW loading: " + www.error);
        }
        www.Dispose();
        www = null;

#endif
    }
    #endregion
}

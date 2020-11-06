// Created by SwanDEV 2017

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_2017_3_OR_NEWER
using UnityEngine.Networking;
#endif

public class MobileMediaTest : DImageDisplayHandler
{
    public CanvasScaler canvasScaler;

    public RawImage displayImage;
    public Toggle tog_Popup;

    public Toggle tog_UseNewContentPicker;
    public Toggle tog_LocalOnly;

    public Text debugText;

    public GameObject container_AndroidOnly;
    public GameObject container_iOSOnly;

    private string hints = "Mobile Media Plugin Demo\nTo test save/pick media to/from Native, please build Android, iOS app and test on device.\n\n";

    private FilePathName filePathName = new FilePathName();

    void Start()
    {
        debugText.text = hints;

        //Check screen orientation for setting canvas resolution
        if (Screen.width > Screen.height)
        {
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
        }
        else
        {
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
        }

#if !UNITY_EDITOR
        if (Application.platform != RuntimePlatform.Android) container_AndroidOnly.SetActive(false);
        if (Application.platform != RuntimePlatform.IPhonePlayer) container_iOSOnly.SetActive(false);
#endif
    }

    public void GetThumbnail(DImageDisplayHandler imageHandler)
    {
        RawImage getImage = imageHandler.GetComponent<RawImage>();
        int fileType = 2;   // Photo = 0, Video = 1, Photo & Video = 2 
        int fileIndex = 0;  // 0: Latest photo/video

        MobileMedia.GetMediaThumbnail((imagePath) =>
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                Debug.Log("Image Path: " + imagePath);
                debugText.text = hints + "Image Path: " + imagePath;

                Texture2D texture2D = filePathName.LoadImage(imagePath);
                if (texture2D)
                {
                    imageHandler.SetRawImage(getImage, texture2D); // Display image

                    _ShowLargePreviewImage(texture2D);
                }
            }
            else
            {
                debugText.text = hints + "Path is empty or null.";
                Debug.Log("Path is empty or null.");
            }

        }, fileType, fileIndex, 96, "MobileMediaTest");
    }

    public void GetPhoto(DImageDisplayHandler imageHandler)
    {
        RawImage getImage = imageHandler.GetComponent<RawImage>();
        int fileType = 2;   // Photo = 0, Video = 1, Photo & Video = 2 
        int fileIndex = 0;  // 0: Latest photo/video

        MobileMedia.GetMediaPhoto((imagePath) =>
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                Debug.Log("Image Path: " + imagePath);
                debugText.text = hints + "Image Path: " + imagePath;

                Texture2D texture2D = filePathName.LoadImage(imagePath);
                if (texture2D)
                {
                    imageHandler.SetRawImage(getImage, texture2D); // Display image

                    _ShowLargePreviewImage(texture2D);
                }
            }
            else
            {
                debugText.text = hints + "Path is empty or null.";
                Debug.Log("Path is empty or null.");
            }

        }, fileType, fileIndex, "MobileMediaTest");
    }

    /// <summary>
    /// Gets the photo thumbnail in the Album (iOS only). 
    /// </summary>
    public void GetPhotoThumbnail_iOS(DImageDisplayHandler imageHandler)
    {
        RawImage getImage = imageHandler.GetComponent<RawImage>();
        int fileType = 2;   // Photo = 0, Video = 1, Photo & Video = 2 
        int fileIndex = 0;  // 0: Latest photo/video
        int targetSize = 0; // Use default size = 0

        MobileMedia.GetMediaThumbnail_IOS((imagePath) =>
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                Debug.Log("Image Path: " + imagePath);
                debugText.text = hints + "Image Path: " + imagePath;

                Texture2D texture2D = filePathName.LoadImage(imagePath);
                if (texture2D)
                {
                    imageHandler.SetRawImage(getImage, texture2D); // Display image

                    _ShowLargePreviewImage(texture2D);
                }
            }
            else
            {
                debugText.text = hints + "Path is empty or null.";
                Debug.Log("Path is empty or null.");
            }

        }, fileType, fileIndex, targetSize, "thumbnail_temp");
    }

    /// <summary>
    /// Gets the last media photo in the Album (iOS only). 
    /// </summary>
    public void GetLastPhoto_iOS(DImageDisplayHandler imageHandler)
    {
        RawImage getImage = imageHandler.GetComponent<RawImage>();
        int fileType = 2;   // Photo = 0, Video = 1, Photo & Video = 2 
        int fileIndex = 0;  // 0: Latest photo/video

        MobileMedia.GetMediaPhoto_IOS((imagePath) =>
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                Debug.Log("Image Path: " + imagePath);
                debugText.text = hints + "Image Path: " + imagePath;

                Texture2D texture2D = filePathName.LoadImage(imagePath);
                if (texture2D)
                {
                    imageHandler.SetRawImage(getImage, texture2D); // Display image

                    _ShowLargePreviewImage(texture2D);
                }
            }
            else
            {
                debugText.text = hints + "Path is empty or null.";
                Debug.Log("Path is empty or null.");
            }

        }, fileType, fileIndex, "LastPhoto_temp");
    }

    public void PickImage()
    {
        MobileMedia.Android_NewContentPicker = tog_UseNewContentPicker.isOn;
        MobileMedia.Android_NewContentPicker_LocalOnly = tog_LocalOnly.isOn;

        MobileMedia.PickImage((imagePath) =>
        {
            // Implement your code to load & use the image using the returned image path:
            if (!string.IsNullOrEmpty(imagePath))
            {
                //FileInfo fileInfo = new FileInfo(imagePath);
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                texture2D.LoadImage(imageBytes);

                // Display image
                _ShowLargePreviewImage(texture2D);

                Debug.Log("Image Path: " + imagePath);
                debugText.text = hints + "Image Path: " + imagePath;
            }
            else
            {
                debugText.text = hints + "Path is empty or null.";
                Debug.Log("Path is empty or null.");
            }
        }, "Pick an Image", "image/*", tog_Popup.isOn);
    }

    public void Pick_JPG_Android()
    {
        MobileMedia.Android_NewContentPicker = tog_UseNewContentPicker.isOn;
        MobileMedia.Android_NewContentPicker_LocalOnly = tog_LocalOnly.isOn;

        MobileMedia.PickImage((imagePath) =>
        {
            // Implement your code to load & use the image using the returned image path:
            if (!string.IsNullOrEmpty(imagePath))
            {
                //FileInfo fileInfo = new FileInfo(imagePath);
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                texture2D.LoadImage(imageBytes);

                // Display image
                _ShowLargePreviewImage(texture2D);

                Debug.Log("Image Path: " + imagePath);
                debugText.text = hints + "Image Path: " + imagePath;
            }
            else
            {
                debugText.text = hints + "Path is empty or null.";
                Debug.Log("Path is empty or null.");
            }
        }, "Pick a JPG", "image/jpeg", tog_Popup.isOn);
    }

    public void Pick_GIF_Android()
    {
        MobileMedia.Android_NewContentPicker = tog_UseNewContentPicker.isOn;
        MobileMedia.Android_NewContentPicker_LocalOnly = tog_LocalOnly.isOn;

        MobileMedia.PickImage((imagePath) =>
        {
            // Implement your code to load & use the image using the returned image path:
            if (!string.IsNullOrEmpty(imagePath))
            {
                //FileInfo fileInfo = new FileInfo(imagePath);
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                texture2D.LoadImage(imageBytes);

                // Display image
                _ShowLargePreviewImage(texture2D);

                Debug.Log("Image Path: " + imagePath);
                debugText.text = hints + "Image Path: " + imagePath;
            }
            else
            {
                debugText.text = hints + "Path is empty or null.";
                Debug.Log("Path is empty or null.");
            }
        }, "Pick a GIF", "image/gif", tog_Popup.isOn);
    }

    public void PickVideo()
    {
        MobileMedia.Android_NewContentPicker = tog_UseNewContentPicker.isOn;
        MobileMedia.Android_NewContentPicker_LocalOnly = tog_LocalOnly.isOn;

        MobileMedia.PickVideo((videoPath) =>
        {
            // Implement your code to load & play the video with the videoPath.

            if (!string.IsNullOrEmpty(videoPath))
            {
                Debug.Log("Video Path: " + videoPath);
                FileInfo fileInfo = new FileInfo(videoPath);
                debugText.text = hints + "File Size: " + fileInfo.Length
                    + "\nVideo Path: " + videoPath + "\n(Video player feature not provided. Please implement your player to play the video by path)";
                _PlayVideo_Handheld(videoPath);
            }
            else
            {
                Debug.Log("Path is empty or null.");
            }
        }, "Pick a Video", "video/*", tog_Popup.isOn);
    }

    private AudioSource audioSource = null;
    public void PickAudio_Android()
    {
        MobileMedia.Android_NewContentPicker = tog_UseNewContentPicker.isOn;
        MobileMedia.Android_NewContentPicker_LocalOnly = tog_LocalOnly.isOn;

        MobileMedia.PickAudioAndroid((audioPath) =>
        {
            // Implement your code to load & play the video using the returned video path:
            if (!string.IsNullOrEmpty(audioPath))
            {
                Debug.Log("Audio Path: " + audioPath);
                debugText.text = hints + "Audio Path: " + audioPath;

                StartCoroutine(_LoadAudioFile(audioPath, (audioClip) =>
                {
                    if (audioClip)
                    {
                        _PlayAudio(audioClip);
                        debugText.text += " " + audioSource.name;
                    }
                }));
            }
            else
            {
                Debug.Log("Path is empty or null.");
            }
        }, "Pick an Audio", "audio/*", tog_Popup.isOn);
    }

    private void _PlayAudio(AudioClip audioClip)
    {
        if (audioSource == null)
        {
            audioSource = new GameObject("[AudioPlayer]").AddComponent<AudioSource>();
        }
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    /// <summary>
    /// To test save Audio file to Native (Android). Put your audio file in the Assets/StreamingAssets folder and rename it to SampleMP3.mp3.
    /// </summary>
    public void SaveAudio_Android()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "SampleMP3.mp3");

        if (!File.Exists(filePath) && !IsAndroidOrWebGL)
        {
            Debug.LogWarning("To test save Audio file to Native. Put your audio file in the Assets/StreamingAssets folder and rename it to SampleMP3.mp3.");
            return;
        }

        string tmpText = hints + "(SaveAudio) Origin file path: " + filePath;
        debugText.text = tmpText;

        _LoadFile(filePath, (bytes) =>
        {
            if (bytes != null)
            {
                //string savePath = MobileMedia.SaveBytes(bytes, "MobileMediaTest", "Audio_" + filePathName.GetFileNameWithoutExt(), Path.GetExtension(filePath), MobileMedia.MediaType.Audio_Android);
                string savePath = MobileMedia.SaveAudioAndroid(bytes, "MobileMediaTest", "Audio_" + filePathName.GetFileNameWithoutExt(), Path.GetExtension(filePath));
                debugText.text = hints + "Audio Path: " + savePath;
            }
        });
    }

    public void SaveJPG()
    {
        TakeScreenshot((tex2D) =>
        {
            string savePath = MobileMedia.SaveImage(tex2D, "MobileMediaTest", filePathName.GetJpgFileName(), MobileMedia.ImageFormat.JPG);
            _ShowLargePreviewImage(tex2D);

            debugText.text = hints + "Save Path: " + savePath;
            Debug.Log("Save Path: " + savePath);
        });
    }

    public void SavePNG()
    {
        TakeScreenshot((tex2D) =>
        {
            string savePath = MobileMedia.SaveImage(tex2D, "MobileMediaTest", filePathName.GetPngFileName(), MobileMedia.ImageFormat.PNG);
            _ShowLargePreviewImage(tex2D);

            debugText.text = hints + "Save Path: " + savePath;
            Debug.Log("Save Path: " + savePath);
        });
    }

    /// <summary>
    /// To test save GIF file to Native. Put your GIF file in the Assets/StreamingAssets folder and rename it to SampleGIF.gif.
    /// </summary>
    public void SaveGIF()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "SampleGIF.gif");

        if (!File.Exists(filePath) && !IsAndroidOrWebGL)
        {
            Debug.LogWarning("To test save GIF file to Native. Put your GIF file in the Assets/StreamingAssets folder and rename it to SampleGIF.gif.");
            return;
        }

        string tmpText = hints + "(SaveGIF) Origin file path: " + filePath;
        debugText.text = tmpText;

        _LoadFile(filePath, (bytes) =>
        {
            if (bytes != null)
            {
                string savePath = MobileMedia.SaveBytes(bytes, "MobileMediaTest", filePathName.GetFileNameWithoutExt(), Path.GetExtension(filePath).ToLower(), true);
                debugText.text += "\n\nSave path: " + savePath;
            }
        });
    }

    /// <summary>
    /// Convert texture(s) to GIF (Requires Pro GIF). 
    /// </summary>
    public void ConvertToGIF()
    {
        TakeScreenshot((tex2D) =>
        {
            string savePath = MobileMedia.SaveImage(tex2D, "MobileMediaTest", filePathName.GetGifFileName(), MobileMedia.ImageFormat.GIF);
            _ShowLargePreviewImage(tex2D);

            debugText.text = hints + "Save Path: " + savePath;
            Debug.Log("Save Path: " + savePath);
        });
    }

    /// <summary>
    /// To test save MP4 file to Native. Put your MP4 file in the Assets/StreamingAssets folder and rename it to SampleMP4.mp4.
    /// </summary>
    public void SaveMP4()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "SampleMP4.mp4");

        if (!File.Exists(filePath) && !IsAndroidOrWebGL)
        {
            Debug.LogWarning("To test save MP4 file to Native. Put your MP4 file in the Assets/StreamingAssets folder and rename it to SampleMP4.mp4.");
            return;
        }

        string tmpText = hints + "(SaveMP4) Origin file path: " + filePath;
        debugText.text = tmpText;

        _LoadFile(filePath, (bytes) =>
        {
            if (bytes != null)
            {
                string savePath = MobileMedia.SaveBytes(bytes, "MobileMediaTest", filePathName.GetFileNameWithoutExt(), Path.GetExtension(filePath).ToLower(), false);
                debugText.text += "\n\nSave path: " + savePath;
            }
        });
    }

    private void _ShowLargePreviewImage(Texture2D texture)
    {
        base.SetRawImage(displayImage, texture);
        Resources.UnloadUnusedAssets();
    }


    #region ----- Others -----
    public void MoreAssetsAndDocuments()
    {
        Application.OpenURL("https://www.swanob2.com/assets");
    }

    public void TakeScreenshot(Action<Texture2D> onComplete)
    {
        StartCoroutine(_TakeScreenshot(onComplete));
    }

    private void _LoadFile(string url, Action<byte[]> onComplete)
    {
#if UNITY_2017_3_OR_NEWER
        StartCoroutine(filePathName.LoadFileUWR(url, (bytes) =>
        {
            onComplete(bytes);
        }));
#else
        StartCoroutine(filePathName.LoadFileWWW(url, (bytes) =>
        {
            onComplete(bytes);
        }));
#endif
    }

    private IEnumerator _LoadAudioFile(string url, Action<AudioClip> onComplete)
    {
        url = filePathName.EnsureValidPath(url);

#if UNITY_2017_3_OR_NEWER
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG)) // AudioType.MPEG: MP2, MP3
        {
            yield return uwr.SendWebRequest();
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                Debug.Log("File load error.\n" + uwr.error);
                if (onComplete != null) onComplete(null);
            }
            else
            {
                if (onComplete != null) onComplete(DownloadHandlerAudioClip.GetContent(uwr));
            }
        }
#else
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogError("File load error.\n" + www.error);
                if(onComplete != null) onComplete(null);
                yield break;
            }
            if (onComplete != null) onComplete(www.GetAudioClip());
        }
#endif
    }

    private void _PlayVideo_Handheld(string videoPath)
    {
        StartCoroutine(__PlayVideo_Handheld(videoPath));
    }
    private IEnumerator __PlayVideo_Handheld(string videoPath)
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.PlayFullScreenMovie(videoPath, Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFit);
#endif
        yield return null;
    }

    private IEnumerator _TakeScreenshot(Action<Texture2D> onComplete)
    {
        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        Texture2D readTex = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect rect = new Rect(0, 0, width, height);
        readTex.ReadPixels(rect, 0, 0);
        readTex.Apply();
        onComplete(readTex);
    }

    private Sprite _ToSprite(Texture2D texture)
    {
        if (texture == null) return null;

        Vector2 pivot = new Vector2(0.5f, 0.5f);
        float pixelPerUnit = 100;
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelPerUnit);
    }

    private bool IsAndroidOrWebGL
    {
        get
        {
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WebGLPlayer;
        }
    }
#endregion
}

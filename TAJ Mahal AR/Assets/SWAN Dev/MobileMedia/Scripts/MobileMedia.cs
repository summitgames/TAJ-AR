// Created by SwanDEV 2017

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public static class MobileMedia
{
	public enum ImageFormat
	{
		JPG = 0,
		PNG,
		GIF,
	}

	public enum Permission
	{
		Denied = 0,
		Granted,
		Ask,
	}

    public enum MediaType
    {
        Image = 0,
        Video = 1,

        Audio_Android = 2,
    }

#if UNITY_ANDROID
	private static AndroidJavaClass _androidPlugin = null;
	private static AndroidJavaClass androidPlugin
	{
		get
		{
			if(_androidPlugin == null)
			{
				_androidPlugin = new AndroidJavaClass("unity.swanob2.com.mobilemedia.MobileMedia");
			}
			return _androidPlugin;
		}
	}

	private static AndroidJavaObject _unityActivity = null;
	private static AndroidJavaObject unityActivity
	{
		get
		{
			if(_unityActivity == null)
			{
				using(AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					_unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				}
			}
			return _unityActivity;
		}
	}
#endif

#if UNITY_IOS
    [System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern int iCheckPermission();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern int iRequestPermission();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern int iCanOpenSettings();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void iOpenSettings();

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void iSaveImage(string path, string albumName);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void iSaveVideo(string path, string albumName);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void iPickImage(string temporaryImageSavePath, bool usePopup);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void iPickVideo(bool usePopup);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void iGetMediaPreviewPhoto(int mediaType, int mediaIndex, int targetSize, string temporaryImageSavePath);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void iGetMediaPhoto(int mediaType, int mediaIndex, string temporaryImageSavePath);
#endif


    #region ----- Permission -----
#if UNITY_ANDROID
	private static string PP_KEY_PERMISSION = "MobileMediaPlugin";
#endif

    public static Permission CheckPermission()
	{
		if(IsEditor) return Permission.Granted;

		#if UNITY_ANDROID
		Permission permission = (Permission) androidPlugin.CallStatic<int>("CheckPermission", unityActivity);
		if(permission == Permission.Denied && (Permission) PlayerPrefs.GetInt(PP_KEY_PERMISSION, (int) Permission.Ask) == Permission.Ask)
		{
			permission = Permission.Ask;
		}
		return permission;

		#elif UNITY_IOS
		return (Permission) iCheckPermission();
		#else
		return Permission.Granted;
		#endif
	}

	public static Permission RequestPermission()
	{
		if(IsEditor) return Permission.Granted;

		#if UNITY_ANDROID
		object threadLockObject = new object();
		lock(threadLockObject)
		{
			MMPermissionReceiver_Android mmReceiver = new MMPermissionReceiver_Android(threadLockObject);

			androidPlugin.CallStatic("RequestPermission", unityActivity, mmReceiver, PlayerPrefs.GetInt(PP_KEY_PERMISSION, (int) Permission.Ask));

			if(mmReceiver.Result == -1)
				System.Threading.Monitor.Wait(threadLockObject);

			if((Permission) mmReceiver.Result != Permission.Ask && PlayerPrefs.GetInt(PP_KEY_PERMISSION, -1) != mmReceiver.Result)
			{
				PlayerPrefs.SetInt(PP_KEY_PERMISSION, mmReceiver.Result);
				PlayerPrefs.Save();
			}

			return (Permission) mmReceiver.Result;
		}
		#elif UNITY_IOS
		return (Permission) iRequestPermission();
		#else
		return Permission.Granted;
		#endif
	}

	/// <summary>
	/// Check if this app can open the system settings menu for changing permission.
	/// </summary>
	/// <value><c>true</c> if can open settings; otherwise, <c>false</c>.</value>
	public static bool CanOpenSettings
	{
		get
		{
			#if UNITY_IOS
			if(!IsEditor)
			{
				return iCanOpenSettings() == 1;
			}
			else
			{
				return true;
			}
			#else
			return true;
			#endif
		}
	}

	/// <summary>
	/// Open the system settings menu for this app for changing permission.
	/// </summary>
	public static void OpenSettings()
	{
		if(IsEditor) return;

		#if UNITY_ANDROID
		androidPlugin.CallStatic("OpenSettings", unityActivity);

		#elif UNITY_IOS
		if(!CanOpenSettings) return;
		iOpenSettings();

		#endif
	}

    #endregion


    #region ----- Save Media to Gallery -----
    /// <summary>
    /// Save the byte array of a media as a file to the gallery.
    /// </summary>
    /// <param name="mediaBytes">The media byte array.</param>
    /// <param name="folderName">The target folder name to save the media file.</param>
    /// <param name="fileName">The media file name (with out extension name).</param>
    /// <param name="extensionName">The extension name of the media file (eg.: .jpg, .png, .gif, .mp3, .mp4, etc.).</param>
    /// <param name="mediaType">Types: Image(e.g. jpg, png, gif), Video(e.g. mp4), or Audio(e.g. mp3) </param>
    public static string SaveBytes(byte[] mediaBytes, string folderName, string fileName, string extensionName, MediaType mediaType)
	{
		string savePath = "";
		if(RequestPermission() == Permission.Granted)
		{
			if(mediaBytes == null || mediaBytes.Length == 0)
				throw new ArgumentException("mediaBytes is null or empty!");

			if(string.IsNullOrEmpty(folderName) || folderName.Length == 0)
				throw new ArgumentException("folderName is null or empty!");

			if(string.IsNullOrEmpty(fileName) || fileName.Length == 0)
				throw new ArgumentException("fileName is null or empty!");

			if(string.IsNullOrEmpty(extensionName) || extensionName.Length == 0)
				throw new ArgumentException("extensionName is null or empty!");

			string path = _GetSavePath(folderName, fileName + extensionName, mediaType == MediaType.Audio_Android);
			savePath = path;

			File.WriteAllBytes(path, mediaBytes);
			mediaBytes = null;

			_SaveInternal(path, folderName, mediaType);
		}
		return savePath;
	}
    /// <summary>
    /// Save the byte array of a media as a file to the gallery.
    /// </summary>
    /// <param name="mediaBytes">The media byte array.</param>
    /// <param name="folderName">The target folder name to save the media file.</param>
    /// <param name="fileName">The media file name (with out extension name).</param>
    /// <param name="extensionName">The extension name of the media file (eg.: .jpg, .png, .gif, .mp3, .mp4, etc.).</param>
    /// <param name="isImage">If set to <c>true</c> is image, else video.</param>
    public static string SaveBytes(byte[] mediaBytes, string folderName, string fileName, string extensionName, bool isImage)
    {
        return SaveBytes(mediaBytes, folderName, fileName, extensionName, isImage ? MediaType.Image : MediaType.Video);
    }

    /// <summary>
    /// Copy an existing media file to the gallery.
    /// </summary>
    /// <param name="existingMediaPath">The path of the existing media file.</param>
    /// <param name="folderName">The target folder name to save the media file.</param>
    /// <param name="fileName">The media file name (with out extension name).</param>
    /// <param name="extensionName">The extension name of the media file (eg.: .jpg, .png, .gif, .mp3, .mp4, etc.).</param>
    /// <param name="mediaType">Types: Image(e.g. jpg, png, gif), Video(e.g. mp4), or Audio(e.g. mp3) </param>
    public static string CopyMedia(string existingMediaPath, string folderName, string fileName, string extensionName, MediaType mediaType)
    {
        string savePath = "";
        if (RequestPermission() == Permission.Granted)
        {
            if (!File.Exists(existingMediaPath))
                throw new FileNotFoundException("File not found at " + existingMediaPath);

            if (string.IsNullOrEmpty(folderName) || folderName.Length == 0)
                throw new ArgumentException("folderName is null or empty!");

            if (string.IsNullOrEmpty(fileName) || fileName.Length == 0)
                throw new ArgumentException("fileName is null or empty!");

            if (string.IsNullOrEmpty(extensionName) || extensionName.Length == 0)
                throw new ArgumentException("extensionName is null or empty!");

            string path = _GetSavePath(folderName, fileName + extensionName, mediaType == MediaType.Audio_Android);
            savePath = path;

            File.Copy(existingMediaPath, path, true);

            _SaveInternal(path, folderName, mediaType);
        }
        return savePath;
    }
    /// <summary>
    /// Copy an existing image or video file to the gallery.
    /// </summary>
    /// <param name="existingMediaPath">The path of the existing media file.</param>
    /// <param name="folderName">The target folder name to save the media file.</param>
    /// <param name="fileName">The media file name (with out extension name).</param>
    /// <param name="extensionName">The extension name of the media file (eg.: .jpg, .png, .gif, .mp3, .mp4, etc.).</param>
    /// <param name="isImage">If set to <c>true</c> is image, else video.</param>
    public static string CopyMedia(string existingMediaPath, string folderName, string fileName, string extensionName, bool isImage)
    {
        return CopyMedia(existingMediaPath, folderName, fileName, extensionName, isImage ? MediaType.Image : MediaType.Video);
    }

    /// <summary>
    /// Save a Texture2D as JPG/PNG/GIF to gallery.
    /// </summary>
    /// <param name="texture2d">Image.</param>
    /// <param name="folderName">Folder name.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="imageFormat">Image format: JPG/PNG/GIF.</param>
    /// <param name="quality">The quality of JPG and GIF. 1..100, 1=lowest, 100=highest.</param>
    public static string SaveImage(Texture2D texture2d, string folderName, string fileName, ImageFormat imageFormat = ImageFormat.JPG, int quality = 90)
	{
		if(texture2d == null) throw new ArgumentException("image is null!");

		quality = Mathf.Clamp(quality, 1, 100);
		string savePath = "";
		byte[] imgBytes = null;
		switch(imageFormat)
		{
		case ImageFormat.JPG:
			imgBytes = texture2d.EncodeToJPG(quality);
			savePath = SaveBytes(imgBytes, folderName, fileName, ".jpg", true);
			imgBytes = null;
			break;
		case ImageFormat.PNG:
			imgBytes = texture2d.EncodeToPNG();
			savePath = SaveBytes(imgBytes, folderName, fileName, ".png", true);
			imgBytes = null;
			break;
		case ImageFormat.GIF:
		#if PRO_GIF
			int gifQuality = 101 - quality; //1: high quality .... 100: low quality
			savePath = ProGifTexturesToGIF.Instance.Save(new List<Texture2D>{texture2d}, texture2d.width, texture2d.height, 1, -1, gifQuality,
				(workerId, gifPath)=>{
					CopyMedia(gifPath, folderName, fileName, ".gif", true);
				}
			);
		#endif
			break;
		}
		return savePath;
    }

    /// <summary>
    /// Save video to gallery.
    /// </summary>
    public static string SaveVideo(byte[] mediaBytes, string folderName, string fileName, string extensionName)
    {
        string savePath = SaveBytes(mediaBytes, folderName, fileName, extensionName, false);
        return savePath;
    }

    /// <summary>
    /// Save audio to Music folder (Android).
    /// </summary>
    public static string SaveAudioAndroid(byte[] mediaBytes, string folderName, string fileName, string extensionName)
    {
        string savePath = SaveBytes(mediaBytes, folderName, fileName, extensionName, MediaType.Audio_Android);
        return savePath;
    }

    /// <summary>
    /// Save to Android/iOS gallery.
    /// </summary>
    /// <param name="path">Complete file path.</param>
    /// <param name="iOSAlbumName">Album name (for iOS only).</param>
    /// <param name="mediaType">If set to <c>true</c> is image.</param>
    private static void _SaveInternal(string path, string iOSAlbumName, MediaType mediaType)
	{
		if(IsEditor) return;

		#if UNITY_ANDROID
		androidPlugin.CallStatic("MediaScanFile", unityActivity, path);

		#elif UNITY_IOS
		if(mediaType == MediaType.Image)
		{
			iSaveImage(path, iOSAlbumName);
		}
		else if(mediaType == MediaType.Video)
		{
			iSaveVideo(path, iOSAlbumName);
		}
		#endif
	}

	private static string _GetSavePath(string folderName, string filenameWithExtension, bool isAudio_Android)
	{
		string savePath;

#if UNITY_ANDROID
		if(!IsEditor)
		{
            // e.g. DCIM: /storage/sdcard0/DCIM (reminded that the Android path may vary on different devices)
            string androidGetMediaType = isAudio_Android ? "AUDIO" : "DCIM";
			savePath = androidPlugin.CallStatic<string>("GetMediaPath", folderName, androidGetMediaType);

            // Customize the parent directory by modifying the save path:
            //savePath = Path.Combine(Directory.GetParent(savePath).FullName, folderName);
            //if (!Directory.Exists(savePath))
            //{
            //    Directory.CreateDirectory(savePath);
            //}
        }
        else
		{
			savePath = Path.Combine(Application.persistentDataPath, folderName);
			if(!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
		}
#else
        savePath = Path.Combine(Application.persistentDataPath, folderName);
		if(!Directory.Exists(savePath))
		{
			Directory.CreateDirectory(savePath);
		}
		#endif
		
		if(filenameWithExtension.Contains("{0}"))
		{
			int fileIndex = 0;
			string path;
			do
			{
				path = Path.Combine(savePath, string.Format(filenameWithExtension, ++fileIndex));
			} while(File.Exists(path));

			return path;
		}

		savePath = Path.Combine(savePath, filenameWithExtension);

		#if UNITY_IOS
		if(!IsEditor)
		{
			// The file should not be overwritten on iOS. We should always ensure an unique filename.
			if(File.Exists(savePath))
			{
				return _GetSavePath(folderName, Path.GetFileNameWithoutExtension(filenameWithExtension) + " {0}" + Path.GetExtension(filenameWithExtension), isAudio_Android);
			}
		}
		#endif

		return savePath;
	}

    #endregion


    #region ----- Media Picker -----
    /// <summary>
    /// If 'true', use the new content picker for picking Image and Video on Android, else use the old one.
    /// The new content picker provides more options for picking content and allows picking content on the cloud drives.
    /// </summary>
    public static bool Android_NewContentPicker = false;

    /// <summary>
    /// If 'true', only allows the Android new content picker to pick local content only, else also show the cloud drives if available.
    /// </summary>
    public static bool Android_NewContentPicker_LocalOnly = true;

    public static bool IsMediaPickerBusy
	{
		get
		{
			#if UNITY_IOS
			if(!IsEditor)
			{
				return MMPickerReceiver_iOS.IsBusy;
			}
			else
			{
				return false;
			}
			#else
			return false;
			#endif
		}
	}

	public static void PickImage(Action<string> onReceived, string title = "", string androidMimeType = "image/*", bool iOS_UsePopup = false, string iOS_TempFileNameWithoutExtension = "temp")
	{
		PickMediaSingle(onReceived, MediaType.Image, androidMimeType, title, iOS_UsePopup, iOS_TempFileNameWithoutExtension);
	}

	public static void PickVideo(Action<string> onReceived, string title = "", string androidMimeType = "video/*", bool iOS_UsePopup = false)
	{
		PickMediaSingle(onReceived, MediaType.Video, androidMimeType, title, iOS_UsePopup);
	}

    public static void PickAudioAndroid(Action<string> onReceived, string title = "", string androidMimeType = "audio/*", bool iOS_UsePopup = false)
    {
        PickMediaSingle(onReceived, MediaType.Audio_Android, androidMimeType, title, iOS_UsePopup);
    }

    public static void PickImageIOS(Action<string> onReceived, bool iOS_UsePopup = false, string iOS_TempFileNameWithoutExtension = "temp")
	{
		PickMediaSingle(onReceived, MediaType.Image, "", "", iOS_UsePopup, iOS_TempFileNameWithoutExtension);
	}

	public static void PickVideoIOS(Action<string> onReceived, bool iOS_UsePopup)
	{
		PickMediaSingle(onReceived, MediaType.Video, "", "", iOS_UsePopup);
	}

	/// <summary>
	/// Pick a media file from gallery (Android & iOS).
	/// </summary>
	public static void PickMediaSingle(Action<string> onReceived, MediaType mediaType, string androidMimeType, string title, bool iOS_UsePopup = false, string iOS_TempFileNameWithoutExtension = "temp")
	{
		if(RequestPermission() == Permission.Granted && !IsMediaPickerBusy)
		{
			if(IsEditor)
			{
				if(onReceived != null) onReceived(null);
				return;
			}

			#if UNITY_ANDROID
			object threadLockObject = new object();
			lock(threadLockObject)
			{
				MMPickerReceiver_Android mmReceiver = new MMPickerReceiver_Android(threadLockObject);

                //androidPlugin.CallStatic("PickMedia", unityActivity, mmReceiver, (int)mediaType, false, androidMimeType, title);   // <= v1.1.3 aar
                androidPlugin.CallStatic("PickMedia", unityActivity, mmReceiver, (int)mediaType, false, androidMimeType, title, Android_NewContentPicker, Android_NewContentPicker_LocalOnly); // >= v1.1.4 aar

                if (string.IsNullOrEmpty(mmReceiver.Path))
				{
					System.Threading.Monitor.Wait(threadLockObject);
				}

				string path = mmReceiver.Path;
				if(string.IsNullOrEmpty(path))
				{
					path = null;
				}

				if(onReceived != null)
				{
					onReceived(path);
				}
			}
			#elif UNITY_IOS
			MMPickerReceiver_iOS.Initialize(onReceived);
			if(mediaType == MediaType.Image)
			{
				iPickImage(Path.Combine(Application.temporaryCachePath, iOS_TempFileNameWithoutExtension), iOS_UsePopup);
			}
			else if(mediaType == MediaType.Video)
			{
				iPickVideo(iOS_UsePopup);
			}
			#endif
		}
		else if(onReceived != null)
		{
			onReceived(null);
		}
	}

    /// <summary>
    /// Gets the media (Image/Video/GIF) thumbnail image (Android, iOS).
    /// </summary>
    /// <param name="onReceived">The callback for receiving the file path.</param>
    /// <param name="mediaType">0: Photo, 1: Video, 2: Photo or Video.</param>
    /// <param name="mediaIndex">Equal/less than -1: get oldest, 0: get most recent(default), other values will be clamped between 0 and the total item count.</param>
    /// <param name="targetSize">The target size to generate the image.</param>
    /// <param name="Android_TargetNativeFolderName">The target native folder to get file from (For Android).</param>
    public static void GetMediaThumbnail(Action<string> onReceived, int mediaType, int mediaIndex = 0, int targetSize = 96, string Android_TargetNativeFolderName = "Camera")
    {
        if (RequestPermission() == Permission.Granted && !IsMediaPickerBusy)
        {
            if (IsEditor)
            {
                if (onReceived != null) onReceived(null);
                return;
            }
#if UNITY_ANDROID
            string imagePath = androidPlugin.CallStatic<string>("GetMediaImage", mediaType, mediaIndex, targetSize, Android_TargetNativeFolderName, "DCIM", Path.Combine(Application.temporaryCachePath, "MMP_Temp.png"), false);
            if (onReceived != null) onReceived(imagePath);
#elif UNITY_IOS
            GetMediaThumbnail_IOS(onReceived, mediaType, mediaIndex, targetSize, "MMP_Temp");
#endif
        }
        else if (onReceived != null)
        {
            onReceived(null);
        }
    }

    /// <summary>
    /// Gets the media (Image/Video/GIF) full size image (Android, iOS).
    /// </summary>
    /// <param name="onReceived">The callback for receiving the file path.</param>
    /// <param name="mediaType">0: Photo, 1: Video, 2: Photo or Video.</param>
    /// <param name="mediaIndex">Equal/less than -1: get oldest, 0: get most recent(default), other values will be clamped between 0 and the total item count.</param>
    /// <param name="Android_TargetNativeFolderName">The target native folder to get file from (For Android).</param>
    public static void GetMediaPhoto(Action<string> onReceived, int mediaType, int mediaIndex = 0, string Android_TargetNativeFolderName = "Camera")
    {
        if (RequestPermission() == Permission.Granted && !IsMediaPickerBusy)
        {
            if (IsEditor)
            {
                if (onReceived != null) onReceived(null);
                return;
            }
#if UNITY_ANDROID
            string imagePath = androidPlugin.CallStatic<string>("GetMediaImage", mediaType, mediaIndex, 0, Android_TargetNativeFolderName, "DCIM", Path.Combine(Application.temporaryCachePath, "MMP_Temp.png"), true);
            if (onReceived != null) onReceived(imagePath);
#elif UNITY_IOS
            GetMediaPhoto_IOS(onReceived, mediaType, mediaIndex, "MMP_Temp");
#endif
        }
        else if (onReceived != null)
        {
            onReceived(null);
        }
    }

    /// <summary>
    /// Gets the media (Image/Video/GIF) thumbnail image (iOS).
    /// </summary>
    /// <param name="onReceived">The callback for receiving the file path.</param>
    /// <param name="mediaType">0: Photo, 1: Video, 2: Photo or Video.</param>
    /// <param name="mediaIndex">Equal/less than -1: get oldest, 0: get most recent(default), other values will be clamped between 0 and the total item count.</param>
    /// <param name="targetSize">The target size to generate the image.</param>
    /// <param name="iOS_TempFileNameWithoutExtension">Temporary path for storing the preview photo.</param>
    public static void GetMediaThumbnail_IOS(Action<string> onReceived, int mediaType, int mediaIndex = 0, int targetSize = 100, string iOS_TempFileNameWithoutExtension = "temp")
	{
		if(RequestPermission() == Permission.Granted && !IsMediaPickerBusy)
		{
			if(IsEditor)
			{
				if(onReceived != null) onReceived(null);
				return;
			}
#if UNITY_IOS
			MMPickerReceiver_iOS.Initialize(onReceived);
			iGetMediaPreviewPhoto(mediaType, mediaIndex, targetSize, Path.Combine(Application.temporaryCachePath, iOS_TempFileNameWithoutExtension));
#else
			if(onReceived != null) onReceived(null);
#endif
		}
		else if(onReceived != null)
		{
			onReceived(null);
		}
	}

    /// <summary>
    /// Gets the media (Image/Video/GIF) full size image (iOS).
    /// </summary>
    /// <param name="onReceived">The callback for receiving the file path.</param>
    /// <param name="mediaType">0: Photo, 1: Video, 2: Photo or Video.</param>
    /// <param name="mediaIndex">Equal/less than -1: get oldest, 0: get most recent(default), other values will be clamped between 0 and the total item count.</param>
    /// <param name="iOS_TempFileNameWithoutExtension">Temporary path for storing the preview photo.</param>
    public static void GetMediaPhoto_IOS(Action<string> onReceived, int mediaType, int mediaIndex = 0, string iOS_TempFileNameWithoutExtension = "temp")
    {
        if (RequestPermission() == Permission.Granted && !IsMediaPickerBusy)
        {
            if (IsEditor)
            {
                if (onReceived != null) onReceived(null);
                return;
            }
#if UNITY_IOS
            MMPickerReceiver_iOS.Initialize(onReceived);
            iGetMediaPhoto(mediaType, mediaIndex, Path.Combine(Application.temporaryCachePath, iOS_TempFileNameWithoutExtension));
#else
            if(onReceived != null) onReceived(null);
#endif
        }
        else if (onReceived != null)
        {
            onReceived(null);
        }
    }

#endregion

    private static bool IsEditor
	{
		get{
			return Application.platform == RuntimePlatform.LinuxEditor ||
			Application.platform == RuntimePlatform.OSXEditor ||
			Application.platform == RuntimePlatform.WindowsEditor;
		}
	}
}

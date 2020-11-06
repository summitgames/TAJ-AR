// Created by SwanDEV 2018

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// On editor GIF recorder: record GIF of your app's development screens.
/// [ HOW To USE ] Attach the OnEditorGifRecorder script on a GameObject in the scene, or drag the OnEditorGifRecorder prefab from OnEditor folder to the scene.
/// </summary>
public class OnEditorGifRecorder : MonoBehaviour
{
	[Space()]
	[Header("[ Recorder Settings ]")]
	[Tooltip("If 'True', automatically save the recorder when Record Progress is 100%.")]
	public bool m_AutoSave = false;

	[Space()]
	public Vector2 m_AspectRatio = new Vector2(0, 0);
	public bool m_AutoAspect = true;
	public int m_Width = 360;
	public int m_Height = 360;
	[Tooltip("The target recording time in seconds.")]
	public float m_Duration = 3f;
	[Tooltip("The GIF target framerate (frames per second).")]
	[Range(1, 60)] public int m_Fps = 15;
	[Tooltip("Loop time for the GIF. -1: no repeat, 0: loop forever, or set any number greater than 0.")]
	public int m_Loop = 0;								//-1: no repeat, 0: infinite, >0: repeat count
	[Tooltip("1 is the best quality but slower, set 15 - 30 has a good balance.")]
	[Range(1, 100)] public int m_Quality = 20;			//(1 - 100), 1: best(larger storage size), 100: faster(smaller storage size)

	[Space()]
	[Header("[ Camera Settings ]")]
	[Tooltip("The camera for recording GIF. Drag camera on this variable or click the 'Find Camera' button to setup.")]
	public Camera m_RecorderCamera;
	public Camera[] m_AllCameras;
	private int _currCameraIndex = 0;
	private const string _recorderName = "OnEditorGifRecorder";

	[HideInInspector] public string m_RecordingProgress = "0%";
	[HideInInspector] public string m_SaveProgress = "0%";
	[HideInInspector] public string m_State = "Idle";
	[HideInInspector] [TextArea(1, 2)] public string m_SavePath = "GIF Path";


	public void FindCameras(OnEditorGifRecorderCustomEditor editorScript)
	{
		m_AllCameras = Camera.allCameras;
		editorScript.SetCameraOptions(m_AllCameras);

		if(m_AllCameras != null && m_AllCameras.Length > 0 && m_RecorderCamera == null)
		{
			m_RecorderCamera = m_AllCameras[0];
		}
	}

	public void SetCamera(int index)
	{
		if(_currCameraIndex == index) return;
		_currCameraIndex = index;
		if(index < m_AllCameras.Length) m_RecorderCamera = m_AllCameras[index];
	}

	public void StartRecord()
	{
		if(!Application.isPlaying || !Application.isEditor)
		{
			Debug.LogWarning("This script is designed to work in the Editor Mode with Editor Playing.");
			return;
		}

		//Debug.Log("Start Record");
		m_State = "Recording..";
		PGif.iSetRecordSettings(m_AutoAspect, m_Width, m_Height, m_Duration, m_Fps, m_Loop, m_Quality);
		PGif.iStartRecord(((m_RecorderCamera == null)?Camera.main:m_RecorderCamera), _recorderName, 
			(progress)=>{
				m_RecordingProgress = Mathf.CeilToInt(progress*100) + "%";
			},
			()=>{
				m_State = "Press the <Save Record> button to save GIF";
				if(m_AutoSave)
				{
					SaveRecord();
				}
			},
			null,
			(id, progress)=>{
				m_SaveProgress = Mathf.CeilToInt(progress*100) + "%";
			},
			(id, path)=>{
				m_SavePath = path;
				m_RecordingProgress = "0%";
				m_SaveProgress = "0%";
				m_State = "Idle";
			}
		);
	}

	public void SaveRecord()
	{
		if(!Application.isPlaying || !Application.isEditor)
		{
			Debug.LogWarning("This script is designed to work in the Editor Mode with Editor Playing.");
			return;
		}

        //Debug.Log("Save Record");
        if (State != ProGifRecorder.RecorderState.Stopped)
        {
            m_State = "Saving..";
            PGif.iStopAndSaveRecord(_recorderName);
        }
    }

    public void PauseRecord()
    {
        if (!Application.isPlaying || !Application.isEditor)
        {
            Debug.LogWarning("This script is designed to work in the Editor Mode with Editor Playing.");
            return;
        }

        //Debug.Log("Pause Record");
        m_State = "Paused";
        PGif.iPauseRecord(_recorderName);
    }

    public void ResumeRecord()
    {
        if (!Application.isPlaying || !Application.isEditor)
        {
            Debug.LogWarning("This script is designed to work in the Editor Mode with Editor Playing.");
            return;
        }

        //Debug.Log("Resume Record");
        m_State = "Recording..";
        PGif.iResumeRecord(_recorderName);
    }

    public void CancelRecord()
    {
        if (!Application.isPlaying || !Application.isEditor)
        {
            Debug.LogWarning("This script is designed to work in the Editor Mode with Editor Playing.");
            return;
        }

        //Debug.Log("Cancel Record");
        m_RecordingProgress = "0%";
        m_SaveProgress = "0%";
        m_State = "Idle";
        PGif.iStopRecord(_recorderName);
        PGif.iClearRecorder(_recorderName);
    }

    public ProGifRecorder.RecorderState State
    {
        get
        {
            if (!PGif.HasInstance || PGif.iGetRecorder(_recorderName) == null) return ProGifRecorder.RecorderState.Stopped;
            return PGif.iGetRecorder(_recorderName).State;
        }
    }

}


[CustomEditor(typeof(OnEditorGifRecorder))]
public class OnEditorGifRecorderCustomEditor : Editor
{
	private static string[] cameraOptions = new string[]{}; 
	private static int cameraSelection = 0;

    private static bool showHelpsMessage = false;
    private string helpsMessage = "[ HOW ] Attach this script on a GameObject in the scene, modify the recorder settings and start record GIF in the Editor."
        + "\n\n---- Steps ----"
        + "\n(1) Run your scene, click the 'Find Camera' button and set camera."
        + "\n(2) Click the 'Start Record' button to start the recorder."
        + "\n(3) Click the 'Save Record' button to save the recorded frames as GIF."
        + "\n(4) Wait until the 'Save Progress' become 100%.";

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
        bool isLightSkin = !EditorGUIUtility.isProSkin;

        //GUI.backgroundColor = isLightSkin ? new Color(0.8f, 0.7f, 0.2f, 1f) : Color.yellow; 

		OnEditorGifRecorder recorder = (OnEditorGifRecorder)target;
        
        cameraSelection = GUILayout.SelectionGrid(cameraSelection, cameraOptions, 2);
		recorder.SetCamera(cameraSelection);

		GUILayout.Label("Find all Cameras in the scene:\n(Drag the camera you want to Recorder Camera)");
		if(GUILayout.Button("Find Cameras"))
		{
			recorder.FindCameras(this);
		}

        if (recorder.State == ProGifRecorder.RecorderState.Stopped)
        {
            GUILayout.Label("\n\n[ Start Record GIF ]\nStart record GIF with Recorder Camera, or main camera:");
            if (GUILayout.Button("Start Record"))
            {
                recorder.StartRecord();
            }
        }
        else
        {
            GUILayout.Label("\n\n[ Recorder Started ]\nControl & Save the recorder using below buttons:");
            EditorGUILayout.BeginHorizontal();
            if (recorder.State == ProGifRecorder.RecorderState.Recording)
            {
                if (GUILayout.Button("Pause Record")) recorder.PauseRecord();
            }
            else
            {
                if (GUILayout.Button("Resume Record")) recorder.ResumeRecord();
            }

            if (GUILayout.Button("Cancel Record")) recorder.CancelRecord();
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label("Stop and save the stored frames as GIF:");
        if (GUILayout.Button("Save Record"))
        {
            recorder.SaveRecord();
        }

		GUILayout.Label("\nRecord Progress: " + recorder.m_RecordingProgress
			+ "\nSave Progress: " + recorder.m_SaveProgress
			+ "\nStatus: " + recorder.m_State
			+ "\n\nGIF Path: " + recorder.m_SavePath + "\n");

		if(GUILayout.Button("View GIF"))
		{
			if(string.IsNullOrEmpty(recorder.m_SavePath)) return;
			_OpenURL(new FilePathName().EnsureLocalPath(recorder.m_SavePath));
		}

		if(GUILayout.Button("Reveal In Folder"))
		{
			if(string.IsNullOrEmpty(recorder.m_SavePath)) return;
			string fileName = Path.GetFileName(recorder.m_SavePath);
			string directoryPath = recorder.m_SavePath.Remove(recorder.m_SavePath.IndexOf(fileName));
			_OpenURL(new FilePathName().EnsureLocalPath(directoryPath));
		}

		if(GUILayout.Button("Copy GIF Path"))
		{
			if(string.IsNullOrEmpty(recorder.m_SavePath)) return;

			TextEditor te = null;
			te = new TextEditor();
			te.text = recorder.m_SavePath;
			te.SelectAll();
			te.Copy();
		}

        GUILayout.Space(10);

        Color tipTextColor = isLightSkin ? new Color(0.12f, 0.12f, 0.12f, 1f) : Color.cyan;
        GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.textArea);
        helpBoxStyle.normal.textColor = tipTextColor;

        GUIStyle tipsStyle = new GUIStyle(EditorStyles.boldLabel);
        tipsStyle.normal.textColor = tipTextColor;

        showHelpsMessage = GUILayout.Toggle(showHelpsMessage, " Help (How To Use? Click here...)", tipsStyle);
        if (showHelpsMessage) GUILayout.Label(helpsMessage, helpBoxStyle);

    }

    private void _OpenURL(string url)
    {
#if UNITY_EDITOR_OSX
        System.Diagnostics.Process.Start(url);
#else
        Application.OpenURL(url);
#endif
    }

    public void SetCameraOptions(Camera[] cameras)
	{
		cameraSelection = 0;
		cameraOptions = new string[cameras.Length];
		for(int i=0; i<cameras.Length; i++)
		{
			cameraOptions[i] = cameras[i].name;
		}
	}
}
#endif
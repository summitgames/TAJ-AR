using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;

namespace TajAR
{

	public class StorieManager : MonoBehaviour
	{
		public int CurrentPlayStoryIs = 0;
		public GameObject[] allStoryCont;
		public ScrollRect StoryScrollRect;
		public MediaPlayer videoPlayer;
		public string[] videoName;


		void Start()
		{

		}

		void Update()
		{

		}

		public void StoryPlay(int no)
		{
			StoryScrollRect.content = allStoryCont[no].GetComponent<RectTransform>();
			gameObject.SetActive(true);
			CurrentPlayStoryIs = no;
			videoPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoName[CurrentPlayStoryIs], true);
		}

		public void AnimFadeInClick()
		{
			allStoryCont[CurrentPlayStoryIs].SetActive(true);
			//videoPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoName[CurrentPlayStoryIs], true);
		}
		public void AnimFadeOutClick()
		{
			allStoryCont[CurrentPlayStoryIs].SetActive(false);
		}

		public void CloseBT()
		{
			GetComponent<Animation>().Play("StoryScreenFadeOutAnim");
			videoPlayer.Control.Pause();
		}
		public void CloseCurrentObj()
		{
			gameObject.SetActive(false);
		}
	}
}
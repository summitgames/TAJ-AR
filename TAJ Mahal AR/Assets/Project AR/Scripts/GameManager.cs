using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TajAR
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager inst;
		public static bool isAllCalculationDone;
		public GameObject openingScreen, detailScreen, swipeScreen, loadingScreen, introScreen, contentObj, footerObj, dustParticle;
		public ScrollRect SwipeScreenScroll;
		public RectTransform[] allContent, swipeScreenRect, allExperinceObj;
		public bool waitForNextAnim = true;
		public int currentSwipeObj = 0;
		//	public Button[] ExperienceBt;
		public static float ContentHeight, contentVal, ExpectedPos, differentVal, imageWidth, imageHeight, fullScreenValue, fourExpDisplayVal;
		public static float[] ScrollPosSave = new float[8];
		public static Vector3 ContentStartPosSave;


		void Awake()
		{

			inst = this;
		}

		void Start()
		{

			//if (SingletonController.instance.ifExperiencePlay)
			//{
			//	BackToMainScreenAfterExperiencePlay();
			//}
			//else
			//{
			//	//loadingScreen.GetComponent<Animation>().Play("LoadingFadeOut");
			//}

			//

			if (!isAllCalculationDone)
			{
				float ScreenResolution = (float)Screen.height / (float)Screen.width;

				if (ScreenResolution.ToString("F2") == "1.33")
				{
					imageWidth = 1390;
					imageHeight = 1650;
					//1436
				}
				else if (ScreenResolution.ToString("F2") != "1.78")
				{
					if (ScreenResolution <= 1.78)
					{
						imageWidth = 1050;
						imageHeight = 1650;
						//1100
					}
					else
					{
						imageWidth = 1030;
						float hig = (1650 * ScreenResolution) / 1.8f;
						imageHeight = hig + 80;

						float ScreenHeight = (1920 * ScreenResolution) / 1.8f;
						fullScreenValue = ScreenHeight + 80;
					}

				}
				else if (ScreenResolution.ToString("F2") == "1.78")
				{
					imageHeight = 1650;
					imageWidth = 1030;
					fullScreenValue = 1920;
				}
				fourExpDisplayVal = (imageHeight - 50) / 4;
				footerObj.GetComponent<RectTransform>().sizeDelta = new Vector2(imageWidth, 216.3f);

				for (int i = 0; i < swipeScreenRect.Length; i++)
				{
					swipeScreenRect[i].GetComponent<SwipeImageScale>().ObjectInit();
					allExperinceObj[i].GetComponent<ImageScaleManager>().ObjectInit();
				}

				StartCoroutine("AllExperienceSetInBackend");
			}
			else
			{
				fourExpDisplayVal = (imageHeight - 50) / 4;
				footerObj.GetComponent<RectTransform>().sizeDelta = new Vector2(imageWidth, 216.3f);

				for (int i = 0; i < swipeScreenRect.Length; i++)
				{
					swipeScreenRect[i].GetComponent<SwipeImageScale>().ObjectInit();
					allExperinceObj[i].GetComponent<ImageScaleManager>().ObjectInit();
					allExperinceObj[i].GetComponent<ImageScaleManager>().ScrollPos = ScrollPosSave[i];
				}
				if (SingletonController.instance.ifExperiencePlay)
				{
					BackToMainScreenAfterExperiencePlay();
				}
			}
		}

		IEnumerator AllExperienceSetInBackend()
		{
			yield return new WaitForSeconds(1f);
			contentObj.transform.localPosition = new Vector2(0, -5000);
			contentObj.GetComponent<VerticalLayoutGroup>().enabled = false;
			contentObj.GetComponent<ContentSizeFitter>().enabled = false;

			ContentHeight = contentObj.GetComponent<RectTransform>().sizeDelta.y;


			if (ContentHeight > 3370.396)
			{
				contentVal = ((ContentHeight + 152) + (imageHeight - fourExpDisplayVal));
				ExpectedPos = (-1547.6f * contentVal) / 4772.4f;
				differentVal = (440 * ContentHeight) / 3370.39f;
				allExperinceObj[0].GetComponent<ImageScaleManager>().ScrollPos = ExpectedPos;
				ScrollPosSave[0] = ExpectedPos;
				for (int i = 1; i < allExperinceObj.Length; i++)
				{
					if (i >= 3)
					{
						allExperinceObj[i].GetComponent<ImageScaleManager>().ScrollPos = (differentVal * i) + (ExpectedPos - 20);
						ScrollPosSave[i] = (differentVal * i) + (ExpectedPos - 20);
					}
					else
					{
						allExperinceObj[i].GetComponent<ImageScaleManager>().ScrollPos = (differentVal * i) + ExpectedPos;
						ScrollPosSave[i] = (differentVal * i) + ExpectedPos;
					}

				}
			}
			else
			{
				for (int i = 0; i < swipeScreenRect.Length; i++)
				{
					ScrollPosSave[i] = allExperinceObj[i].GetComponent<ImageScaleManager>().ScrollPos;
				}
			}


			yield return new WaitForSeconds(0.5f);
			openingScreen.SetActive(false);
			isAllCalculationDone = true;
		}

		public void BackToMainScreenAfterExperiencePlay()
		{
			allContent[SingletonController.instance.currentExperiencePlay].gameObject.SetActive(true);
			SwipeScreenScroll.content = allContent[SingletonController.instance.currentExperiencePlay];
			swipeScreen.GetComponent<Image>().enabled = true;


			allExperinceObj[SingletonController.instance.currentExperiencePlay].GetComponent<ImageScaleManager>().ExperienceBTClick();
			SingletonController.instance.ifExperiencePlay = false;
			SingletonController.instance.currentExperiencePlay = 0;
			introScreen.SetActive(false);
			openingScreen.SetActive(true);
			dustParticle.SetActive(false);
		}

		void Update()
		{

		}

		public void PlayExperience(int no)
		{
			SingletonController.instance.ifExperiencePlay = true;
			SingletonController.instance.currentExperiencePlay = no;
			introScreen.SetActive(false);
			loadingScreen.GetComponent<Animation>().Play("LoadingFadeIn");


		}
		public void SceneChange(int no)
		{
			switch (no)
			{
				case 0: // 360 Image play
					Application.LoadLevel("360Image");
					break;
				case 1: // 360 video play
					Application.LoadLevel("360Video");
					break;
				case 2: // View in AR
					Application.LoadLevel("ViewInAR");
					break;
			}
		}

		public void DetailScreenOpenClose(bool status)
		{
			if (status)
			{
				detailScreen.SetActive(true);
			}
			else
			{
				detailScreen.SetActive(false);
			}
		}
		public void ExploreBTClick()
		{
			dustParticle.SetActive(false);
			openingScreen.SetActive(true);
			openingScreen.transform.SetSiblingIndex(1);
			ContentStartPosSave = contentObj.transform.localPosition;
		}

		public void SwipeUPBT()
		{
			openingScreen.GetComponent<Swipe>().enabled = false;
			allContent[currentSwipeObj].gameObject.SetActive(true);
			SwipeScreenScroll.content = allContent[currentSwipeObj];
			swipeScreen.GetComponent<Image>().enabled = true;
		}

		public void SwipeDown()
		{
			openingScreen.GetComponent<Swipe>().enabled = true;
			allContent[currentSwipeObj].gameObject.SetActive(false);
			swipeScreen.GetComponent<Image>().enabled = false;

		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TajAR
{
	public class ImageScaleManager : MonoBehaviour
	{
		float imageHeight, parentObjWidth, footerHeight;
		public Vector3 saveStartContentPos;
		public bool ScreenExpand = false, IsAnimationPlay = false;
		public RectTransform CurrentObj;
		public GameObject contentObj, scrollRectObj, closeBT;
		public float ScrollPos = 0;

		[Header("Main Poster Animation")]
		public RectTransform posterImgRect, footer;
		public Sprite[] footerImg;
		public Vector2 startPos, startSize;
		public float AnimZoomSpeed = 5, ImageExpandVal;
		public int experienceNo;



		void Start()
		{

		}

		public void ObjectInit()
		{
			footerHeight = GameManager.fourExpDisplayVal;
			imageHeight = GameManager.imageHeight;
			parentObjWidth = GameManager.imageWidth;
			footer.sizeDelta = new Vector2(parentObjWidth, footerHeight);
			CurrentObj.sizeDelta = new Vector2(parentObjWidth, footerHeight);
			posterImgRect.sizeDelta = new Vector2(parentObjWidth, posterImgRect.sizeDelta.y);
		}


		void Update()
		{
			if (IsAnimationPlay)
			{
				if (ScreenExpand)
				{
					// Main Object height change base on screen size
					CurrentObj.sizeDelta = new Vector2(parentObjWidth, Mathf.Lerp(CurrentObj.sizeDelta.y, imageHeight, Time.deltaTime * 5));

					//  ScrollRect Position set 
					contentObj.transform.localPosition = Vector3.Lerp(contentObj.transform.localPosition, new Vector3(0, ScrollPos, 0), Time.deltaTime * 5f);

					// poster Zoom Animation
					posterImgRect.sizeDelta = new Vector2(Mathf.Lerp(posterImgRect.sizeDelta.x, (parentObjWidth + ImageExpandVal), Time.deltaTime * AnimZoomSpeed), imageHeight);
					posterImgRect.transform.localPosition = Vector3.Lerp(posterImgRect.transform.localPosition, Vector3.zero, Time.deltaTime * 5f);

					// poster image y position change
					//if (imageHeight - CurrentObj.sizeDelta.y <= 500)
					//{
					//	posterImgRect.sizeDelta = new Vector2(Mathf.Lerp(posterImgRect.sizeDelta.x, PosterWidthAfterExpand, Time.deltaTime * AnimZoomSpeed), imageHeight);
					//	posterImgRect.transform.localPosition = Vector3.Lerp(posterImgRect.transform.localPosition, Vector3.zero, Time.deltaTime * 5f);
					//}
					if (imageHeight - CurrentObj.sizeDelta.y <= 2)
					{
						IsAnimationPlay = false;


						CurrentObj.sizeDelta = new Vector2(parentObjWidth, imageHeight);
						contentObj.transform.localPosition = new Vector3(0, ScrollPos, 0);

						posterImgRect.sizeDelta = new Vector2((parentObjWidth + ImageExpandVal), imageHeight);
						posterImgRect.transform.localPosition = Vector3.zero;

						GameManager.inst.openingScreen.GetComponent<Swipe>().enabled = true;
					}
				}
				else
				{
					//  ScrollRect Position set 
					contentObj.transform.localPosition = Vector3.Lerp(contentObj.transform.localPosition, saveStartContentPos, Time.deltaTime * 5f);

					CurrentObj.sizeDelta = new Vector2(parentObjWidth, Mathf.Lerp(CurrentObj.sizeDelta.y, footerHeight, Time.deltaTime * 5));
					posterImgRect.sizeDelta = new Vector2(Mathf.Lerp(posterImgRect.sizeDelta.x, parentObjWidth, Time.deltaTime * 5), Mathf.Lerp(posterImgRect.sizeDelta.y, startSize.y, Time.deltaTime * 5));
					posterImgRect.transform.localPosition = Vector2.Lerp(posterImgRect.transform.localPosition, startPos, Time.deltaTime * 5f);

					if (CurrentObj.sizeDelta.y - footerHeight <= 2)
					{
						GameManager.inst.waitForNextAnim = true;
						IsAnimationPlay = false;

						// Reset Position
						CurrentObj.sizeDelta = new Vector2(parentObjWidth, footerHeight);
						posterImgRect.sizeDelta = new Vector2(parentObjWidth, startSize.y);
						posterImgRect.transform.localPosition = startPos;
						contentObj.transform.localPosition = saveStartContentPos;
						scrollRectObj.GetComponent<ScrollRect>().enabled = true;
					}
				}
			}
		}

		public void ExperienceBTClick()
		{
			if (!ScreenExpand && GameManager.inst.waitForNextAnim)
			{
				if (SingletonController.instance.ifExperiencePlay)
				{
					saveStartContentPos = GameManager.ContentStartPosSave;
				}
				else
				{
					GameManager.ContentStartPosSave = contentObj.transform.localPosition;
					saveStartContentPos = contentObj.transform.localPosition;
				}

				contentObj.GetComponent<VerticalLayoutGroup>().spacing = 40;
				GameManager.inst.currentSwipeObj = experienceNo;

				IsAnimationPlay = true;
				GameManager.inst.waitForNextAnim = false;
				closeBT.GetComponent<Animation>().Play("AlphaOn");
				scrollRectObj.GetComponent<ScrollRect>().enabled = false;
				ScreenExpand = true;
				GetComponent<Image>().sprite = footerImg[1];
			}
		}

		public void ExitBT()
		{
			if (ScreenExpand && !IsAnimationPlay)
			{
				contentObj.GetComponent<VerticalLayoutGroup>().spacing = 21;
				GameManager.inst.currentSwipeObj = experienceNo;
				ScreenExpand = false;
				IsAnimationPlay = true;
				GetComponent<Image>().sprite = footerImg[0];
				closeBT.GetComponent<Animation>().Play("AlphaOff");
				GameManager.inst.openingScreen.GetComponent<Swipe>().enabled = false;
			}
		}
		public void ResetScreenAfterExperiencePlay()
		{
			CurrentObj.sizeDelta = new Vector2(parentObjWidth, imageHeight);
			contentObj.transform.localPosition = new Vector3(0, ScrollPos, 0);

			posterImgRect.sizeDelta = new Vector2((parentObjWidth + ImageExpandVal), imageHeight);
			posterImgRect.transform.localPosition = Vector3.zero;
		}

		public void AnimationOver()
		{
			contentObj.GetComponent<VerticalLayoutGroup>().enabled = true;
			contentObj.GetComponent<ContentSizeFitter>().enabled = true;
		}

	}
}
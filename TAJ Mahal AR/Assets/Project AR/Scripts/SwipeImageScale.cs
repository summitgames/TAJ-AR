using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TajAR
{
	public class SwipeImageScale : MonoBehaviour
	{

		public RectTransform CurrentObj, footer;
		float imageHeight = 0;
		public bool isSwipeOn = false, IsPosterExpand = false;
		[Header("Main Poster Animation")]
		public RectTransform posterImgRect;
		//public Vector2 posterZoomRect, PosterZoomPos;
		public Vector2 StartZoomRect;
		float parentObjWidth, footerHeight;
		public float ImageExpandVal;

		void Start()
		{
		}

		private void OnEnable()
		{
			StartExpanding();
		}

		public void ObjectInit()
		{

			footerHeight = GameManager.fourExpDisplayVal;
			imageHeight = GameManager.imageHeight;
			parentObjWidth = GameManager.imageWidth;


			footer.sizeDelta = new Vector2(parentObjWidth, footerHeight);
			CurrentObj.sizeDelta = new Vector2(parentObjWidth, imageHeight);
			posterImgRect.sizeDelta = new Vector2((parentObjWidth + ImageExpandVal), imageHeight);


			//screenWidth = 500;
			//fullScreenValue = GameManager.fullScreenValue;

			StartZoomRect = new Vector2(posterImgRect.sizeDelta.x, imageHeight);
		}

		void Update()
		{

			if (IsPosterExpand)
			{
				if (isSwipeOn)
				{
					// Main Object height change base on screen size
					CurrentObj.sizeDelta = new Vector2(Mathf.Lerp(CurrentObj.sizeDelta.x, (parentObjWidth + 180), Time.deltaTime * 3),
						Mathf.Lerp(CurrentObj.sizeDelta.y, (imageHeight + 40), Time.deltaTime * 3));

					//CurrentObj.transform.localPosition = Vector3.Lerp(CurrentObj.transform.localPosition, Vector3.zero, Time.deltaTime * 5f);

					// Main Poster height change

					//posterImgRect.transform.localPosition = Vector2.Lerp(posterImgRect.transform.localPosition, PosterZoomPos, Time.deltaTime * 5f);

					posterImgRect.sizeDelta = new Vector2(Mathf.Lerp(posterImgRect.sizeDelta.x, (parentObjWidth + ImageExpandVal + ImageExpandVal), Time.deltaTime * 2), Mathf.Lerp(posterImgRect.sizeDelta.y, (imageHeight + ImageExpandVal), Time.deltaTime * 2));



					// Close Boolen After zoom
					if ((parentObjWidth + 180) - CurrentObj.sizeDelta.x <= 2)
					{
						// Reset
						IsPosterExpand = false;
						CurrentObj.sizeDelta = new Vector2((parentObjWidth + 180), (imageHeight + 40));
						//CurrentObj.transform.localPosition = Vector3.zero;
						//posterImgRect.sizeDelta = new Vector2(posterImgRect.sizeDelta.x, fullScreenValue);

						GameManager.inst.SwipeScreenScroll.enabled = true;
					}
				}
				else
				{
					// Main Object height change base on screen size
					CurrentObj.sizeDelta = new Vector2(Mathf.Lerp(CurrentObj.sizeDelta.x, parentObjWidth, Time.deltaTime * 3), Mathf.Lerp(CurrentObj.sizeDelta.y, imageHeight, Time.deltaTime * 5));

					//CurrentObj.transform.localPosition = Vector3.Lerp(CurrentObj.transform.localPosition, new Vector3(0, -128, 0), Time.deltaTime * 5f);

					// Main Poster height change
					posterImgRect.transform.localPosition = Vector2.Lerp(posterImgRect.transform.localPosition, Vector2.zero, Time.deltaTime * 5f);

					posterImgRect.sizeDelta = new Vector2(Mathf.Lerp(posterImgRect.sizeDelta.x, StartZoomRect.x, Time.deltaTime * 5),
						Mathf.Lerp(posterImgRect.sizeDelta.y, StartZoomRect.y, Time.deltaTime * 5));

					if (CurrentObj.sizeDelta.x - parentObjWidth <= 2)
					{
						//Reset
						IsPosterExpand = false;
						//CurrentObj.sizeDelta = new Vector2(1030, imageHeight);
						posterImgRect.sizeDelta = new Vector2((parentObjWidth + ImageExpandVal), imageHeight);
						GameManager.inst.SwipeDown();
					}
				}
			}
		}

		public void StartExpanding()
		{
			if (!isSwipeOn)
			{
				isSwipeOn = true;
				IsPosterExpand = true;
				//GetComponent<Mask>().enabled = false;
			}
		}

		public void ResetPoster()
		{
			if (isSwipeOn)
			{
				GameManager.inst.SwipeScreenScroll.enabled = false;
				GameManager.inst.openingScreen.GetComponent<Swipe>().enabled = false;
				IsPosterExpand = true;
				isSwipeOn = false;
				//GetComponent<Mask>().enabled = true;
			}
		}

	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TajAR
{
	public class Image360Manager : MonoBehaviour
	{
		public Animation[] variation;
		public int CurrentPlayVariation = 0;

		public Image MainPosterImg;
		public Sprite[] AllImage;
		public Animation LoadingScreen;

		void Start()
		{
			StartCoroutine("DisplayVariationAnim");
		}

		void Update()
		{

		}

		public void BackToMainScene()
		{
			LoadingScreen.Play("FadeIN");
			StartCoroutine("OpenMainScene");
		}

		IEnumerator OpenMainScene()
		{
			yield return new WaitForSeconds(1f);
			Application.LoadLevel("MainScene"); //TajAR
		}
		IEnumerator DisplayVariationAnim()
		{
			for (int i = 0; i < variation.Length; i++)
			{
				yield return new WaitForSeconds(0.1f);
				variation[i].Play("MoveLeft");
			}
			yield return new WaitForSeconds(0.2f);
			variation[0].Play("ZoomIN");
			CurrentPlayVariation = 1;
		}

		public void VariationBTClick(int no)
		{
			if (no + 1 == CurrentPlayVariation)
			{
				return;
			}
			variation[no].Play("ZoomIN");
			variation[CurrentPlayVariation - 1].Play("QuickZoomOut");
			CurrentPlayVariation = no + 1;
			StartCoroutine(ChangeVariatation());
		}

		IEnumerator ChangeVariatation()
		{
			LoadingScreen.Play("FadeInOut");
			yield return new WaitForSeconds(0.5f);
			MainPosterImg.sprite = AllImage[CurrentPlayVariation - 1];
		}
		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}




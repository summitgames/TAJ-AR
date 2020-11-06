using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TajAR
{
	public class LoadingScreen : MonoBehaviour
	{
		public GameObject loadingTx;


		private void Start()
		{
			GetComponent<Animation>().Play("LoadingFadeOut");
			//if (SingletonController.instance.ifExperiencePlay)
			//{
			//	GameManager.inst.BackToMainScreenAfterExperiencePlay();
			//	GetComponent<Animation>().Play("LoadingFadeOut");
			//}
			//else
			//{
			//	GetComponent<Animation>().Play("LoadingFadeOut");
			//}
		}
		public void SwitchScene()
		{
			loadingTx.SetActive(true);
			GameManager.inst.SceneChange(SingletonController.instance.currentExperiencePlay);
		}
		public void BacktoMainScreen()
		{

		}
	}
}

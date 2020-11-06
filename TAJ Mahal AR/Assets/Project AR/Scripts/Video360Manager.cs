using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TajAR
{
	public class Video360Manager : MonoBehaviour
	{
		public Animation LoadingScreen;
		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
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
			Application.LoadLevelAsync("MainScene");
		}
	}
}

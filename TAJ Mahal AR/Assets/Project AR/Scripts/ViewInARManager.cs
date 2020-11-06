using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



namespace TajAR
{

	public class ViewInARManager : MonoBehaviour
	{
		public GameObject[] cloudParticleObj;
		[Header("Light Variation Change")]
		public GameObject lightObj;
		public Light[] lights;

		void Start()
		{

		}

		void Update()
		{

		}

		public void ShowCloud(bool status)
		{
			cloudParticleObj[0].SetActive(status);
			cloudParticleObj[1].SetActive(status);

		}
	}
}

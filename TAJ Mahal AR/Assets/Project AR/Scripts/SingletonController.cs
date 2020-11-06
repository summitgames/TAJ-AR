using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TajAR
{

	public class SingletonController : MonoBehaviour
	{
		public static SingletonController instance;

		public int currentExperiencePlay = 0;
		public bool ifExperiencePlay;

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				instance = this;
				DontDestroyOnLoad(gameObject);
			}
		}
	}
}
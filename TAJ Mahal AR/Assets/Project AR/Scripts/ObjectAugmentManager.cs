using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



namespace TajAR
{
	/// <summary>
	/// Listens for touch events and performs an AR raycast from the screen touch point.
	/// AR raycasts will only hit detected trackables like feature points and planes.
	///
	/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
	/// and moved to the hit position.
	/// </summary>
	[RequireComponent(typeof(ARRaycastManager))]
	public class ObjectAugmentManager : MonoBehaviour
	{

		public Material _360Material;
		public Animation loadingScreen;
		public Animation[] variation;
		//public GameObject preb;
		public Slider scaleSlider;
		public int CurrentPlayVariation;
		bool isZoom, PlayFirstTime;
		public Texture[] all360Texture;

		[Header("Light Variation Change")]
		public GameObject lightObj;
		public Light[] lights;
		public Color sunriseCol, daytimeCol, sunsetCol, twilightCol, nightCol;

		public bool isLightChange;
		public int CurrentLightVariation;
		Quaternion sunrise, daytime, sunset, twilight, night;
		public float saveSliderVal, changeVal;



		[SerializeField]
		[Tooltip("Instantiates this prefab on a plane at the touch location.")]
		GameObject m_PlacedPrefab;
		ARPlaneManager m_ARPlaneManager;

		/// <summary>
		/// The prefab to instantiate on touch.
		/// </summary>
		public GameObject placedPrefab
		{
			get { return m_PlacedPrefab; }
			set { m_PlacedPrefab = value; }
		}

		/// <summary>
		/// The object instantiated as a result of a successful raycast intersection with a plane.
		/// </summary>
		public GameObject spawnedObject { get; private set; }


		//public void TestingBTClck()
		//{
		//	spawnedObject = Instantiate(m_PlacedPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
		//	lightObj = spawnedObject.transform.GetChild(0).gameObject;
		//	lights[0] = spawnedObject.transform.GetChild(0).GetComponent<Light>();
		//	lights[1] = spawnedObject.transform.GetChild(1).GetComponent<Light>();
		//}
		void Awake()
		{
			m_RaycastManager = GetComponent<ARRaycastManager>();
			m_ARPlaneManager = GetComponent<ARPlaneManager>();
		}
		void Start()
		{
			_360Material.SetTexture("_MainTex", null);
			_360Material.SetTexture("_BlendTex", all360Texture[0]);
			_360Material.SetFloat("_Blend", 0);

			sunrise = Quaternion.Euler(9, -85, -75);
			daytime = Quaternion.Euler(73, 10, 8);
			sunset = Quaternion.Euler(16, 85, 72);
			twilight = Quaternion.Euler(-7, 93, 73);
			night = Quaternion.Euler(-8, 58, 80);
		}

		void Update()
		{
			if (isLightChange)
			{
				saveSliderVal = Mathf.Lerp(saveSliderVal, changeVal, Time.deltaTime * 2f);
				/*
				if (CurrentLightVariation == 1)
				{
					lights[0].color = Color.Lerp(lights[0].color, sunriseCol, Time.deltaTime * 1f);
					lights[1].color = Color.Lerp(lights[1].color, sunriseCol, Time.deltaTime * 1f);
					lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, sunrise, Time.deltaTime * 2f);

					_360Material.SetFloat("_Blend", saveSliderVal);
					if (lightObj.transform.rotation == sunrise)
					{
						saveSliderVal = changeVal;
						isLightChange = false;
					}
				}
				else if (CurrentLightVariation == 2)
				{
					lights[0].color = Color.Lerp(lights[0].color, daytimeCol, Time.deltaTime * 1f);
					lights[1].color = Color.Lerp(lights[1].color, daytimeCol, Time.deltaTime * 1f);
					lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, daytime, Time.deltaTime * 2f);

					_360Material.SetFloat("_Blend", saveSliderVal);

					if (lightObj.transform.rotation == daytime)
					{
						saveSliderVal = changeVal;
						isLightChange = false;
					}
				}
				else if (CurrentLightVariation == 3)
				{
					lights[0].color = Color.Lerp(lights[0].color, sunsetCol, Time.deltaTime * 1f);
					lights[1].color = Color.Lerp(lights[1].color, sunsetCol, Time.deltaTime * 1f);
					lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, sunset, Time.deltaTime * 2f);

					_360Material.SetFloat("_Blend", saveSliderVal);

					if (lightObj.transform.rotation == sunset)
					{
						saveSliderVal = changeVal;
						isLightChange = false;
					}
				}
				else if (CurrentLightVariation == 4)
				{
					lights[0].color = Color.Lerp(lights[0].color, twilightCol, Time.deltaTime * 1f);
					lights[1].color = Color.Lerp(lights[1].color, twilightCol, Time.deltaTime * 1f);
					lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, twilight, Time.deltaTime * 2f);

					_360Material.SetFloat("_Blend", saveSliderVal);

					if (lightObj.transform.rotation == twilight)
					{
						saveSliderVal = changeVal;
						isLightChange = false;
					}
				}
				else if (CurrentLightVariation == 5)
				{
					lights[0].color = Color.Lerp(lights[0].color, nightCol, Time.deltaTime * 1f);
					lights[1].color = Color.Lerp(lights[1].color, nightCol, Time.deltaTime * 1f);
					lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, night, Time.deltaTime * 2f);

					_360Material.SetFloat("_Blend", saveSliderVal);

					if (lightObj.transform.rotation == night)
					{
						saveSliderVal = changeVal;
						isLightChange = false;
					}
				}
				 */
			}

			if (!TryGetTouchPosition(out Vector2 touchPosition))
				return;

			if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
			{
				// Raycast hits are sorted by distance, so the first one
				// will be the closest hit.
				var hitPose = s_Hits[0].pose;

				if (spawnedObject == null)
				{
					scaleSlider.gameObject.SetActive(true);
					spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
					SetAllPlanesActive(false);
					//public GameObject lightObj;
					// public Light[] lights;
					lightObj = spawnedObject.GetComponent<ViewInARManager>().lightObj;
					lights[0] = spawnedObject.GetComponent<ViewInARManager>().lights[0];
					lights[1] = spawnedObject.GetComponent<ViewInARManager>().lights[1];
				}
				else
				{
					SetAllPlanesActive(false);
					//spawnedObject.transform.position = hitPose.position;
				}
			}
		}

		public void VariationBTClick(int no)
		{
			//if (isLightChange) { return; }
			if (no + 1 == CurrentPlayVariation)
			{
				return;
			}
			variation[no].Play("ZoomIN");
			variation[CurrentPlayVariation - 1].Play("ZoomOut");
			CurrentPlayVariation = no + 1;

			if (saveSliderVal >= 0.5f)
			{
				changeVal = 0;
				_360Material.SetTexture("_MainTex", all360Texture[no]);
			}
			else
			{
				_360Material.SetTexture("_BlendTex", all360Texture[no]);
				changeVal = 1;
			}
			isLightChange = true;
			CurrentLightVariation = no + 1;
		}
		public void SliderClick()
		{
			isLightChange = false;
			if (!isZoom && scaleSlider.value == 1)
			{
				spawnedObject.GetComponent<ViewInARManager>().ShowCloud(true);
				isZoom = true;
				StartCoroutine(ZoomInDelay());
			}
			else if (isZoom && scaleSlider.value < 1)
			{
				spawnedObject.GetComponent<ViewInARManager>().ShowCloud(false);

				isZoom = false;
				StartCoroutine(ZoomOutDelay());
			}

			//=============================
			float scaleSlideral = scaleSlider.value * 8;
			//preb.transform.localScale = new Vector3(scalescaleSlideral, scalescaleSlideral, scalescaleSlideral);

			if (saveSliderVal == 0)
			{
				_360Material.SetTexture("_BlendTex", null);
				_360Material.SetTexture("_MainTex", all360Texture[CurrentPlayVariation - 1]);

				float temp = 1 - scaleSlider.value;
				_360Material.SetFloat("_Blend", temp);
			}
			else
			{
				_360Material.SetFloat("_Blend", scaleSlider.value);
				_360Material.SetTexture("_MainTex", null);
				_360Material.SetTexture("_BlendTex", all360Texture[CurrentPlayVariation - 1]);
			}

			if (spawnedObject != null && scaleSlider.value > 0.1f)
			{
				spawnedObject.transform.localScale = new Vector3(scaleSlideral, scaleSlideral, scaleSlideral);
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
		IEnumerator ZoomInDelay()
		{
			for (int i = 0; i < variation.Length; i++)
			{
				yield return new WaitForSeconds(0.05f);
				if (isZoom)
				{
					variation[i].Play("MoveLeft");
				}
			}

			yield return new WaitForSeconds(0.25f);
			if (!PlayFirstTime)
			{
				PlayFirstTime = true;
				variation[1].Play("QuickZoomIN");
				CurrentPlayVariation = 2;
			}
			else if (isZoom)
			{
				variation[CurrentPlayVariation - 1].Play("QuickZoomIN");
			}
		}

		IEnumerator ZoomOutDelay()
		{
			if (!isZoom)
			{
				variation[CurrentPlayVariation - 1].Play("QuickZoomOut");
				yield return new WaitForSeconds(0.2f);
			}
			for (int i = 0; i < variation.Length; i++)
			{
				yield return new WaitForSeconds(0.05f);
				variation[i].Play("MoveRight");
			}
		}

		void SetAllPlanesActive(bool value)
		{
			foreach (var plane in m_ARPlaneManager.trackables)
			{
				plane.gameObject.SetActive(value);
			}
		}
		public void BackToMainScene()
		{
			loadingScreen.Play("FadeIN");
			StartCoroutine(OpenMainScene());
		}
		IEnumerator OpenMainScene()
		{
			yield return new WaitForSeconds(1f);
			Application.LoadLevel("MainScene");
		}



		bool TryGetTouchPosition(out Vector2 touchPosition)
		{
#if UNITY_EDITOR
			if (Input.GetMouseButton(0))
			{
				var mousePosition = Input.mousePosition;
				touchPosition = new Vector2(mousePosition.x, mousePosition.y);
				return true;
			}
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif

			touchPosition = default;
			return false;
		}



		static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

		ARRaycastManager m_RaycastManager;
	}

}

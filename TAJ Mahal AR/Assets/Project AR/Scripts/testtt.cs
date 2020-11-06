using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtt : MonoBehaviour
{
	/*
	public Slider mainSlider;
	public Texture firstTex, secondTex;
	public Material myMat;
	*/

	public GameObject lightObj;
	public Light[] lights;
	public Color sunriseCol, daytimeCol, sunsetCol, twilightCol, nightCol;

	public bool isLightChange;
	public int CurrentLightVariation;
	Quaternion sunrise, daytime, sunset, twilight, night;
	int saveSliderVal = 0;

	void Start()
	{
		sunrise = Quaternion.Euler(9, -85, -75);
		daytime = Quaternion.Euler(73, 10, 8);
		sunset = Quaternion.Euler(16,85,72);
		twilight = Quaternion.Euler(-7, 93, 73);
		night = Quaternion.Euler(-8, 58, 80);

	}

	void Update()
	{
		if (isLightChange)
		{
			if (CurrentLightVariation == 1)
			{
				lights[0].color = Color.Lerp(lights[0].color, sunriseCol, Time.deltaTime * 1f);
				lights[1].color = Color.Lerp(lights[1].color, sunriseCol, Time.deltaTime * 1f);
				lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, sunrise, Time.deltaTime * 2f);
				if (lightObj.transform.rotation == sunrise)
				{
					isLightChange = false;
				}
			}
			else if (CurrentLightVariation == 2)
			{
				lights[0].color = Color.Lerp(lights[0].color, daytimeCol, Time.deltaTime * 1f);
				lights[1].color = Color.Lerp(lights[1].color, daytimeCol, Time.deltaTime * 1f);
				lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, daytime, Time.deltaTime * 2f);
				if (lightObj.transform.rotation == daytime)
				{
					isLightChange = false;
				}
			}
			else if (CurrentLightVariation == 3)
			{
				lights[0].color = Color.Lerp(lights[0].color, sunsetCol, Time.deltaTime * 1f);
				lights[1].color = Color.Lerp(lights[1].color, sunsetCol, Time.deltaTime * 1f);
				lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, sunset, Time.deltaTime * 2f);
				if (lightObj.transform.rotation == sunset)
				{
					isLightChange = false;
				}
			}
			else if (CurrentLightVariation == 4)
			{
				lights[0].color = Color.Lerp(lights[0].color, twilightCol, Time.deltaTime * 1f);
				lights[1].color = Color.Lerp(lights[1].color, twilightCol, Time.deltaTime * 1f);
				lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, twilight, Time.deltaTime * 2f);
				if (lightObj.transform.rotation == twilight)
				{
					isLightChange = false;
				}
			}
			else if (CurrentLightVariation == 5)
			{
				lights[0].color = Color.Lerp(lights[0].color, nightCol, Time.deltaTime * 1f);
				lights[1].color = Color.Lerp(lights[1].color, nightCol, Time.deltaTime * 1f);
				lightObj.transform.rotation = Quaternion.Slerp(lightObj.transform.rotation, night, Time.deltaTime * 2f);
				if (lightObj.transform.rotation == night)
				{
					isLightChange = false;
				}
			}

		}


	}

	public void ChangeLight(int no)
	{
		isLightChange = true;
		CurrentLightVariation = no;
	}
	public void SliderClick()
	{
		//gameObject.GetComponent<Renderer>().material.SetFloat("_Blend", mainSlider.value);
	}

	public void ChangeTexture()
	{
		//myMat.SetTexture("_MainTex", null);
		//	myMat.SetTexture("_BlendTex", secondTex);
	}
}

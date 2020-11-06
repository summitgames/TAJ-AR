//This script demonstrates how to return (GetSiblingIndex) and change (SetSiblingIndex) the sibling index of a GameObject.
//Attach this script to the GameObject you would like to change the sibling index of.
//To see this in action, make this GameObject the child of another GameObject, and create siblings for it.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformGetSiblingIndex : MonoBehaviour
{
	//Use this to change the hierarchy of the GameObject siblings
	int m_IndexNumber;

	public void ChangeIndexPos(int no)
	{
		//Initialise the Sibling Index to 0
		//		m_IndexNumber = 0;
		//Set the Sibling Index
		transform.SetSiblingIndex(no);
		//Output the Sibling Index to the console
		//		Debug.Log ("Sibling Index : " + transform.GetSiblingIndex ());
	}

	public void IncreaseIndex()
	{
		if (m_IndexNumber == 6)
		{
			m_IndexNumber = 1;
		}
		else
		{
			m_IndexNumber++;
		}
		ChangeIndexPos(1);
	}
}
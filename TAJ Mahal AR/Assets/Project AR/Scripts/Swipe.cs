using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Right / Left swipt script.
/// </summary>
public class Swipe : MonoBehaviour
{
	public static Swipe swipeInstance;
	private Vector3 startPosition;

	private Vector3 stopPosition;

	private Vector3 swipe;

	private Vector3 lastPosition;

	private List<GameObject> iKnowList;
	private List<GameObject> iDontKnowList;

	[System.Serializable] 
	public class MyEvent : UnityEvent
	{

	}

	[System.Serializable]
	public class normSwipe
	{
		[Tooltip ("Swipe Detection Limit_UD")]
		[Range (0f, 1f)]
		public float swipeDetectionLimit_UD = 0.3f;

		[Tooltip ("Swipe Detection Limit_LR")]
		[Range (0f, 1f)]
		public float swipeDetectionLimit_LR = 0.3f;

		public MyEvent swipeUp;

		public MyEvent swipeDown;

		public MyEvent swipeLeft;

		public MyEvent swipeRight;
	}

	public normSwipe usualSwipes;

	Vector3 swipeVector = Vector3.zero;

	public Vector2 getSwipeVector ()
	{
		return swipeVector;
	}

	[Tooltip ("Swipe Scale")]
	public Vector2 swipeScale = Vector3.zero;
        
	public bool pressed = false;

	private bool oldPressed = false;

	public float actualSwipeDistance = 0f;

	public Vector2 getScaledSwipeVector ()
	{
		Vector2 retVal = Vector2.zero;
		retVal.x = swipeVector.x * swipeScale.x;
		retVal.y = swipeVector.y * swipeScale.y;
		return retVal;
	}

	void Awake ()
	{
		swipeInstance = this;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButton (0)) {
			pressed = true;
		} else {
			pressed = false;
		}
        
		if (oldPressed == false && pressed == true) {
			onClick ();
		}
        
		if (oldPressed == true && pressed == false) {
			onRelease ();
		}

		if (pressed == true) {
           
			swipeVector = Input.mousePosition - startPosition;
            
			swipeVector.x = swipeVector.x / Screen.width;
			swipeVector.y = swipeVector.y / Screen.height;
           
			actualSwipeDistance = swipeVector.magnitude;
		} else {
			actualSwipeDistance = 0f;
			swipeVector = Vector3.zero;
		}

		oldPressed = pressed;
	}

	void onClick ()
	{
		startPosition = Input.mousePosition;
		lastPosition = startPosition;
	}

	public void A_up()
	{
		Debug.Log("up");
	}
	public void A_down()
	{
		Debug.Log("down");
	}


	void onRelease ()
	{        
		stopPosition = Input.mousePosition;

		processSwipe ();
	}

	void processSwipe ()
	{
		if (Mathf.Abs (swipeVector.x) > Mathf.Abs (swipeVector.y)) {
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_LR) {
				if (swipeVector.x > 0f) {
					usualSwipes.swipeRight.Invoke ();                    
				} else {
					usualSwipes.swipeLeft.Invoke ();
				}
			}
		} else {
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_UD) {
                
				if (swipeVector.y > 0f) {
					usualSwipes.swipeUp.Invoke ();
				} else {
					usualSwipes.swipeDown.Invoke ();
				}
			}
		}
		startPosition = stopPosition = Vector3.zero;
	}
}

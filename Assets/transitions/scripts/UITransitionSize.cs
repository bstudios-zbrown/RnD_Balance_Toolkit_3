using UnityEngine;
using System.Collections;



public class UITransitionSize : MonoBehaviour {

	public enum PositionUse		{start,end};
	public enum OffsetUse		{absolute,relative};
	public enum OnEnableUse		{startToEnd,endToStart,positionAtStart,positionAtEnd,none};
	public enum DisableUse		{transitionsToStart,transitionsToEnd,both,none};
	public enum InterpolationUse{linear,smooth,extraSmooth, rubberBand01};


	public RectTransform 	rectTransform;

	public bool				unscaledTime		= false;
	public bool 			loop 				= false;
	public float 			delayTime;
	public float 			transitionTime 		= 1f;
	public float			transitionTime_loop	= 0f;
	public InterpolationUse	interpolation 		= InterpolationUse.extraSmooth;
	public OnEnableUse 		actionOnEnable 		= OnEnableUse.none;
	public DisableUse		disableObjectAfter 	= DisableUse.none;
	public PositionUse 		originalPositionIs 	= PositionUse.start;
	public OffsetUse		offsetUsed 			= OffsetUse.absolute;
	public Vector2			secondPosition;


	private Vector2 		positionStart;
	private Vector2 		positionEnd;


	void Awake(){

		if (rectTransform == null)
			rectTransform = transform as RectTransform;


		switch (offsetUsed)
		{
		case OffsetUse.absolute:
			positionStart 	= (originalPositionIs == PositionUse.start 	? rectTransform.sizeDelta 					: secondPosition);
			positionEnd 	= (originalPositionIs == PositionUse.start 	? secondPosition 							: rectTransform.sizeDelta);
			break;
		case OffsetUse.relative:
			positionStart 	= (originalPositionIs == PositionUse.start 	? rectTransform.sizeDelta 					: rectTransform.sizeDelta + secondPosition);
			positionEnd 	= (originalPositionIs == PositionUse.start 	? rectTransform.sizeDelta + secondPosition 	: rectTransform.sizeDelta);
			break;
		}
	}


	void OnEnable(){

		switch (actionOnEnable) 
		{
		case OnEnableUse.endToStart:
			TransitionToStart (false);
			break;
		case OnEnableUse.startToEnd:
			TransitionToEnd (false);
			break;	
		case OnEnableUse.positionAtStart:
			rectTransform.sizeDelta = positionStart;
			break;
		case OnEnableUse.positionAtEnd:
			rectTransform.sizeDelta = positionEnd;
			break;	
		}
	}


	public void TransitionToEnd(bool myStartFromCurrentPosition){
		
		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionStart), positionEnd, transitionTime, delayTime));
	}
	public void TransitionToEnd(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionStart), positionEnd, myTransitionTime, myDelayTime));
	}


	public void TransitionToStart(bool myStartFromCurrentPosition){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionEnd), positionStart, transitionTime, delayTime));
	}
	public void TransitionToStart(bool myStartFromCurrentPosition, float myTransitionTime, float myDelayTime){

		StopAllCoroutines ();
		StartCoroutine(TransitionRoutine((myStartFromCurrentPosition ? rectTransform.rect.size : positionEnd), positionStart, myTransitionTime, myDelayTime));
	}


	public void TransitionToPosition(Vector2 endPosition){

		TransitionToPosition(endPosition,transitionTime,delayTime);
	}
	public void TransitionToPosition(Vector2 endPosition, float transitionTime, float delayTime){

		StopAllCoroutines ();
		StartCoroutine (TransitionRoutine(rectTransform.rect.size,endPosition,transitionTime,delayTime));
	}


	public IEnumerator TransitionRoutine(Vector2 myPositionStart, Vector2 myPositionEnd, float myTransitionTime, float myDelayTime){

		SetSize(myPositionStart);

		if (myDelayTime > 0)
		{
			if (unscaledTime)
				yield return new WaitForSecondsRealtime(myDelayTime);
			else
				yield return new WaitForSeconds(myDelayTime);
		}


		if (myTransitionTime > 0)
		{
			float t = 0;
			while (t < 1)
			{
				t += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / myTransitionTime;
				float interp = t;

				switch (interpolation)
				{
					case InterpolationUse.smooth:
						interp = Mathf.SmoothStep(0, 1, t);
						break;
					case InterpolationUse.extraSmooth:
						interp = Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, t));
						break;
					case InterpolationUse.rubberBand01:
						interp = (Mathf.Sin(-3f * Mathf.PI * t) / (3f * Mathf.PI * t)) * (1f - t) + 1f;
						break;
				}

				SetSize(Vector2.LerpUnclamped(myPositionStart, myPositionEnd, interp));
				yield return null;
			}
		}

		//make sure end position is reached
		SetSize(myPositionEnd);

		

		if (disableObjectAfter == DisableUse.both ||
			(disableObjectAfter == DisableUse.transitionsToEnd 		&& myPositionEnd == positionEnd) ||
			(disableObjectAfter == DisableUse.transitionsToStart 	&& myPositionEnd == positionStart)) 
		{
			gameObject.SetActive (false);
		}		

		if(loop)
			StartCoroutine(TransitionRoutine(myPositionEnd, myPositionStart, transitionTime_loop != 0 ? transitionTime_loop : transitionTime, 0f));
	}

	void SetSize(Vector2 size)
    {
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
	}
}
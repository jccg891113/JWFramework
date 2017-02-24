using UnityEngine;
using System.Collections;

public class JW_EffectScale : MonoBehaviour
{
	public float delayTime;
	public bool playX;
	public AnimationCurve scaleXCurve;
	public bool playY;
	public AnimationCurve scaleYCurve;
	public bool playZ;
	public AnimationCurve scaleZCurve;
	public float durationTime;
	
	private bool delayStep;
	private float delta;
	private Transform _trans;

	private Transform trans {
		get {
			if (_trans == null) {
				_trans = transform;
			}
			return _trans;
		}
	}

	// Use this for initialization
	void Start ()
	{
		delayStep = (delayTime > 0);
		delta = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (delayStep) {
			if (delta >= delayTime) {
				delayStep = false;
				delta = 0;
			}
		} else {
			float p = Mathf.Clamp01 (delta / durationTime);
			Vector3 localScale = trans.localScale;
			float x = playX ? scaleXCurve.Evaluate (p) : localScale.x;
			float y = playY ? scaleYCurve.Evaluate (p) : localScale.y;
			float z = playZ ? scaleZCurve.Evaluate (p) : localScale.z;
			trans.localScale = new Vector3 (x, y, z);
		}
		delta += Time.deltaTime;
	}
}

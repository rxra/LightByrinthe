using UnityEngine;
using System.Collections;

public class LifeBar : MonoBehaviour {

	public float value = 1.0f;

	private Vector3 _initScale;
	// Use this for initialization
	void Start () {
	
		_initScale = transform.localScale;
		SetValue (value);
	}

	public void SetValue(float v)
	{
		value = Mathf.Clamp01(v);
		renderer.material.mainTextureScale = new Vector2(value, 1.0f);
		transform.localScale = new Vector3(_initScale.x * value, _initScale.y, _initScale.z);
	}
}

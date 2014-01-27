﻿using UnityEngine;
using System.Collections;

public class MidiTranslation : MonoBehaviour {

	public enum JButton
	{
		A_1,
		B_1,
		X_1,
		Y_1
	}
	
	public int key = -1;
	public JButton button = JButton.A_1;
	public float jspeed = 25;
	public float kspeed = 5;
	//public float maxTranslation = 2f;
	public Vector2 maxTranslation = new Vector2(2.0f, 2.0f);
	public Vector3 axis = new Vector3(1,0,0);
	
	private bool midiEnabled = false;
	private bool j360enabled = false;
	
	private float midiZero = 127f/2f;
	private Vector3 initPos;

	// Use this for initialization
	void Start () {
#if UNITY_STANDALONE_WIN
		mManager = MidiManager.instance;
		midiEnabled = mManager!=null && mManager.enabled && mManager.devices>0;
#endif
		initPos = transform.position;
		j360enabled = Input.GetJoystickNames().Length>0&&Input.GetJoystickNames()[0].Contains("360")?true:false;
	}

	private Vector3 backPos;

	// Update is called once per frame
	void Update () {
		
#if UNITY_STANDALONE_WIN
		if (midiEnabled) {
			int v = 0;
			if (mManager.GetKeyVelocity(key, out v)) {
				transform.position = initPos - axis * (((float)v>midiZero)?((midiZero-v)*maxTranslation.x/midiZero):((127-v)*maxTranslation.y/midiZero-maxTranslation.y));
				Debug.Log (initPos + axis * (((float)v>midiZero)?((midiZero-v)*maxTranslation.x/midiZero):((127-v)*maxTranslation.y/midiZero-maxTranslation.y)));
			}/* else {
				//transform.position = initPos;
			}*/
		} else
#endif
		if (j360enabled) {
			if (Input.GetButton(button.ToString())) {
				transform.position = initPos - axis * maxTranslation.x * Input.GetAxis("Triggers_1");
			}
			else
			{
				if(!transform.position.Equals(initPos))
				{
					backPos = transform.position;
					transform.position = Vector3.Lerp(backPos, initPos, 0.5f);
				}
			}
		} else {
			if (Input.GetKey(KeyCode.Alpha1)) {
				if (Input.GetKey(KeyCode.LeftArrow)) {
					Debug.Log ("left arrow");
					transform.position = initPos - axis * maxTranslation.x * Time.deltaTime*kspeed;
				} else if (Input.GetKey(KeyCode.RightArrow)) {
					transform.position = initPos + axis * maxTranslation.x * Time.deltaTime*kspeed;
				}
			}
		}
	}

#if UNITY_STANDALONE_WIN
	private MidiManager mManager; 
#endif

}

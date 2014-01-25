using UnityEngine;
using System.Collections;

public class MidiRotation : MonoBehaviour {

	public enum JButton
	{
		A_1,
		B_1,
		X_1,
		Y_1
	}

	public int key = -1;
	public JButton button = JButton.A_1;
	public float jspeed = 20;
	public float maxRotation = 90f;

	public bool miniEnabled = false;

	private float midiZero = 127f/2f;
	// Use this for initialization
	void Start () {
		mManager = MidiManager.instance;
	}
	
	// Update is called once per frame
	void Update () {

		if (mManager!=null && mManager.enabled) {
			int v = 0;
			if (mManager.GetKeyVelocity(key, out v)) {
				transform.localRotation = Quaternion.Euler(0,0,((float)v)<midiZero?((midiZero-v)*maxRotation/midiZero):((127-v)*maxRotation/midiZero));
			} else {
				transform.localRotation = Quaternion.identity;
			}
		} else {
			if (Input.GetButton(button.ToString())) {
				_currentRotation += Input.GetAxis("Triggers_1")*Time.deltaTime*jspeed;
				if (_currentRotation>maxRotation) {
					_currentRotation = maxRotation;
				} else if (_currentRotation<-maxRotation) {
					_currentRotation = -maxRotation;
				}
				Debug.Log (_currentRotation);
				transform.localRotation = Quaternion.Euler(0,0,_currentRotation);
			}
		}
	}

	private MidiManager mManager; 
	private float _currentRotation = 0;
}

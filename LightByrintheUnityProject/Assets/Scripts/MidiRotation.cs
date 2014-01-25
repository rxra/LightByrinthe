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

	public bool midiEnabled = false;

	private float midiZero = 127f/2f;
	private Quaternion initialeQ;

	// Use this for initialization
	void Start () {
		mManager = MidiManager.instance;
		initialeQ = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {

		if (mManager!=null && mManager.enabled) {
			int v = 0;
			if (mManager.GetKeyVelocity(key, out v)) {
				transform.localRotation = Quaternion.Euler(
					((float)v)<midiZero?((midiZero-v)*maxRotation/midiZero):((127-v)*maxRotation/midiZero-maxRotation),
					initialeQ.eulerAngles.y,
					initialeQ.eulerAngles.z);
			} else {
				transform.localRotation = initialeQ;
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
				transform.localRotation = Quaternion.Euler(_currentRotation,initialeQ.eulerAngles.y,initialeQ.eulerAngles.z);
			}
		}
	}

	private MidiManager mManager; 
	private float _currentRotation = 0;
}

using UnityEngine;
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
	public float jspeed = 20;
	public float maxTranslation = 4f;
	public Vector3 axis = new Vector3(1,0,0);
	
	public bool midiEnabled = false;
	
	private float midiZero = 127f/2f;
	private Vector3 initPos;

	// Use this for initialization
	void Start () {
		mManager = MidiManager.instance;
		initPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (mManager!=null && mManager.enabled) {
			int v = 0;
			if (mManager.GetKeyVelocity(key, out v)) {
				transform.position = initPos + axis * (((float)v<midiZero)?((midiZero-v)*maxTranslation/midiZero):((127-v)*maxTranslation/midiZero-maxTranslation));

				/*transform.localRotation = Quaternion.Euler(
					((float)v)<midiZero?((midiZero-v)*maxRotation/midiZero):((127-v)*maxRotation/midiZero-maxRotation),
					initialeQ.eulerAngles.y,
					initialeQ.eulerAngles.z);*/
			} else {
				transform.position = initPos;
			}
		}/* else {
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
		}*/
	}

	private MidiManager mManager; 

}

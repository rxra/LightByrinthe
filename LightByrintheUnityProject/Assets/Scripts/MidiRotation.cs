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

	public bool miniEnabled = false;

	// Use this for initialization
	void Start () {
		mManager = MidiManager.instance;
	}
	
	// Update is called once per frame
	void Update () {

		if (mManager!=null && mManager.enabled) {
			int v = 0;
			if (mManager.GetKeyVelocity(key, out v)) {
				transform.localRotation = Quaternion.Euler(0,0,v);
			} else {
				transform.localRotation = Quaternion.identity;
			}
		} else {
			if (Input.GetButton(button.ToString())) {
				transform.localRotation = Quaternion.Euler(0,0,Input.GetAxis("Triggers_1")*180f);
			}
		}
	}

	private MidiManager mManager; 
}

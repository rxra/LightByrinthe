using UnityEngine;
using System.Collections;

public class MidiRotation : MonoBehaviour {

	public int key = -1;
	// Use this for initialization
	void Start () {
		mManager = MidiManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		int v = 0;
		if (mManager.GetKeyVelocity(key, out v)) {
			transform.localRotation = Quaternion.Euler(0,0,v);
		} else {
			transform.localRotation = Quaternion.identity;
		}
	}

	private MidiManager mManager; 
}

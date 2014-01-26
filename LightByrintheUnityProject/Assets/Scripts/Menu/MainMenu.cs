using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	}

	void Update()
	{
		if(Input.anyKeyDown) {
			Application.LoadLevel("TestMap");
		}
	}
}

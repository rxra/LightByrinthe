using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public GameObject[] mechas;
	public float timeBeforeStart = 1;
	public float step = 1;
	public GameObject text;

	// Use this for initialization
	void Start ()
	{
#if UNITY_STANDALONE_OSX
		text.GetComponent<GUIText>().text = "Press both triggers (Left/Right)";
#endif
		foreach(GameObject go in mechas) {
			go.SetActive(false);
			StartCoroutine(WaitAndStart(timeBeforeStart,go));
			timeBeforeStart += step;
		}
	}

	IEnumerator WaitAndStart(float waitTime, GameObject go) {
		yield return new WaitForSeconds(waitTime);
		go.SetActive(true);
	}

	IEnumerator WaitAndLoad(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		//mainAudio.clip = mainClip;
		///mainAudio.Play();
		Application.LoadLevel("Level1");
	}
	
	void Update()
	{
#if UNITY_STANDALONE_OSX
		if(Input.GetAxis("LTriggers_1_OSX")!=0 && Input.GetAxis("RTriggers_1_OSX")!=0 && text.activeSelf) {
#else
		if(Input.anyKeyDown && text.activeSelf) {
#endif
			text.audio.Play();
			StartCoroutine(WaitAndLoad(0.25f));
		}
	}
}

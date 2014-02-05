using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

	public GameObject[] objectsToActivate;
	public float timeBeforeStart = 1;
	public float step = 1;
	public GameObject text;
	public Vector3 GoToTarget;

	private List<Actor> actors = new List<Actor>();
	private int actorsVisible = 0;
	private int objectsVisible = 0;
	private bool actorsStarted = false;
	private float timeToWalk;
	private bool j360enabled = false;

	// Use this for initialization
	void Start ()
	{
#if UNITY_STANDALONE_WIN
		j360enabled = Input.GetJoystickNames().Length>0&&Input.GetJoystickNames()[0].Contains("360")?true:false;
#elif UNITY_STANDALONE_OSX
		j360enabled = Input.GetJoystickNames().Length>0?true:false;
#endif

		timeToWalk = timeBeforeStart;

#if UNITY_STANDALONE_OSX
		if (j360enabled) {
			text.GetComponent<GUIText>().text = "Press both triggers (Left/Right)";
		}
#endif
		foreach(GameObject go in objectsToActivate) {
			if (go==null) {
				continue;
			}
			if (go.GetComponent<Actor>()!=null) {
				actors.Add(go.GetComponent<Actor>());
			}
			go.SetActive(false);
			StartCoroutine(WaitAndStart(timeBeforeStart,go));
			timeBeforeStart += step;
		}
	}

	IEnumerator WaitAndStart(float waitTime, GameObject go) {
		yield return new WaitForSeconds(waitTime);
		go.SetActive(true);
		if (go.GetComponent<Actor>()!=null) {
			actorsVisible++;
		}
		objectsVisible++;
	}

	
	IEnumerator WaitAndWalk(float waitTime, Actor go) {
		yield return new WaitForSeconds(waitTime);
		go.WalkRight();
		go.GoTo(GoToTarget);
	}
	
	IEnumerator WaitAndLoad(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		//mainAudio.clip = mainClip;
		GameManager.instance.audio.Stop();
		Application.LoadLevel("Level1");
	}
	
	void Update()
	{
#if UNITY_STANDALONE_OSX
		if ((j360enabled && Input.GetAxis("LTriggers_1_OSX")!=0 && Input.GetAxis("RTriggers_1_OSX")!=0 && text.activeSelf) ||
		    (Input.anyKeyDown && text.activeSelf)) {
#else
		if(Input.anyKeyDown && text.activeSelf) {
#endif
			text.audio.Play();
			StartCoroutine(WaitAndLoad(0.25f));
		}

		if (actorsStarted==false && objectsVisible==objectsToActivate.Length) {
			actorsStarted = true;
			foreach(Actor a in actors) {
				StartCoroutine(WaitAndWalk(timeToWalk,a));
				timeToWalk += step;
			}
		}
	}
}

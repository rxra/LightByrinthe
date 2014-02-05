using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public enum Levels
	{
		Level_1 = 1,
		Level_2,
		Level_3
	}

	public GUIText resumeText;
	public GUIText mainText;
	public GUIText quitText;

	private static Levels _curLevel = Levels.Level_1;

	private static GameManager s_Instance = null;
	public static GameManager instance
	{
		get
		{
			return s_Instance;
		}
	}

	void Awake() {
		s_Instance = this;
		GameObject.DontDestroyOnLoad(this);
	}

	void Start () {

	}

	public void Reset() 
	{
		_curLevel = Levels.Level_1;
		Application.LoadLevel("MainMenu");
	}

	private bool paused = false;
	private int current = 0;
	void Update ()
	{
		if (!paused && Level.instance!=null && (Input.GetKeyUp(KeyCode.Escape) ||
#if UNITY_STANDALONE_OSX
		    Input.GetButton("Start_OSX"))) {
#else
			Input.GetButton("Start"))) {
#endif
			resumeText.enabled = true;
			quitText.enabled = true;
			mainText.enabled = true;
			paused = true;
			Level.instance.Pause(true);
			Level.instance.gameObject.SetActive(false);
		}

		if (paused) {

			if (Input.GetKeyDown(KeyCode.DownArrow) && current<2) {
				current++;
			} else if (Input.GetKeyDown(KeyCode.UpArrow) && current>0) {
				current--;
			} else if (Input.GetKeyDown(KeyCode.Return)) {
				switch(current) {
				case 0:
					paused = false;
					current = 0;
					resumeText.enabled = false;
					mainText.enabled = false;
					quitText.enabled = false;
					Level.instance.Pause(false);
					Level.instance.gameObject.SetActive(true);
					break;
				case 1:
					paused = false;
					current = 0;
					resumeText.enabled = false;
					mainText.enabled = false;
					quitText.enabled = false;
					Reset ();
					break;
				case 2:
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#else
					Application.Quit();
#endif					
					break;
				}
			}
			switch(current) {
			case 0:
				resumeText.color = Color.red;
				mainText.color = Color.white;
				quitText.color = Color.white;
				break;
			case 1:
				resumeText.color = Color.white;
				mainText.color = Color.red;
				quitText.color = Color.white;
				break;
			case 2:
				resumeText.color = Color.white;
				mainText.color = Color.white;
				quitText.color = Color.red;
				break;
			}
		}
	}

	public void RestartLevel()
	{
	}

	public void GoToNextLevel()
	{
		Debug.Log(_curLevel + " " + Application.levelCount);
		if((int)_curLevel < Application.levelCount-1)
		{
			_curLevel++;
			Application.LoadLevel((int)_curLevel);
		}
		else
		{
			Debug.Log ("Reset");
			Reset();
		}
	}
}

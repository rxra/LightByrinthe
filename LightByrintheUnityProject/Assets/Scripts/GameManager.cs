using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public enum Levels
	{
		Level_1 = 1,
		Level_2,
		Level_3
	}
	
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
		if(!GameObject.Find ("GameManager"))
			GameObject.DontDestroyOnLoad(this);
	}

	void Start () {

	}

	public void Reset() 
	{
		_curLevel = Levels.Level_1;
		Application.LoadLevel("MainMenu");
	}

	void Update () {
	
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

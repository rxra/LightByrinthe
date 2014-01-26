using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public enum Levels
	{
		Level_1 = 1,
		Level_2
	}
	
	private Levels _curLevel = Levels.Level_1;

	private static GameManager s_Instance = null;
	public static GameManager instance
	{
		get
		{
			return s_Instance;
		}
	}
	
	void Start () {

		s_Instance = this;
		GameObject.DontDestroyOnLoad(this);
	}

	void Update () {
	
	}

	public void GoToNextLevel()
	{
		if((int)_curLevel < Application.levelCount)
		{
			Application.LoadLevel((int)_curLevel+1);
			_curLevel++;
		}
		else
		{
			// Game is finished
		}
	}
}

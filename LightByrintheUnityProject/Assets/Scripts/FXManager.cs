using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FXManager : MonoBehaviour {

	public List<GameObject> FXs;

	private static FXManager s_Instance = null;
	public static FXManager instance
	{
		get
		{
			return s_Instance;
		}
	}

	void Awake()
	{
		s_Instance = this;
		if(!GameObject.Find("FXManager"))
		{
			DontDestroyOnLoad(gameObject);
		}
	}

	public GameObject Get(string name)
	{
		for(int i = 0; i < FXs.Count; ++i)
		{
			if(FXs[i].name == name)
			{
				return FXs[i];
			}
		}

		return null;
	}
}

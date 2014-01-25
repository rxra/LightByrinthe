using UnityEngine;
using System.Collections;

public class Cell : LightReceiver {

	private int 	_Type;
	private Sprite _Sprite;

	// Use this for initialization
	public override void Start ()
	{
	
	}
	
	// Update is called once per frame
	public override void Update() 
	{
	
	}

	public void SetCellType(int t) 	{ _Type = t; }
	public int GetCellType() 		{ return _Type; }

	public void SetSprite(Sprite spr) 	{ _Sprite = spr; }
	public Sprite GetSprite() 			{ return _Sprite; }

	public Transform GetTransform()
	{
		return gameObject.transform;
	}
}

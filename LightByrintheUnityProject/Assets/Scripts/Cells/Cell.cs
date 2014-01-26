using UnityEngine;
using System.Collections;

public class Cell : LightReceiver {

	private int 	_Type;
	private Sprite _Sprite;
	private Vector2 pos = Vector2.zero;

	public int x
	{
		get
		{
			return (int)pos.x;
		}
	}

	public int y
	{
		get
		{
			return (int)pos.y;
		}
	}

	public Vector2 position
	{
		get
		{
			return pos;
		}
	}

	public void SetPosition(int x, int y)
	{
		pos = new Vector2(x,y);
	}

	public void SetCellType(int t) 	{ _Type = t; }
	public int GetCellType() 		{ return _Type; }

	public void SetSprite(Sprite spr) 	{ _Sprite = spr; }
	public Sprite GetSprite() 			{ return _Sprite; }

	public Transform GetTransform()
	{
		return gameObject.transform;
	}

	public virtual void OnActorEnter(Actor a)
	{
	}

	public virtual void OnActorExit(Actor a)
	{
	}

	protected override void OnLightEnter()
	{
	}
	
	protected override void OnLightExit()
	{
	}
}

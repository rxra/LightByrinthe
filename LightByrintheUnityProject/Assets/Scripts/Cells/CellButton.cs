using UnityEngine;
using System.Collections;

public class CellButton : Cell {

	public override void OnActorEnter(Actor a)
	{
		Debug.Log("Trigger Button!");
	}

	public virtual void OnActorExit(Actor a)
	{
	}

}

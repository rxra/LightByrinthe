using UnityEngine;
using System.Collections;

public class CellExit : Cell 
{
	public override void OnActorEnter(Actor a)
	{
		Debug.Log("Trigger Exit!");
	}
}

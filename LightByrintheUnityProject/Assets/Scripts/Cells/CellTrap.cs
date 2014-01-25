using UnityEngine;
using System.Collections;

public class CellTrap : Cell {
	
	public override void OnActorEnter(Actor a)
	{
		Debug.Log("Trigger Trap!");
	}
}

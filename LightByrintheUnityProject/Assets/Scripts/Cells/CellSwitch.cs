using UnityEngine;
using System.Collections;

public class CellSwitch : Cell {
	
	public override void OnActorEnter(Actor a)
	{
		if(_lightEntered)
		{
			//Level.instance.ActivateButton(a, this);
		}
	}

	public override void OnActorExit(Actor a)
	{
	}

	protected override void OnLightEnter()
	{
	}
	
	protected override void OnLightExit()
	{
	}
}

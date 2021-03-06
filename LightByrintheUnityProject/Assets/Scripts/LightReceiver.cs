﻿using UnityEngine;
using System.Collections;

public class LightReceiver : MonoBehaviour {

	protected bool 	_lightEntered;
	protected bool 	_lightComputation = true;

	protected Level _level;

	// Use this for initialization
	public virtual void Start () {
		_level = Level.instance;
		_lightEntered = false;
	}
	
	// Update is called once per frame
	public virtual void Update () 
	{
		if (_level==null) {
			return;
		}

		if(_lightComputation)
		{
			if(IsInLight())
			{
				if(!_lightEntered)
				{
					OnLightEnter();
					_lightEntered = true;
				}
			}
			else
			{
				if(_lightEntered)
				{
					OnLightExit();
					_lightEntered = false;
				}
			}
		}
	}

	public bool IsInLight()
	{
		for(int i = 0; i< _level.Lights.Count; ++i)
		{
			Vector3 H 			= transform.position;
			Vector3 G 			= _level.Lights[i].transform.position;
			Vector3 D 			= Vector3.Normalize(_level.Lights[i].transform.forward);
			
			float Dist 			= Vector3.Distance(H, G); 
			float GAngle 		= _level.Lights[i].spotAngle;
			
			Vector3 V = Vector3.Normalize(H-G);

			float r = _level.Lights[i].range;
			float p = _level.Lights[i].transform.position.z;

			float nr = (r+p) / r;

			if(Vector3.Dot(V,D) > 0 && (Vector3.Angle(V,D) < GAngle) && (Dist < (_level.Lights[i].range*nr)+0.05f))
				return true;
		}
		
		return false;
	}

	protected virtual void OnLightEnter()
	{
	}

	protected virtual void OnLightExit()
	{
	}
}

using UnityEngine;
using System.Collections;

public class CellExit : Cell 
{
	private GameObject _overlay;

	private float intervaleSin = 1.0f;
	private float intervaleCur = 0.0f;

	public override void Start()
	{
		base.Start ();

		_overlay = (GameObject)GameObject.Instantiate(FXManager.instance.Get("OverlayExit"));
		_overlay.transform.position = transform.position;
		//_overlay.transform.position = transform.position; //new Vector3(transform.position.x, transform.position.y, -0.2f);
		//_overlay.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
	}

	public override void Update()
	{
		//_overlay.renderer.material.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Sin(Time.time)+1.0f);
	}

	public override void OnActorEnter(Actor a)
	{
		a.OnExit();
	}
}

using UnityEngine;
using System.Collections;

public class CellButton : Cell {

	private GameObject _overlay;

	private float _fadeDuration = 1.0f;
	private float _curFade = 0.0f;
	private bool _toFade;

	public override void Start ()
	{
		base.Start();

		_overlay = (GameObject)GameObject.Instantiate(FXManager.instance.Get("OverlayButton"));
		_overlay.transform.position = transform.position;
	}

	public override void Update()
	{
		base.Update();

		if(_toFade)
		{
			_curFade += Time.deltaTime;
			_overlay.renderer.material.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(1.0f, 0.0f, _curFade/_fadeDuration));
		
			if(_curFade>=_fadeDuration)
			{
				_curFade = 0;
				_toFade = false;
			}
		}
	}

	public override void OnActorEnter(Actor a)
	{
	}

	public override void OnActorExit(Actor a)
	{
		_toFade = true;
		Level.instance.ActivateButton(a,this);
	}

	public void FadeButton()
	{
	}
}

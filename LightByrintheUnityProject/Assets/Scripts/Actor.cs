using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : LightReceiver {
	
	public SettlersEngine.MyPathNode[,] _mapNoded;
	public float goTimer;
	private float startTime;
	private bool started = false;
	
	private Vector3 CurCell;
	private Vector3 NextCell;

	private int CurPathIdx;

	private float 	_startTime;
	private bool 	_lerp;

	public float	CooldownCur;
	public float 	CooldownTimer;
	private float 	Speed;

	private bool 	InBlackArea;

	private Vector3 _zOffset;

	private List<Vector2> Path;

	private bool _ReachExit;
	private bool _isDead;

	private LifeBar _lifeBar;

	private Animator anim;							// a reference to the animator on the character

	public AudioClip walkSound;
	public AudioClip dieSound;

	public GameObject fxEnergieLow;
	public GameObject fxEnergieUp;

	// Use this for initialization
	public override void Start () {

		base.Start();

		if (_level==null)
			return;

		startTime = Time.time;
		started = false;

		// initialising reference variables
		anim = GetComponentInChildren<Animator>();					  

		Speed = 0.4f;
		CooldownCur = 0;
		//CooldownTimer = 10.0f;

		_lifeBar = GetComponentInChildren<LifeBar>();

		_isDead = false;
		anim.SetBool("die", false);				

		InBlackArea = true;

		_zOffset = new Vector3(0,0,-0.2f);

		Path = new List<Vector2>();

		Cell spawn 	= _level.GetCellAt(0,0);
		//Cell exit 	= _level.GetCellAt(_level.width-1,_level.height-1);

		//Debug.Log (spawn.Go.transform.position);
		transform.position = spawn.GetTransform().position - new Vector3(0, 0, -1.0f);

	}

	public void StartPath(Cell spawn)
	{
		bool isWall = false;
		_mapNoded = new SettlersEngine.MyPathNode[_level.width, _level.height];
		for (int i = 0; i < _level.width; ++i)
		{
			for (int j = 0; j < _level.height; ++j)
			{
				//Debug.Log(i + "  " + j);
				if(_level.GetCellAt(i,j).GetCellType() == 9 ||
				   _level.GetCellAt(i,j).GetCellType() == 10 ||
				   _level.GetCellAt(i,j).GetCellType() == 5 ||
				   _level.GetCellAt(i,j).GetCellType() == 6 ||
				   _level.GetCellAt(i,j).GetCellType() == 7 ||
				   _level.GetCellAt(i,j).GetCellType() == 8 ||
				   _level.GetCellAt(i,j).GetCellType() == 13 ||
				   _level.GetCellAt(i,j).GetCellType() == 14 ||
				   _level.GetCellAt(i,j).GetCellType() == 15 ||
				   _level.GetCellAt(i,j).GetCellType() == 16 ||
				   _level.GetCellAt(i,j).GetCellType() > 20)

				/*if(_level.GetCellAt(i,j).GetCellType() == 4 ||
					   _level.GetCellAt(i,j).GetCellType() == 10 ||
					   _level.GetCellAt(i,j).GetCellType() == 16 ||
					   _level.GetCellAt(i,j).GetCellType() == 5 ||
					   _level.GetCellAt(i,j).GetCellType() == 6 ||
					   _level.GetCellAt(i,j).GetCellType() == 7 ||
					   _level.GetCellAt(i,j).GetCellType() == 8 ||
					   _level.GetCellAt(i,j).GetCellType() == 9 ||
					   _level.GetCellAt(i,j).GetCellType() == 12 ||
					   _level.GetCellAt(i,j).GetCellType() == 13 ||
					   _level.GetCellAt(i,j).GetCellType() == 14 ||
					   _level.GetCellAt(i,j).GetCellType() == 15 ||
					   _level.GetCellAt(i,j).GetCellType() > 20)*/

				{
					isWall = true;
				}
				else {
					isWall = false;
				}
				//Debug.Log(isWall);
				_mapNoded[i, j] = new SettlersEngine.MyPathNode()
				{
					
					IsWall = isWall,
					X = i,
					Y = j,
				};
			}
		}
		
		IEnumerable<SettlersEngine.MyPathNode> map = getAstarPath(Vector2.zero, new Vector2(_level.width-1,_level.height-2), 2);
		foreach (SettlersEngine.MyPathNode node in map)
		{
			Path.Add(new Vector2(node.X, node.Y));
			//_level.GetCellAt((int)node.X, (int)node.Y).renderer.material.color = Color.green;
		}
		
		CurCell  = spawn.GetTransform().position + _zOffset;
		NextCell = _level.GetCellAt((int)Path[1].x, (int)Path[1].y).GetTransform().position + _zOffset;
		
		CurPathIdx = 1;
		
		_startTime = Time.time;
		_lerp = true;
		
		Vector3 delta = NextCell - CurCell;
		Debug.Log ("delta: " + delta);
		int walk = 0;
		if (delta.y<0) {
			Debug.Log ("down");
			walk = 3;
		} else if (delta.x==0) {
			Debug.Log ("up");
			walk = 4;
		} else if (delta.x<0) {
			Debug.Log ("left");
			walk = 1;
		} else {
			Debug.Log ("right");
			walk = 2;
		}
		anim.SetInteger("walk", walk);
		audio.clip = walkSound;
		audio.loop = true;
		audio.Play();
		started = true;
	}

	public void RecomputeMap(Cell cell)
	{
		cell.SetCellType(1);

		_mapNoded[cell.x, cell.y] = new SettlersEngine.MyPathNode()
		{
			
			IsWall = true,
			X = cell.x,
			Y = cell.y,
		};

		IEnumerable<SettlersEngine.MyPathNode> map = getAstarPath(_level.GetCellAt((int)Path[CurPathIdx].x,(int)Path[CurPathIdx].y).position, new Vector2(_level.width-1,_level.height-2), 2);
		CurCell  = transform.position;
		Path.Clear();
		foreach (SettlersEngine.MyPathNode node in map)
		{
			Path.Add(new Vector2(node.X, node.Y));
			//_level.GetCellAt((int)node.X, (int)node.Y).Go.renderer.material.color = Color.green;
		}
		
		NextCell = _level.GetCellAt((int)Path[1].x, (int)Path[1].y).GetTransform().position + _zOffset;
		CurPathIdx = 1;

		_startTime = Time.time;
		_lerp = true;

		Vector3 delta = NextCell - CurCell;
		Debug.Log ("delta: " + delta);
		int walk = 0;
		if (delta.y<0) {
			Debug.Log ("down");
			walk = 3;
		} else if (delta.x==0) {
			Debug.Log ("up");
			walk = 4;
		} else if (delta.x<0) {
			Debug.Log ("left");
			walk = 1;
		} else {
			Debug.Log ("right");
			walk = 2;
		}
		anim.SetInteger("walk", walk);
	}

	// Update is called once per frame
	public override void Update () 
	{
		if(_isDead)
			return;

		base.Update();

		if (_level==null) {
			return;
		}

		if (Finished()) {
			_lifeBar.SetValue(1);
			return;
		}

		if (!started) {
			if ((Time.time-startTime)>goTimer) {
				StartPath(_level.GetCellAt(0,0));
			}
		}

		if(InBlackArea)
		{
			if (fxEnergieLow.activeSelf==false) {
				fxEnergieLow.SetActive(true);
			}
			if (fxEnergieUp.activeSelf) {
				fxEnergieUp.SetActive(false);
			}

			CooldownCur += Time.deltaTime;

			_lifeBar.SetValue(1.0f-CooldownCur / CooldownTimer);
			if (_lifeBar.audio.isPlaying) {
				_lifeBar.audio.Stop();
			}
			if(CooldownCur >= CooldownTimer)
			{
				_isDead = true;
				OnDead();
			}
		} else {

			CooldownCur -= Time.deltaTime;
			if (CooldownCur<0) {
				CooldownCur = 0;
			}
						
			_lifeBar.SetValue(1.0f-CooldownCur / CooldownTimer);
			if (!_lifeBar.audio.isPlaying) {
				_lifeBar.audio.Play();
			}

			if (fxEnergieLow.activeSelf) {
				fxEnergieLow.SetActive(false);
			}
			if (fxEnergieUp.activeSelf==false) {
				fxEnergieUp.SetActive(true);
			}
		}

		if(transform.position.Equals(NextCell)==false) {
			if(_lerp) {

				float _length = Vector3.Distance(NextCell, CurCell);

				float distCovered = (Time.time - _startTime) * Speed;
				float fracJourney = distCovered / _length;
				transform.position = Vector3.Lerp(CurCell,NextCell,fracJourney);
			}
		}
		else {

			if(CurPathIdx < Path.Count-1)
			{
				_startTime = Time.time;

				CurPathIdx++;
				CurCell = NextCell;
				NextCell = _level.GetCellAt((int)Path[CurPathIdx].x,(int)Path[CurPathIdx].y).GetTransform().position + _zOffset;

				_level.GetCellAt((int)Path[CurPathIdx-1].x,(int)Path[CurPathIdx-1].y).OnActorExit(this);
				_level.GetCellAt((int)Path[CurPathIdx].x,(int)Path[CurPathIdx].y).OnActorEnter(this);

				Vector3 delta = NextCell - CurCell;
				Debug.Log ("delta: " + delta);
				int walk = 0;
				if (delta.y<0) {
					Debug.Log ("down");
					walk = 3;
				} else if (delta.x==0) {
					Debug.Log ("up");
					walk = 4;
				} else if (delta.x<0) {
					Debug.Log ("left");
					walk = 1;
				} else {
					Debug.Log ("right");
					walk = 2;
				}
				anim.SetInteger("walk", walk);	
			}
			else{
				_lerp = false;
				anim.SetInteger("walk", 0);				
				audio.Stop();
			}
		}
	}

	public IEnumerable<SettlersEngine.MyPathNode> getAstarPath(Vector2 start, Vector2 end, int ignoreID)
	{
		SettlersEngine.MySolver<SettlersEngine.MyPathNode, System.Object> aStar = new SettlersEngine.MySolver<SettlersEngine.MyPathNode, System.Object>(_mapNoded);
		return aStar.Search(start, end, null, true);	
	}

	protected override void OnLightEnter()
	{
		//renderer.material.color = Color.red;
		InBlackArea = false;
	}

	protected override void OnLightExit()
	{
		//renderer.material.color = Color.white;
		InBlackArea = true;
	}

	protected void OnDead()
	{
		anim.SetBool("die", true);
		audio.Stop();
		audio.clip = dieSound;
		audio.loop = false;
		audio.volume = 1f;
		audio.Play();
	}

	public void OnExit()
	{
		_ReachExit = true;
		_lifeBar.SetValue(1);
		fxEnergieUp.SetActive(false);
		fxEnergieLow.SetActive(false);
	}

	public bool Dead()
	{
		return _isDead;
	}

	public bool Finished()
	{
		return _ReachExit && !_lerp;
	}
}

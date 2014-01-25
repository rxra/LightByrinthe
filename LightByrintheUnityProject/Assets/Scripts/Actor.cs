using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : LightReceiver {
	
	public SettlersEngine.MyPathNode[,] _mapNoded;
	
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

	public AudioClip dieSound;

	// Use this for initialization
	public override void Start () {

		base.Start();

		// initialising reference variables
		anim = GetComponentInChildren<Animator>();					  

		Speed = 0.4f;
		CooldownCur = 0;
		CooldownTimer = 10.0f;

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

		/*transform.localScale = new Vector3(
			_level.tileSize.x*_level.pixelToWorldRatio*_level.scaleToReference.x,
			_level.tileSize.y*_level.pixelToWorldRatio*_level.scaleToReference.y,
			1.0f
			);
*/
		bool isWall = false;
		_mapNoded = new SettlersEngine.MyPathNode[_level.width, _level.height];
		for (int i = 0; i < _level.width; ++i)
		{
			for (int j = 0; j < _level.height; ++j)
			{
				//Debug.Log(i + "  " + j);
				isWall = _level.GetCellAt(i,j).GetCellType() == 1 ? true : false;
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
			//_level.GetCellAt((int)node.X, (int)node.Y).Go.renderer.material.color = Color.green;
		}

		CurCell  = spawn.GetTransform().position + _zOffset;
		NextCell = _level.GetCellAt((int)Path[2].x, (int)Path[2].y).GetTransform().position + _zOffset;
		CurPathIdx = 1;

		_startTime = Time.time;
		_lerp = true;
		anim.SetInteger("walk", 1);				
	}

	public void RecomputeMap(Cell cell)
	{
		cell.SetCellType(1);


		Debug.Log ("before : " + _mapNoded[cell.x, cell.y].X + " " + _mapNoded[cell.x, cell.y].Y);
		Debug.Log ("before : " + _mapNoded[cell.x, cell.y].IsWall);
		_mapNoded[cell.x, cell.y] = new SettlersEngine.MyPathNode()
		{
			
			IsWall = true,
			X = cell.x,
			Y = cell.y,
		};
		Debug.Log ("after : " + _mapNoded[cell.x, cell.y].X + " " + _mapNoded[cell.x, cell.y].Y);
		Debug.Log ("after : " + _mapNoded[cell.x, cell.y].IsWall);

		IEnumerable<SettlersEngine.MyPathNode> map = getAstarPath(_level.GetCellAt((int)Path[CurPathIdx].x,(int)Path[CurPathIdx].y).position, new Vector2(_level.width-1,_level.height-2), 2);
		CurCell  = transform.position;
		Path.Clear();
		foreach (SettlersEngine.MyPathNode node in map)
		{
			Path.Add(new Vector2(node.X, node.Y));
			//_level.GetCellAt((int)node.X, (int)node.Y).Go.renderer.material.color = Color.green;
		}
		
		NextCell = _level.GetCellAt((int)Path[2].x, (int)Path[2].y).GetTransform().position + _zOffset;
		CurPathIdx = 1;
		
		_startTime = Time.time;
		_lerp = true;
		anim.SetInteger("walk", 1);				
	}

	// Update is called once per frame
	public override void Update () 
	{
		if(_isDead)
			return;

		base.Update();

		if(InBlackArea)
		{
			CooldownCur += Time.deltaTime;

			_lifeBar.SetValue(1.0f-CooldownCur / CooldownTimer);
			if(CooldownCur >= CooldownTimer)
			{
				_isDead = true;
				OnDead();
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
			}
			else{
				_lerp = false;
				anim.SetInteger("walk", 0);				
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
		_lifeBar.SetValue(1.0f);
		InBlackArea = false;
	}

	protected override void OnLightExit()
	{
		//renderer.material.color = Color.white;
		InBlackArea = true;
		CooldownCur = 0;
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
	}

	public bool Dead()
	{
		return _isDead;
	}

	public bool Finished()
	{
		return _ReachExit;
	}
}

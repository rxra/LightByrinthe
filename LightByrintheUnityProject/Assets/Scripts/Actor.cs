using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : MonoBehaviour {

	public int[,] _board;
	public SettlersEngine.MyPathNode[,] _mapNoded;

	private Level _level;

	private Vector3 CurCell;
	private Vector3 NextCell;

	private int CurPathIdx;

	private float _Speed;
	private float _startTime;
	private bool _lerp;

	private Vector3 _zOffset;

	private List<Vector2> Path;

	// Use this for initialization
	void Start () {

		_Speed = 3.0f;

		_zOffset = new Vector3(0,0,-0.2f);

		_level = Level.instance;

		Path = new List<Vector2>();

		Cell spawn 	= _level.GetCellAt(0,0);
		Cell exit 	= _level.GetCellAt(_level.width-1,_level.height-1);

		//Debug.Log (spawn.Go.transform.position);
		transform.position = spawn.Go.transform.position - new Vector3(0, 0, -1.0f);

		transform.localScale = new Vector3(
			_level.tileSize.x*_level.pixelToWorldRatio*_level.scaleToReference.x,
			_level.tileSize.y*_level.pixelToWorldRatio*_level.scaleToReference.y,
			1.0f
			);

		bool isWall = false;
		_mapNoded = new SettlersEngine.MyPathNode[_level.width, _level.height];
		for (int i = 0; i < _level.width; ++i)
		{
			for (int j = 0; j < _level.height; ++j)
			{
				//Debug.Log(i + "  " + j);
				isWall = _level.GetCellAt(i,j).Type == 1 ? true : false;
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

		CurCell  = spawn.Go.transform.position + _zOffset;
		NextCell = _level.GetCellAt((int)Path[2].x, (int)Path[2].y).Go.transform.position + _zOffset;
		CurPathIdx = 1;

		_startTime = Time.time;
		_lerp = true;
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(transform.position.Equals(NextCell)==false) {
			if(_lerp) {

				float speed = 1;
				float _length = Vector3.Distance(NextCell, CurCell);

				float distCovered = (Time.time - _startTime) * speed;
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
				NextCell = _level.GetCellAt((int)Path[CurPathIdx].x,(int)Path[CurPathIdx].y).Go.transform.position + _zOffset;

			}
			else{
				_lerp = false;
			}
		}

		if(IsInLight())
		{
			renderer.material.color = Color.red;
		}
	}
	
	
	public IEnumerable<SettlersEngine.MyPathNode> getAstarPath(Vector2 start, Vector2 end, int ignoreID)
	{
		SettlersEngine.MySolver<SettlersEngine.MyPathNode, System.Object> aStar = new SettlersEngine.MySolver<SettlersEngine.MyPathNode, System.Object>(_mapNoded);
		return aStar.Search(start, end, null, true);	
	}

	public bool IsInLight()
	{
		/*for(int i = 0; i< myLevel.Lights.Count; ++i)
		{
			Vector3 H 			= transform.position;
			Vector3 G 			= myLevel.Lights[i].transform.position;
			Vector3 D 			= Vector3.Normalize(myLevel.Lights[i].transform.forward);
			
			float Dist 			= Vector3.Distance(H, G); 
			float GAngle 		= myLevel.Lights[i].spotAngle;
			
			Vector3 V = Vector3.Normalize(H-G);
			
			RaycastHit hit = new RaycastHit();
			
			if(Physics.Raycast(G, V, out hit, Dist)) 
			{
				if(hit.collider.gameObject != this.gameObject)
					return false;
			}
			//return false;
			return Vector3.Dot(V,D) > 0 && (Vector3.Angle(V,D) < GAngle) && (Dist < myLevel.Lights[i].range);
		}*/

		return false;
	}
}

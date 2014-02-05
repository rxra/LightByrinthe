using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public TextAsset TexLevel;
	public Texture CookieTex;

	public Vector2 tileSize = new Vector2(64,64);
	public Vector2 screenSizeRef = new Vector2(1024,768);

	private List<Cell> 	Grid;
	public List<Light> 	Lights;
	private List<Actor> Actors = new List<Actor>();

	public int NbActors;
	public GameObject actorPrefab;
	public float LifeActor;

	public int width;
	public int height;

	//public float pixelToWorldRatio = 0;
	//public Vector2 scaleToReference;

	public static Level instance = null;

	public List<GameObject> TileSet;

	public float goTimer = 2f;
	public float spawnTimeFreq = 3f;
	private float lastSpawnTime;

	private float finishTimer = 0;
	private float finishDuration = 3f;

	private float deadTimer = 0;
	private float deadDuration = 4f;

	void Awake()
	{
		if(!GameObject.Find("GameManager"))
		{
			GameObject go = new GameObject("GameManager");
			go.AddComponent<GameManager>();
		}

		if(!instance)
			instance = this;

		Grid = new List<Cell>();
		Lights = new List<Light>();
	}

	void Start () 
	{
		lastSpawnTime = Time.time;

		//pixelToWorldRatio = 2*Camera.main.orthographicSize/Screen.height;
		/*scaleToReference = new Vector2(
			(float)Screen.width/screenSizeRef.x,
			(float)Screen.height/screenSizeRef.y
			);*/

		string[] lines = TexLevel.text.Split('\n');
		string[] size = lines[0].Split(',');

		width 	= int.Parse(size[0]);
		height 	= int.Parse(size[1]);

		for(int j  = 1; j <= height; j++)
		{
			string[] v = lines[j].Split(',');
			for(int i = 0; i < width; i++)
			{
				int type = int.Parse(v[i]);

				GameObject go = (GameObject)GameObject.Instantiate(GetTile (type));

				go.transform.position = new Vector3(
					i*tileSize.x+tileSize.x/2-screenSizeRef.x/2,
					screenSizeRef.y/2-j*tileSize.y+tileSize.y/2,
					0);

				go.transform.parent = this.transform;

				Cell c;

				if(type == 18)
				{
					c = go.AddComponent<CellSpawn>();
				}
				if(type == 19)
				{
					c = go.AddComponent<CellExit>();
				}
				else if(type == 11 || type == 12)
				{
					c = go.AddComponent<CellTrap>();
				}
				else if(type == 17)
				{
					c = go.AddComponent<CellButton>();
				}
				else if(type == 10)
				{
					c = go.AddComponent<CellSwitch>();
				}
				else
				{
					c = go.AddComponent<Cell>();
				}

				c.SetCellType(type);
				c.SetPosition(i,j-1);

				Grid.Add(c);
			}
		}

		//transform.position = new Vector3(-(width*0.5f), -(height*0.5f)+1.5f, 2.0f);

		int lightsStart = height+2;
		int nbLights = int.Parse(lines[height+1].Split(',')[0]);

		int lightCount = 0;
		for(int i = lightsStart; i < lightsStart+nbLights; ++i)
		{
			GameObject goLight = new GameObject("Light");

			string[] v = lines[i].Split(',');

			goLight.AddComponent<Light>();

			if(i > lightsStart)
			{
				MidiTranslation mr = goLight.AddComponent<MidiTranslation>() as MidiTranslation;
				mr.key = 9 + lightCount;
#if UNITY_STANDALONE_OSX
				mr.button = MidiTranslation.JButton.A_1_OSX + lightCount;
				#else
				mr.button = MidiTranslation.JButton.A_1 + lightCount;
#endif
				mr.axis = (int.Parse(v[4]) == 1) ? Vector3.up : Vector3.right;
				Vector2 limits = new Vector2(int.Parse(v[11]),int.Parse(v[12]));

				mr.maxTranslation = limits;
				lightCount++;
			}

			Vector3 pos = new Vector3(int.Parse(v[0]), int.Parse(v[1]),0);

			float ox = 0.0f; 
			float oy = 0.0f;

			float.TryParse(v[2], out ox);
			float.TryParse(v[3], out oy);

			Vector3 offset = new Vector3(ox, oy, 0.0f); 

			//Debug.Log (mr.axis);
			float angle = float.Parse(v[5]);
			float intensity = float.Parse(v[6]);
			float range = float.Parse(v[7]);

			Color color = new Color(int.Parse(v[8]),int.Parse(v[9]),int.Parse(v[10]));
                                          

			Cell c = GetCellAt((int)pos[0], (int)pos[1]);
			//Debug.Log (color);
			//goLight.light.color = Color.white;
			goLight.light.type 	= LightType.Spot;

			goLight.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			goLight.transform.position = c.GetTransform().position + new Vector3(offset.x, offset.y, -50f);
			goLight.transform.parent = c.GetTransform().transform;
			goLight.light.spotAngle = angle;
			goLight.light.intensity = intensity;
			goLight.light.range = range;
			goLight.light.color = color;
			goLight.light.cookie = CookieTex;

			Lights.Add(goLight.light);
		}
	}

	public void Pause(bool pause) {
		foreach(Actor a in Actors) {
			a.gameObject.SetActive(!pause);
		}
	}

	public Cell GetCellAt(int x, int y)
	{
		return Grid[x + width * y];
	}

	public void ActivateButton(Actor a, Cell c)
	{
		foreach(Actor b in Actors) {
			if (a!=b) {
				b.RecomputeMap(c);
			}
		}
	}

	void Update()
	{
		if (Actors.Count==0 && NbActors>0) {
			GameObject actor = GameObject.Instantiate(actorPrefab) as GameObject;
			actor.GetComponent<Actor>().goTimer = goTimer;
			actor.GetComponent<Actor>().CooldownTimer = LifeActor;
			Actors.Add (actor.GetComponent<Actor>());
			lastSpawnTime = Time.time;
		} else if (Actors.Count<NbActors) {
			if ((Time.time-lastSpawnTime)>spawnTimeFreq) {
				GameObject actor = GameObject.Instantiate(actorPrefab) as GameObject;
				actor.GetComponent<Actor>().goTimer = goTimer;
				actor.GetComponent<Actor>().CooldownTimer = LifeActor;
				Actors.Add (actor.GetComponent<Actor>());
				lastSpawnTime = Time.time;
			}
		}
		else if (Actors.Count==NbActors)
		{
			int dead 		= 0;
			int finished 	= 0;

			for(int i = 0; i < Actors.Count; ++i)
			{
				if(!Actors[i].Dead())
				{
					if(Actors[i].Finished())
						finished++;
				} else 
				{
					dead++;
				}
			}

			if ((dead+finished)==NbActors) {
				if(dead>0)
				{
					deadTimer += Time.deltaTime;
					if(deadTimer >= deadDuration)
					{
						deadTimer = 0;
						GameManager.instance.Reset();
					}
					return;
				}

				if(finished==NbActors)
				{
					Finish();
				}
			}
		}
	}

	public void Finish()
	{
		finishTimer += Time.deltaTime;
		if(finishTimer >= finishDuration)
		{
			// Reach Next Level
			finishTimer = 0;
			GameManager.instance.GoToNextLevel();
		}
	}

	public GameObject GetTile(int type)
	{
		for(int i = 0; i < TileSet.Count; ++i)
		{
			string name = "Tiles_64_" + (type-1);
			if(TileSet[i].name == name)
				return TileSet[i];
		}

		return TileSet[0];
	}
}

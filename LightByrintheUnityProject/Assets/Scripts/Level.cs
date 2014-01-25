using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public TextAsset TexLevel;
	public Vector2 tileSize = new Vector2(64,64);
	public Vector2 screenSizeRef = new Vector2(1024,768);

	private List<Cell> 	Grid;
	public List<Light> 	Lights;
	private List<Actor> Actors = new List<Actor>();

	public int NbActors;
	public GameObject actorPrefab;

	public int width;
	public int height;

	public float pixelToWorldRatio = 0;
	public Vector2 scaleToReference;

	public static Level instance = null;

	public List<GameObject> TileSet;

	public float firstSpawnTime = 2f;
	public float spawnTimeFreq = 5f;
	private float lastSpawnTime;

	void Awake()
	{
		if(!instance)
			instance = this;

		Grid = new List<Cell>();
		Lights = new List<Light>();
	}

	void Start () 
	{
		lastSpawnTime = Time.time;

		pixelToWorldRatio = 2*Camera.main.orthographicSize/Screen.height;
		scaleToReference = new Vector2(
			(float)Screen.width/screenSizeRef.x,
			(float)Screen.height/screenSizeRef.y
			);

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

				GameObject go = (GameObject)GameObject.Instantiate(TileSet[type]);//(GameObject)GameObject.Instantiate(Tile_1);//GameObject.CreatePrimitive(PrimitiveType.Quad);

				/*go.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
					i*tileSize.x+tileSize.x/2-screenSizeRef.x/2,
					screenSizeRef.y/2-j*tileSize.y+tileSize.y/2,
				 	0
				));*/

				go.transform.position = new Vector3(
					(i*tileSize.x+tileSize.x/2-screenSizeRef.x/2)*pixelToWorldRatio*scaleToReference.x,
					((screenSizeRef.y/2-j*tileSize.y+tileSize.y/2)*pixelToWorldRatio*scaleToReference.x),
					0);

				go.transform.parent = this.transform;
				go.transform.localScale = new Vector3(
					tileSize.x*pixelToWorldRatio*scaleToReference.x,
					tileSize.y*pixelToWorldRatio*scaleToReference.y,
					1
				);

				Cell c;

				if(type == 5)
				{
				 	c = go.AddComponent<CellSpawn>();
				}
				if(type == 6)
				{
					c = go.AddComponent<CellExit>();
				}
				else if(type == 7)
				{
					c = go.AddComponent<CellTrap>();
				}
				else if(type == 8)
				{
					c = go.AddComponent<CellButton>();
				}
				else
				{
					c = go.AddComponent<Cell>();
				}
				//if(type == 1)
				//{
				//	go.renderer.material.color = Color.green;
				//}

				//Cell c = go.AddComponent<Cell>();
				c.SetCellType(type);

				Grid.Add(c);
			}
		}

		//transform.position = new Vector3(-(width*0.5f), -(height*0.5f)+1.5f, 2.0f);

		int lightsStart = height+2;
		int nbLights = int.Parse(lines[height+1].Split(',')[0]);

		for(int i = lightsStart; i < lightsStart+nbLights; ++i)
		{
			GameObject goLight = new GameObject("Light");
			goLight.AddComponent<Light>();
			MidiRotation mr = goLight.AddComponent<MidiRotation>() as MidiRotation;
			mr.key = 9 + Lights.Count;
			mr.button = MidiRotation.JButton.A_1 + Lights.Count;

			string[] v = lines[i].Split(',');

			Vector3 pos = new Vector3(int.Parse(v[0]), int.Parse(v[1]), -0.4f);

			float ox = 0.0f; 
			float oy = 0.0f;

			float.TryParse(v[2], out ox);
			float.TryParse(v[3], out oy);

			Vector3 offset = new Vector3(ox, oy, 0.0f); 
			int orientation = int.Parse(v[4]);//Vector3 lookAt = new Vector3(int.Parse(v[4]), int.Parse(v[5]), 0.0f);
			int angle = int.Parse(v[5]);
			int intensity = int.Parse(v[6]);
			int range = int.Parse(v[7]);

			Cell c = GetCellAt((int)pos[0], (int)pos[1]);

			goLight.light.color = Color.white;
			goLight.light.type 	= LightType.Spot;

			goLight.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			goLight.transform.position = c.GetTransform().position + new Vector3(offset.x, offset.y, -1f);
			goLight.transform.parent = c.GetTransform().transform;
			goLight.light.spotAngle = angle;
			goLight.light.intensity = intensity;
			goLight.light.range = range;

			Lights.Add(goLight.light);
		}
	}

	public Cell GetCellAt(int x, int y)
	{
		return Grid[x + width * y];
	}

	void Update()
	{
		if (Actors.Count==0 && NbActors>0) {
			if ((Time.time-lastSpawnTime)>firstSpawnTime) {
				GameObject actor = GameObject.Instantiate(actorPrefab) as GameObject;
				Actors.Add (actor.GetComponent<Actor>());
				lastSpawnTime = Time.time;
			}
		} else if (Actors.Count<NbActors) {
			if ((Time.time-lastSpawnTime)>spawnTimeFreq) {
				GameObject actor = GameObject.Instantiate(actorPrefab) as GameObject;
				Actors.Add (actor.GetComponent<Actor>());
				lastSpawnTime = Time.time;
			}
		}

		/*if (Input.GetButton("A_1")) {
			Debug.Log("b");
		}*/
	}
}

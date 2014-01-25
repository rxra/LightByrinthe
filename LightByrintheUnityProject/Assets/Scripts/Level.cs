using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Cell
{
	public Cell(GameObject go, int type)
	{
		Type 	= type;
		Go 		= go;
	}

	public int 			Type;
	public GameObject	Go;
}

public class Level : MonoBehaviour {

	public TextAsset TexLevel;

	private List<Cell> Grid;
	private int width;
	private int height;
	
	void Start () 
	{
		Grid = new List<Cell>();

		string[] lines = TexLevel.text.Split('\n');
		string[] size = lines[0].Split(',');

		width 	= int.Parse(size[0]);
		height 	= int.Parse(size[1]);

		for(int j  = 1; j <= height; ++j)
		{
			string[] v = lines[j].Split(',');
			for(int i = 0; i < width; ++i)
			{
				int type = int.Parse(v[i]);

				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
				
				go.transform.position = new Vector3(i, height - j-1, 0);
				go.transform.parent = this.transform;

				/*if(type == 1)
				{
					go.renderer.material.color = Color.green;
				}*/

				Grid.Add(new Cell(go, 0));
			}
		}

		transform.position = new Vector3(-(width*0.5f), -(height*0.5f)+1.5f, 2.0f);

		int lightsStart = height+2;
		int nbLights = int.Parse(lines[height+1].Split(',')[0]);

		for(int i = lightsStart; i < lightsStart+nbLights; ++i)
		{
			GameObject goLight = new GameObject("Light");
			goLight.AddComponent<Light>();

			string[] v = lines[i].Split(',');

			Vector3 pos = new Vector3(int.Parse(v[0]), int.Parse(v[1]), -0.3f);

			float ox = 0.0f; 
			float oy = 0.0f;

			float.TryParse(v[2], out ox);
			float.TryParse(v[3], out oy);

			Vector3 offset = new Vector3(ox, oy, 0.0f); 
			Vector3 lookAt = new Vector3(int.Parse(v[4]), int.Parse(v[5]), 0.0f);
			int angle = int.Parse(v[6]);
			int intensity = int.Parse(v[7]);
			int range = int.Parse(v[8]);
			                       
			/*Debug.Log (pos);
			Debug.Log (offset);
			Debug.Log (lookAt);
			Debug.Log (angle);
			Debug.Log (intensity);*/

			Cell c = GetCellAt((int)pos[0], (int)pos[1]);

			goLight.light.color = Color.white;
			goLight.light.type 	= LightType.Spot;

			goLight.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
			goLight.transform.position = c.Go.transform.position + new Vector3(offset.x, offset.y, -0.15f);
			goLight.transform.parent = c.Go.transform;
			goLight.light.spotAngle = angle;
			goLight.light.intensity = intensity;
			goLight.light.range = range;
		}
	}

	Cell GetCellAt(int x, int y)
	{
		return Grid[x + width * y];
	}
}

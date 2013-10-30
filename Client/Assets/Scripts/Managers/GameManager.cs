using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	
	public string mapName;
	
	public UICore core;
	
	public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
	
	public GameObject selectionCircle;
	
	public GameObject defaultTextureTile;
	public GameObject defaultFogTile;
	public GameObject[] doodads;
	Dictionary<string, GameObject> doodadsByName = new Dictionary<string, GameObject>();
	
	[HideInInspector]
	public int DEFAULT_TILE_STEP=2;
	[HideInInspector]
	public float DEFAULT_DOODAD_STEP=1f;
	
	public Hashtable world = new Hashtable();
	List<string> tiles = new List<string>();
	
	public List<Hashtable> entityInfos = new List<Hashtable>();
	public Hashtable entityInfosByName = new Hashtable();
		
	public Hashtable skills = new Hashtable();
	public Hashtable items = new Hashtable();
		
	public Hashtable teamsInfos = new Hashtable();
	public Hashtable mapInfos = new Hashtable();
	
	// Use this for initialization
	void Start () 
	{
		//we need to index the doodads for map loading.
		foreach(GameObject go in doodads)
		{
			doodadsByName.Add(go.name, go);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void clearFog(Vector3 point, float brushSize)
	{
		int checkRadius = (int)Mathf.Ceil(brushSize*3);
		if(checkRadius<3)
			checkRadius = 3;
		
		List<string> testBrush = getTilesAroundPoint(point, checkRadius);
		foreach(string s in testBrush)
		{
			if(Vector3.Distance(((GameObject)((Hashtable)world[s])["textureTile"]).transform.position, point)<brushSize*DEFAULT_TILE_STEP)
			{
				FogTileHandler fog = ((GameObject)((Hashtable)world[s])["fogTile"]).GetComponent<FogTileHandler>();
				
				Dictionary<int, float> colliders = fog.findColliders(point, brushSize, 0.1f);
				
				fog.clearFog(point, brushSize, colliders, 1);		
			}
		}
	}
	
	public void enableFog()
	{
		foreach(string s in tiles)
		{
			Hashtable tile = (Hashtable)world[s];
			(tile["fogTile"] as GameObject).GetComponent<FogTileHandler>().setDefaultFog(new Color32(0, 0, 0, 255));
		}
	}
	
	public void loadMap(string path)
	{
		//clear the previous map Elements
		removeAllMapElements();
		
		world.Clear();
		tiles.Clear();
		
		entityInfos = new List<Hashtable>();
		entityInfosByName = new Hashtable();
		
		skills = new Hashtable();
		items = new Hashtable();
		
		teamsInfos = new Hashtable();
		mapInfos = new Hashtable();
		
		Hashtable map = core.io.loadMapInfos(path);
		
		mapInfos = (Hashtable)map["mapInfos"];
		skills = (Hashtable)map["skills"];
		items = (Hashtable)map["items"];
		
		teamsInfos = (Hashtable)map["teamsInfos"];
		
		//useless actually, since the server handle entities
		entityInfosByName = (Hashtable)map["entityInfos"];
				
		foreach(string s in entityInfosByName.Keys)
		{
			entityInfos.Add((Hashtable)entityInfosByName[s]);
		}
		
		Hashtable mapData = (Hashtable)map["mapData"];
		
		foreach(string s in mapData.Keys)
		{
			Hashtable elementInfos = (Hashtable)mapData[s];
			
			if(s[0].Equals('d')) //if this is a doodad...
			{
				addDoodadSilent(new Vector3((float)elementInfos["x"], (float)elementInfos["y"], (float)elementInfos["z"]), doodadsByName[elementInfos["model"].ToString()]);
			}
			else
			{
				Hashtable tileInfos = addTile(new Vector3((float)elementInfos["x"], (float)elementInfos["y"], (float)elementInfos["z"]));
				
				//we let unity load the textures...
				StartCoroutine(loadTexture(Application.dataPath+"/Maps/"+mapName+"/tex_"+s, tileInfos["textureTile"] as GameObject));
				
				//the color layer will be disabled to save disc space...
				//StartCoroutine(loadTexture(Application.dataPath+"/Maps/"+mapName+"/colo_"+s, tileInfos["colorTile"] as GameObject));
				
				//we rebuild the vertices array and apply it to the tiles meshes.
				Hashtable vertices = (Hashtable) elementInfos["verts"];
				
				Vector3[] finalVertices = new Vector3[vertices.Count];
				for(int i=0; i<vertices.Count; i++)
				{
					Hashtable vertInfos = (Hashtable) vertices[i];
					finalVertices[i] = new Vector3((float)vertInfos["x"], (float)vertInfos["y"], (float)vertInfos["z"]);
				}
				
				//we apply the vertices to the texture and color meshes...
				VerticlesIndexer textureVerticlesIndexer = (tileInfos["textureTile"] as GameObject).GetComponent<VerticlesIndexer>();
				textureVerticlesIndexer.refreshIndex();
				textureVerticlesIndexer._verts = finalVertices;
				textureVerticlesIndexer.updateVertices();
				
				//we apply the vertices to the texture and color meshes...
				VerticlesIndexer fogVerticlesIndexer = (tileInfos["fogTile"] as GameObject).GetComponent<VerticlesIndexer>();
				fogVerticlesIndexer.refreshIndex();
				fogVerticlesIndexer._verts = finalVertices;
				fogVerticlesIndexer.updateVertices();	
			}
		}
		
		if(mapInfos["fog"]!=null)
		{
			//if((bool) mapInfos["fog"])
			Invoke("enableFog", 1);
		}
		else
		{
			Invoke("enableFog", 1);
		}
	}
	
	public Hashtable addTile(Vector3 position)
	{
		string id = getIdWithPosition(position, DEFAULT_TILE_STEP);
		
		if(world[id]==null)
		{
			//the color tile always comes in front.
			GameObject textureTile = (GameObject) Instantiate(defaultTextureTile, position, Quaternion.identity);
			GameObject fogTile = (GameObject) Instantiate(defaultFogTile, position+new Vector3(0, 0.02f, 0), Quaternion.identity);
			
			textureTile.GetComponent<VerticlesIndexer>().Id = id;
			
			Hashtable tileInfos = new Hashtable();
			tileInfos.Add("id", id);
			tileInfos.Add("x", textureTile.transform.position.x);
			tileInfos.Add("y", textureTile.transform.position.y);
			tileInfos.Add("z", textureTile.transform.position.z);
			
			//we keep a reference to the tile's gameobjects
			tileInfos.Add("textureTile", textureTile);
			tileInfos.Add("fogTile", fogTile);
			tileInfos.Add("verticlesIndexer", textureTile.GetComponent<VerticlesIndexer>());
			tileInfos.Add("FogTileHandler", fogTile.GetComponent<FogTileHandler>());
			
			world.Add(id, tileInfos);
			tiles.Add(id);
			
			return tileInfos;
		}
		
		return null;
	}
	
	//adds a doodads without taking the map's vertices in account.
	public void addDoodadSilent(Vector3 position, GameObject doodad)
	{
		string id = "d_"+getIdWithPosition(position, DEFAULT_DOODAD_STEP);
		
		if(world[id]==null)
		{
			GameObject tmpDoodad = (GameObject) Instantiate(doodad, smash(position, DEFAULT_DOODAD_STEP), Quaternion.identity);
			tmpDoodad.name = id;
			
			tmpDoodad.transform.position = tmpDoodad.transform.position;
		
			Hashtable tileInfos = new Hashtable();
			tileInfos.Add("id", id);
			tileInfos.Add("x", tmpDoodad.transform.position.x);
			tileInfos.Add("y", tmpDoodad.transform.position.y);
			tileInfos.Add("z", tmpDoodad.transform.position.z);
			
			tileInfos.Add("rotation_x", tmpDoodad.transform.eulerAngles.x); 
			tileInfos.Add("rotation_y", tmpDoodad.transform.eulerAngles.y); 
			tileInfos.Add("rotation_z", tmpDoodad.transform.eulerAngles.z); 
			tileInfos.Add("scale", tmpDoodad.transform.localScale.x);
			
			//we keep a reference to the doodad's gameobjects
			tileInfos.Add("doodad", tmpDoodad); 
			
			world.Add(id, tileInfos);
		}
	}
	
	void removeAllMapElements()
	{
		
	}
	
	public List<string> getTilesAroundPoint(Vector3 point, int radius)
	{
		List<string> tiles = new List<string>();
		//to avoid looping around every tile in the maps, we use the indexed ids
		for(int i=-radius; i<radius; i++)
		{
			for(int j=-radius; j<radius; j++)
			{
				Vector3 tmpTilePosition = point+new Vector3(i*DEFAULT_TILE_STEP, 0, j*DEFAULT_TILE_STEP);
				string tmpId = getIdWithPosition(tmpTilePosition, DEFAULT_TILE_STEP);
				
				if(world[tmpId]!=null)
				{
					//tile exists, add it to the list...
					tiles.Add(tmpId);
				}
			}	
		}
		
		return tiles;
	}
	
	public string getNearestTile(Vector3 point)
	{
		Vector3 tmpTilePosition = point+new Vector3(DEFAULT_TILE_STEP, 0, DEFAULT_TILE_STEP);
		string tmpId = getIdWithPosition(tmpTilePosition, DEFAULT_TILE_STEP);
		
		if(world[tmpId]!=null)
		{
			return tmpId;
		}
		else
			return string.Empty;
	}
	
	public string getIdWithPosition(Vector3 position, float defaultStep)
	{
		return smash(position, defaultStep).ToString();
	}

    public Vector3 smash(Vector3 position, float factor)
    {
        return new Vector3((float)Mathf.Floor(position.x / factor) * factor, (float)Mathf.Floor(position.y / factor) * factor, (float)Mathf.Floor(position.z / factor) * factor);
    }

    public Vector3 smash(Vector3 position, Vector3 factorAsVector)
    {
        return new Vector3((float)Mathf.Floor(position.x / factorAsVector.x) * factorAsVector.x, (float)Mathf.Floor(position.y / factorAsVector.y) * factorAsVector.y, (float)Mathf.Floor(position.z / factorAsVector.z) * factorAsVector.z);
    }
	
	public IEnumerator loadTexture(string url, GameObject target)
	{
		url = "file://"+ url;
		
		WWW www = new WWW(url);
		
		yield return www;
		
		target.renderer.material.mainTexture = www.texture;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BrushType
{
	none, selection, sculpt, paint, texturePaint, fog, doodads, entities, skills, items, map
}

public class WorldEditor : MonoBehaviour {
	
	public string mapName=string.Empty;
	public IOManager ioManager = new IOManager();
	
	public LayerMask TilesLayer;
	public LayerMask DoodadsLayer;
	
	public Rect guiArea = new Rect(0, 10, 400, 1000);
	
	public GameObject selection=null;
	public GameObject selectionCube;
	
	public BrushType brushType;
	public float brushSize = 1;
	public float brushIntensity = 1;
	
	//for sculpt brush only...
	public Vector3 brushDirection = new Vector3(0, 1, 0);
	public float maxHeight = 50;
	
	//for paint brush only...
	public Vector4 brushColor = new Vector4(0, 0, 0, -1);
	public float maxTransparency = 1;
	
	//for texture brush only...
	public Texture2D brushTexture;
	
	//for doodads brush only...
	public GameObject brushDoodad;
	public int brushMode=0;
	
	//for entity brush only...
	public string brushEntity = string.Empty;
	
	//Terrain assets...
	public GameObject defaultTextureTile;
	public GameObject defaultColorTile;
	public GameObject defaultFogTile;
	public GameObject defaultEntity;
	
	public Texture2D[] terrainTextures;
	public GameObject[] doodads;
	Dictionary<string, GameObject> doodadsByName = new Dictionary<string, GameObject>();
	
	[HideInInspector]
	public int DEFAULT_TILE_STEP=2;
	[HideInInspector]
	public float DEFAULT_DOODAD_STEP=1f;
	
	//This is where all the map's indexed informations are stored.
	Hashtable world = new Hashtable();
	
	List<string> tiles = new List<string>();
	
	//game infos...
	public List<Hashtable> entityInfos = new List<Hashtable>();
	public Hashtable entityInfosByName = new Hashtable();
	public Hashtable entities = new Hashtable();
	
	public Hashtable skills = new Hashtable();
	public Hashtable items = new Hashtable();
	
	public List<Hashtable> teams = new List<Hashtable>();
	public Hashtable teamsInfos = new Hashtable();
	public Hashtable mapInfos = new Hashtable();

	public List<string> Tiles 
	{
		get 
		{
			return this.tiles;
		}
		set 
		{
			tiles = value;
		}
	}
	public Hashtable World 
	{
		get 
		{
			return this.world;
		}
		set 
		{
			world = value;
		}
	}	
	// Use this for initialization
	void Start () 
	{
		//we need to index the doodads for map loading.
		foreach(GameObject go in doodads)
		{
			doodadsByName.Add(go.name, go);
		}
		
		//drawEmptyMap(new Vector2(20, 20));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButton(0) && !guiArea.Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y)))
		{
			if(brushType==BrushType.selection)
			{
				Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		
				RaycastHit hitFloor2;
				if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
				{
					selectionCube.transform.localScale = Vector3.one*hitFloor2.collider.bounds.size.x;
					selection = hitFloor2.collider.gameObject;
				}
			}
			else if(brushType==BrushType.doodads)
			{
				Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		
				RaycastHit hitFloor2;
				
				if(brushMode>0)
				{
					if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f, DoodadsLayer.value)) 
					{
						removeElement(hitFloor2.collider.name);
					}
				}
				else
				{
					if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f, TilesLayer.value)) 
					{
						for(float i=-brushSize/5; i<brushSize/5; i++)
						{
							for(float j=-brushSize/5; j<brushSize/5; j++)
								addDoodad(hitFloor2.point+new Vector3(i*DEFAULT_DOODAD_STEP, 0, j*DEFAULT_DOODAD_STEP), brushDoodad);
						}
					}
				}
			}
			else if(brushType==BrushType.entities)
			{
				//if i have selected an entity...
				if(brushEntity.Length>0)
				{
					Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			
					RaycastHit hitFloor2;
					
					if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f, TilesLayer.value)) 
					{
						addEntity(hitFloor2.point, brushEntity);
					}
				}
			}
			else
			{
				Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		
				RaycastHit hitFloor2;
				if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f, TilesLayer.value)) 
				{
					int checkRadius = (int)Mathf.Ceil(brushSize*3);
					if(checkRadius<6)
						checkRadius = 6;
					
					List<string> testBrush = getTilesAroundPoint(hitFloor2.point, checkRadius);
					foreach(string s in testBrush)
					{
						
						if(brushType==BrushType.sculpt)
						{
							applySmoothTranslationToDoodads(hitFloor2.point, brushDirection, brushSize, brushIntensity, maxHeight);
							((GameObject)((Hashtable)world[s])["textureTile"]).GetComponent<VerticlesIndexer>().applySmoothTranslationToVerticles(hitFloor2.point, brushDirection, brushSize, brushIntensity, maxHeight);
							((GameObject)((Hashtable)world[s])["colorTile"]).GetComponent<VerticlesIndexer>().applySmoothTranslationToVerticles(hitFloor2.point, brushDirection, brushSize, brushIntensity, maxHeight);
							((GameObject)((Hashtable)world[s])["fogTile"]).GetComponent<VerticlesIndexer>().applySmoothTranslationToVerticles(hitFloor2.point, brushDirection, brushSize, brushIntensity, maxHeight);
						}
						
						if(Vector3.Distance(((GameObject)((Hashtable)world[s])["textureTile"]).transform.position, hitFloor2.point)<brushSize*DEFAULT_TILE_STEP)
						{
							if(brushType==BrushType.paint)
								((GameObject)((Hashtable)world[s])["colorTile"]).GetComponent<TileColorHandler>().paint(hitFloor2.point, brushColor, brushSize, brushIntensity, maxTransparency);
							
							if(brushType==BrushType.texturePaint)
								((GameObject)((Hashtable)world[s])["textureTile"]).GetComponent<TileColorHandler>().paintTexture(hitFloor2.point, brushTexture, brushSize, brushIntensity, maxTransparency);
						
							if(brushType==BrushType.fog)
							{
								FogTileHandler fog = ((GameObject)((Hashtable)world[s])["fogTile"]).GetComponent<FogTileHandler>();
								
								Dictionary<int, float> colliders = fog.findColliders(hitFloor2.point, brushSize, 0.1f);
								
								fog.clearFog(hitFloor2.point, brushSize, colliders, 1);
							}
						}
					}
				}
			}
		}
		
		//process elements to remove in the queue...
		
		for(int i=0; i<removeQueue.Count; i++)
		{
			removeElement(removeQueue[i]);
		}
		
		removeQueue.Clear();
	}
	
	//render an empty map with default grass texture.
	public void drawEmptyMap(Vector2 mapSize)
	{
		//clear the previous map Elements
		foreach(string s in world.Keys)
		{
			removeElement(s);	
		}
		
		world.Clear();
		tiles.Clear();
		
		entityInfos = new List<Hashtable>();
		entityInfosByName = new Hashtable();
		entities = new Hashtable();
		
		skills = new Hashtable();
		items = new Hashtable();
		
		teams = new List<Hashtable>();
		teamsInfos = new Hashtable();
		mapInfos = new Hashtable();
		
		for(int i=0; i<mapSize.x; i++)
		{
			for(int j=0; j<mapSize.y; j++)
			{
				addTile(new Vector3(i*DEFAULT_TILE_STEP, 0, j*DEFAULT_TILE_STEP));
			}
		}
	}
	
	public void loadMap(string path)
	{
		//clear the previous map Elements
		foreach(string s in world.Keys)
		{
			removeElement(s);	
		}
		
		world.Clear();
		tiles.Clear();
		
		entityInfos = new List<Hashtable>();
		entityInfosByName = new Hashtable();
		entities = new Hashtable();
		
		skills = new Hashtable();
		items = new Hashtable();
		
		teamsInfos = new Hashtable();
		mapInfos = new Hashtable();
		
		Hashtable map = ioManager.loadMapInfos(path);
		
		mapInfos = (Hashtable)map["mapInfos"];
		skills = (Hashtable)map["skills"];
		items = (Hashtable)map["items"];
		
		teamsInfos = (Hashtable)map["teamsInfos"];
		
		entityInfosByName = (Hashtable)map["entityInfos"];
				
		foreach(string s in entityInfosByName.Keys)
		{
			entityInfos.Add((Hashtable)entityInfosByName[s]);
		}
		
		Hashtable mapData = (Hashtable)map["mapData"];
		Hashtable entitiesData = (Hashtable)map["entities"];
		
		foreach(string s in entitiesData.Keys)
		{
			Hashtable elementInfos = (Hashtable)entitiesData[s];
			addEntitySilent(new Vector3((float)elementInfos["x"], (float)elementInfos["y"], (float)elementInfos["z"]), elementInfos["name"].ToString());
		}
		
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
				
				VerticlesIndexer colorVerticlesIndexer = (tileInfos["colorTile"] as GameObject).GetComponent<VerticlesIndexer>();
				colorVerticlesIndexer.refreshIndex();
				colorVerticlesIndexer._verts = finalVertices;
				colorVerticlesIndexer.updateVertices();	
			}
		}
	}
	
	public Hashtable addTile(Vector3 position)
	{
		string id = getIdWithPosition(position, DEFAULT_TILE_STEP);
		
		if(world[id]==null)
		{
			//the color tile always comes in front.
			GameObject textureTile = (GameObject) Instantiate(defaultTextureTile, position, Quaternion.identity);
			GameObject colorTile = (GameObject) Instantiate(defaultColorTile, position+new Vector3(0, 0.01f, 0), Quaternion.identity);
			GameObject fogTile = (GameObject) Instantiate(defaultFogTile, position+new Vector3(0, 0.02f, 0), Quaternion.identity);
			
			textureTile.GetComponent<VerticlesIndexer>().Id = id;
			
			Hashtable tileInfos = new Hashtable();
			tileInfos.Add("id", id);
			tileInfos.Add("x", textureTile.transform.position.x);
			tileInfos.Add("y", textureTile.transform.position.y);
			tileInfos.Add("z", textureTile.transform.position.z);
			
			//we keep a reference to the tile's gameobjects
			tileInfos.Add("textureTile", textureTile);
			tileInfos.Add("colorTile", colorTile);
			tileInfos.Add("fogTile", fogTile);
			tileInfos.Add("verticlesIndexer", textureTile.GetComponent<VerticlesIndexer>());
			
			world.Add(id, tileInfos);
			tiles.Add(id);
			
			return tileInfos;
		}
		
		return null;
	}
	
	public void addDoodad(Vector3 position, GameObject doodad)
	{
		string id = "d_"+getIdWithPosition(position, DEFAULT_DOODAD_STEP);
		
		if(world[id]==null)
		{
			GameObject tmpDoodad = (GameObject) Instantiate(doodad, smash(position, DEFAULT_DOODAD_STEP), Quaternion.identity);
			tmpDoodad.name = id;
			
			//we need to determine to closest vertice for the 9 closest tiles and apply its height to the doodad.
			float height = 0;
			
			float lastDistance = float.MaxValue;
			
			List<string> tilesAroundMyPoint = getTilesAroundPoint(tmpDoodad.transform.position, 1);
			foreach(string s in tilesAroundMyPoint)
			{
				Hashtable tmpInfos = (Hashtable) world[s];
				
				VerticlesIndexer verticlesIndexer = (tmpInfos["textureTile"] as GameObject).GetComponent<VerticlesIndexer>();
				
				int bestVert = verticlesIndexer.getNearestPlanarVertice(tmpDoodad.transform.position);
				float planarDistance = Vector2.Distance(new Vector2(verticlesIndexer._verts[bestVert].x, verticlesIndexer._verts[bestVert].z), new Vector2(tmpDoodad.transform.position.x, tmpDoodad.transform.position.z)); 
				if(planarDistance<lastDistance)
				{
					height = verticlesIndexer._verts[bestVert].y;
				}
			}
			
			tmpDoodad.transform.position = tmpDoodad.transform.position+new Vector3(0, height, 0);
		
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
			tileInfos.Add("model", doodad.name); 
			
			world.Add(id, tileInfos);
		}
	}
	
	//behaves like a doodad, except it is stored in the entities hashtable
	public void addEntity(Vector3 position, string entity)
	{
		string id = getIdWithPosition(position, DEFAULT_DOODAD_STEP);
		
		if(entities[id]==null)
		{
			GameObject tmpDoodad = (GameObject) Instantiate(defaultEntity, smash(position, DEFAULT_DOODAD_STEP), Quaternion.identity);
			tmpDoodad.name = id;
			
			//we need to determine to closest vertice for the 9 closest tiles and apply its height to the entity.
			float height = 0;
			
			float lastDistance = float.MaxValue;
			
			List<string> tilesAroundMyPoint = getTilesAroundPoint(tmpDoodad.transform.position, 1);
			foreach(string s in tilesAroundMyPoint)
			{
				Hashtable tmpInfos = (Hashtable) world[s];
				
				VerticlesIndexer verticlesIndexer = (tmpInfos["textureTile"] as GameObject).GetComponent<VerticlesIndexer>();
				
				int bestVert = verticlesIndexer.getNearestPlanarVertice(tmpDoodad.transform.position);
				float planarDistance = Vector2.Distance(new Vector2(verticlesIndexer._verts[bestVert].x, verticlesIndexer._verts[bestVert].z), new Vector2(tmpDoodad.transform.position.x, tmpDoodad.transform.position.z)); 
				if(planarDistance<lastDistance)
				{
					height = verticlesIndexer._verts[bestVert].y;
				}
			}
			
			tmpDoodad.transform.position = tmpDoodad.transform.position+new Vector3(0, height, 0);
		
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
			tileInfos.Add("name", entity); 
			
			entities.Add(id, tileInfos);
		}
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
	
	public void addEntitySilent(Vector3 position, string entity)
	{
		string id = getIdWithPosition(position, DEFAULT_DOODAD_STEP);
		
		if(entities[id]==null)
		{
			GameObject tmpDoodad = (GameObject) Instantiate(defaultEntity, smash(position, DEFAULT_DOODAD_STEP), Quaternion.identity);
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
			tileInfos.Add("name", entity); 
			
			entities.Add(id, tileInfos);
		}
	}
	
	
	List<string> removeQueue = new List<string>();
	public void removeElementLater(string id)
	{
		removeQueue.Add(id);
	}
	
	void removeElement(string id)
	{
		if(world[id]!=null)
		{
			Hashtable elementInfos = (Hashtable)world[id];
		
			if(elementInfos["doodad"]!=null)
			{
				GameObject.Destroy(((GameObject)elementInfos["doodad"]).gameObject);
			}
			else
			{
				GameObject.Destroy(((GameObject)elementInfos["textureTile"]).gameObject);
				
				try
				{
					GameObject.Destroy(((GameObject)elementInfos["colorTile"]).gameObject);
				}
				catch
				{
					
				}
			}
			
			world.Remove(id);
		}
		else
		{
			Hashtable elementInfos = (Hashtable)entities[id];
		
			GameObject.Destroy(((GameObject)elementInfos["doodad"]).gameObject);
			entities.Remove(id);
		}
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
	
	public List<string> getDoodadsAroundPoint(Vector3 point, int radius)
	{
		List<string> tiles = new List<string>();
		//to avoid looping around every tile in the maps, we use the indexed ids
		for(int i=-radius; i<radius; i++)
		{
			for(int j=-radius; j<radius; j++)
			{
				Vector3 tmpTilePosition = point+new Vector3(i*DEFAULT_DOODAD_STEP, 0, j*DEFAULT_DOODAD_STEP);
				string tmpId = "d_"+getIdWithPosition(tmpTilePosition, DEFAULT_DOODAD_STEP);
				
				if(world[tmpId]!=null)
				{
					//tile exists, add it to the list...
					tiles.Add(tmpId);
				}
			}	
		}
		
		return tiles;
	}
	
	//same principle as VerticlesIndexer's applySmoothTranslationToVertices
	public void applySmoothTranslationToDoodads(Vector3 point, Vector3 translation, float _brushSize, float intensity, float MAX_HEIGHT)
	{
		float checkRadius = _brushSize*((float)DEFAULT_TILE_STEP)/((float)DEFAULT_DOODAD_STEP);
		
		List<string> nearestDoodads = getDoodadsAroundPoint(point, (int)checkRadius);
		foreach(string s in nearestDoodads)
		{
			Hashtable vertInfos = (Hashtable) world[s];
			Vector3 doodadPosition = (vertInfos["doodad"] as GameObject).transform.position;
			float distance = Vector3.Distance(doodadPosition, point);
			float relativeIntensity = (1-distance/brushSize)*intensity;
			
			if(relativeIntensity>intensity)
				relativeIntensity = intensity;
			
			if(relativeIntensity>0) //if i am in brush range
			{
				(vertInfos["doodad"] as GameObject).transform.position+=translation*relativeIntensity;
				
				if((vertInfos["doodad"] as GameObject).transform.position.y>MAX_HEIGHT)
					(vertInfos["doodad"] as GameObject).transform.position = new Vector3((vertInfos["doodad"] as GameObject).transform.position.x, MAX_HEIGHT, (vertInfos["doodad"] as GameObject).transform.position.z);
				
				if((vertInfos["doodad"] as GameObject).transform.position.y<-MAX_HEIGHT)
					(vertInfos["doodad"] as GameObject).transform.position = new Vector3((vertInfos["doodad"] as GameObject).transform.position.x, -MAX_HEIGHT, (vertInfos["doodad"] as GameObject).transform.position.z);
			}
		}
	}
	
	public string getIdWithPosition(Vector3 position, float defaultStep)
	{
		return smash(position, defaultStep).ToString();
	}

    public Vector3 smash(Vector3 position, float factor)
    {
        return new Vector3((float)Mathf.Round(position.x / factor) * factor, (float)Mathf.Round(position.y / factor) * factor, (float)Mathf.Round(position.z / factor) * factor);
    }

    public Vector3 smash(Vector3 position, Vector3 factorAsVector)
    {
        return new Vector3((float)Mathf.Round(position.x / factorAsVector.x) * factorAsVector.x, (float)Mathf.Round(position.y / factorAsVector.y) * factorAsVector.y, (float)Mathf.Round(position.z / factorAsVector.z) * factorAsVector.z);
    }
	
	public IEnumerator loadTexture(string url, GameObject target)
	{
		url = "file://"+ url;
		
		WWW www = new WWW(url);
		
		yield return www;
		
		target.renderer.material.mainTexture = www.texture;
	}
}

using UnityEngine;
using System.Collections;
using System;

/*
 * Simple interfaces to use the MWorld Editor's tools
 * */
public class MainInterface: MonoBehaviour 
{
	public Texture2D mainIcon;
	
	public WorldEditor worldEditor;
	
	//show credits first...
	bool credits = true;
	
	int currentDoodad = 0;
	int currentTexture = 0;
	int currentBrushType = 0;
	int currentEntity = 0;
	int currentCameraMode = 0;
	int currentGameMode = 0;
	
	string[] brushes = new string[]{ BrushType.selection.ToString(), BrushType.sculpt.ToString(), BrushType.paint.ToString(), BrushType.texturePaint.ToString(), BrushType.fog.ToString(), BrushType.doodads.ToString(), BrushType.entities.ToString(), BrushType.skills.ToString(), BrushType.items.ToString(), BrushType.map.ToString()};
	string[] brushModes = new string[]{ "Insert", "Erase"};
	
	//rpg mode includes serverside persistence
	string[] gameModes = new string[]{ "rpg", "dota", "rts"};
	
	//fps and mmo modes require an hero
	string[] cameraModes = new string[]{ "rts", "fps", "mmo"};
	
	int mapWidth = 10;
	int mapHeight = 10;
	
	bool enableFog = false;
	
	string tmpMapName = "Untitled";
	
	bool loadingInterface = false;
	
	bool createEntity = false;
	
	void OnGUI()
	{
		if(credits)
		{
			GUI.Box(new Rect(Screen.width/2-150, Screen.height/2-100, 300, 200), "MWorld Editor v1.0 Alpha");
			GUI.DrawTexture(new Rect(Screen.width/2-50, Screen.height/2-80, 100, 100), mainIcon);
			GUI.Label(new Rect(Screen.width/2-145, Screen.height/2+20, 300, 200), "Welcome to the First Alpha release of the MWorld Editor. The MWorld Editor is a powerfull tool that allows its users to manipulate most of the aspects of the 'M' powered games.");
			
			if(GUI.Button(new Rect(Screen.width/2+83, Screen.height/2+70, 60, 25),"Start"))
				credits = false;
			
			return;
		}
		
		if(worldEditor.mapName.Length==0)
		{
			if(!loadingInterface)
			{
				
				GUI.Box(new Rect(Screen.width/2-100, 100, 200, 100), "Create a new Map");
				
				tmpMapName = GUI.TextField(new Rect(Screen.width/2-90, 170, 100, 25), tmpMapName);
				if(GUI.Button(new Rect(Screen.width/2+30, 170, 60, 25), "Create"))
				{
					worldEditor.mapName = tmpMapName;
					worldEditor.drawEmptyMap(new Vector2(mapWidth, mapHeight));
				}
				
				if(GUI.Button(new Rect(Screen.width/2+30, 140, 60, 25), "Load"))
				{
					loadingInterface = true;
				}
			}
			else
			{
				GUI.Box(new Rect(Screen.width/2-100, 100, 200, 100), "Load Map");
				tmpMapName = GUI.TextField(new Rect(Screen.width/2-90, 170, 100, 25), tmpMapName);
				if(GUI.Button(new Rect(Screen.width/2+30, 170, 60, 25), "Load"))
				{
					worldEditor.mapName = tmpMapName;
					worldEditor.loadMap(Application.dataPath+"/Maps/"+worldEditor.mapName+"/mainData");
					loadingInterface = false;
				}
				
				if(GUI.Button(new Rect(Screen.width/2+30, 140, 60, 25), "Create"))
				{
					loadingInterface = false;
				}
			}
		}
		else
		{
			GUILayout.BeginArea(worldEditor.guiArea);
	        
			GUILayout.BeginHorizontal();
			
			GUILayout.Label("Map: "+worldEditor.mapName);
			
			if(GUILayout.Button("Save"))
			{
				worldEditor.ioManager.saveMap(Application.dataPath+"/Maps/"+worldEditor.mapName+"/", worldEditor.World, worldEditor.entities, worldEditor.entityInfosByName, worldEditor.teamsInfos, worldEditor.mapInfos, worldEditor.skills, worldEditor.items);
			}
			
			if(GUILayout.Button("Load"))
			{
				worldEditor.mapName = string.Empty;
				loadingInterface = true;
			}
			
			if(GUILayout.Button("New"))
			{
				worldEditor.mapName = string.Empty;
				loadingInterface = false;
			}
			
	        GUILayout.EndHorizontal();
			
			GUILayout.Label("------- "+brushes[currentBrushType]+" -------");
			
			currentBrushType = GUILayout.SelectionGrid(currentBrushType, brushes, 5);
			worldEditor.brushType = (BrushType)Enum.Parse(typeof(BrushType), brushes[currentBrushType], true);
			
			GUILayout.Label("-------");
			
			if(worldEditor.brushType==BrushType.selection)
			{
				if(worldEditor.selection==null)
				{
					GUILayout.Label("Nothing selected");
					worldEditor.selectionCube.renderer.enabled = false;
				}
				else
				{
					worldEditor.selectionCube.renderer.enabled = true;
					worldEditor.selectionCube.transform.position = worldEditor.selection.transform.position;
					GUILayout.Label("Name: "+worldEditor.selection.name);
					GUILayout.Label("position: "+worldEditor.selection.transform.position.ToString());
					GUILayout.Label("rotation: "+worldEditor.selection.transform.localEulerAngles.ToString());
					GUILayout.Label("scale: "+worldEditor.selection.transform.localScale.x);
					
						if(worldEditor.selection.GetComponent<VerticlesIndexer>())
						{
							//Terrain tiles should not be removed...
							//worldEditor.removeElementLater(worldEditor.selection.GetComponent<VerticlesIndexer>().Id);
						}
						else if(worldEditor.selection.GetComponent<Entity>())
						{
							Hashtable infos = (Hashtable) worldEditor.entities[worldEditor.selection.name];
							GUILayout.Label("entity: "+infos["name"]);
							
							GUILayout.BeginHorizontal();
							GUILayout.Label("Team: ");
							GUILayout.TextField("");
							GUILayout.EndHorizontal();
						
							if(GUILayout.Button("Destroy"))
							{
								worldEditor.removeElementLater(worldEditor.selection.name);
							}
						}
						else
						{
							if(GUILayout.Button("Destroy"))
							{
								worldEditor.removeElementLater(worldEditor.selection.name);
							}
						}
					
				}
			}
			else
				worldEditor.selectionCube.renderer.enabled = false;
			
			if(worldEditor.brushType==BrushType.sculpt)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Brush Size: " + worldEditor.brushSize.ToString(), GUILayout.Width(80));
		        worldEditor.brushSize = GUILayout.HorizontalSlider(worldEditor.brushSize, 0, 10);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Intensity: " + worldEditor.brushIntensity.ToString(), GUILayout.Width(80));
		        worldEditor.brushIntensity = GUILayout.HorizontalSlider(worldEditor.brushIntensity, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Max Height: " + worldEditor.maxHeight.ToString(), GUILayout.Width(80));
		        worldEditor.maxHeight = GUILayout.HorizontalSlider(worldEditor.maxHeight, 0, 50);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Brush Direction (x): " + worldEditor.brushDirection.x.ToString(), GUILayout.Width(80));
		        worldEditor.brushDirection.x = GUILayout.HorizontalSlider(worldEditor.brushDirection.x, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Brush Direction (y): " + worldEditor.brushDirection.y.ToString(), GUILayout.Width(80));
		        worldEditor.brushDirection.y = GUILayout.HorizontalSlider(worldEditor.brushDirection.y, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Brush Direction (z): " + worldEditor.brushDirection.z.ToString(), GUILayout.Width(80));
		        worldEditor.brushDirection.z = GUILayout.HorizontalSlider(worldEditor.brushDirection.z, -1, 1);
		        GUILayout.EndHorizontal();
			}
			
			if(worldEditor.brushType==BrushType.fog)
			{
				bool lastFog = enableFog;
				enableFog = GUILayout.Toggle(enableFog, "Enable Fog");	
				
				if(lastFog!=enableFog)
				{
					if(enableFog)
					{
						foreach(string s in worldEditor.Tiles)
						{
							Hashtable tile = (Hashtable)worldEditor.World[s];
							(tile["fogTile"] as GameObject).GetComponent<FogTileHandler>().setDefaultFog(new Color32(0, 0, 0, 255));
						}
					}
					else
					{
						foreach(string s in worldEditor.Tiles)
						{
							Hashtable tile = (Hashtable)worldEditor.World[s];
							(tile["fogTile"] as GameObject).GetComponent<FogTileHandler>().setDefaultFog(new Color32(0, 0, 0, 0));
						}
					}
				}
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Brush Size: " + worldEditor.brushSize.ToString(), GUILayout.Width(80));
		        worldEditor.brushSize = GUILayout.HorizontalSlider(worldEditor.brushSize, 0, 10);
		        GUILayout.EndHorizontal();
				
			}
			
			if(worldEditor.brushType==BrushType.paint)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Brush Size: " + worldEditor.brushSize.ToString(), GUILayout.Width(80));
		        worldEditor.brushSize = GUILayout.HorizontalSlider(worldEditor.brushSize, 0, 10);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Intensity: " + worldEditor.brushIntensity.ToString(), GUILayout.Width(80));
		        worldEditor.brushIntensity = GUILayout.HorizontalSlider(worldEditor.brushIntensity, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Max Alpha: " + worldEditor.maxTransparency.ToString(), GUILayout.Width(80));
		        worldEditor.maxTransparency = GUILayout.HorizontalSlider(worldEditor.maxTransparency, 0, 50);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Brush Color (r): " + worldEditor.brushColor.x.ToString(), GUILayout.Width(80));
		        worldEditor.brushColor.x = GUILayout.HorizontalSlider(worldEditor.brushColor.x, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Brush Color (g): " + worldEditor.brushColor.y.ToString(), GUILayout.Width(80));
		        worldEditor.brushColor.y = GUILayout.HorizontalSlider(worldEditor.brushColor.y, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Brush Color (b): " + worldEditor.brushColor.z.ToString(), GUILayout.Width(80));
		        worldEditor.brushColor.z = GUILayout.HorizontalSlider(worldEditor.brushColor.z, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Brush Color (a): " + worldEditor.brushColor.w.ToString(), GUILayout.Width(80));
		        worldEditor.brushColor.w = GUILayout.HorizontalSlider(worldEditor.brushColor.w, -1, 1);
		        GUILayout.EndHorizontal();
			}
			
			if(worldEditor.brushType==BrushType.texturePaint)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Brush Size: " + worldEditor.brushSize.ToString(), GUILayout.Width(80));
		        worldEditor.brushSize = GUILayout.HorizontalSlider(worldEditor.brushSize, 0, 10);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Intensity: " + worldEditor.brushIntensity.ToString(), GUILayout.Width(80));
		        worldEditor.brushIntensity = GUILayout.HorizontalSlider(worldEditor.brushIntensity, -1, 1);
		        GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Max Alpha: " + worldEditor.maxTransparency.ToString(), GUILayout.Width(80));
		        worldEditor.maxTransparency = GUILayout.HorizontalSlider(worldEditor.maxTransparency, 0, 50);
		        GUILayout.EndHorizontal();
				
				GUILayout.Label("Brush Textures: ");
				GUILayout.Box(worldEditor.brushTexture);
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Texture: " + worldEditor.brushTexture.name, GUILayout.Width(80));
		        currentTexture = (int)Mathf.Round(GUILayout.HorizontalSlider(currentTexture, 0, worldEditor.terrainTextures.Length-1));
		        
				worldEditor.brushTexture = worldEditor.terrainTextures[currentTexture];
				GUILayout.EndHorizontal();
			}
			
			if(worldEditor.brushType==BrushType.doodads)
			{
				
				GUILayout.Label("Brush Mode:");
				worldEditor.brushMode = GUILayout.SelectionGrid(worldEditor.brushMode, brushModes, 3);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Brush Size: " + worldEditor.brushSize.ToString(), GUILayout.Width(80));
		        worldEditor.brushSize = GUILayout.HorizontalSlider(worldEditor.brushSize, 0, 10);
		        GUILayout.EndHorizontal();
				
				GUILayout.Label("Doodads: ");
				GUILayout.BeginHorizontal();
		        GUILayout.Label("Asset: " + worldEditor.brushDoodad.name, GUILayout.Width(80));
		        currentDoodad = (int)Mathf.Round(GUILayout.HorizontalSlider(currentDoodad, 0, worldEditor.doodads.Length-1));
		        
				worldEditor.brushDoodad = worldEditor.doodads[currentDoodad];
				GUILayout.EndHorizontal();
			}
			
			if(worldEditor.brushType==BrushType.entities)
			{
				//entity's informations are defined in the editor
		        
				if(!createEntity)
				{
					if(GUILayout.Button("Create new Entity"))
						createEntity = true;	
					
					if(worldEditor.entityInfos.Count>0)
					{
						GUILayout.BeginHorizontal();
						if(GUILayout.Button("<"))
						{
							if(currentEntity>0)
								currentEntity--;
						}
						GUILayout.Label("Entity "+currentEntity);
						if(GUILayout.Button("Edit"))
						{
							Hashtable infos = worldEditor.entityInfos[currentEntity];
							newEntityName = infos["name"].ToString();
							newEntityPrefab = infos["prefab"].ToString();
							newEntityIsHero = (bool)infos["hero"];
							newEntityIsImmortal = (bool)infos["immortal"];
							newEntityMaxHp = (float)infos["maxhp"];
							newEntityHp = (float)infos["hp"];
							newEntityMaxMp = (float)infos["maxmp"];
							newEntityMp = (float)infos["mp"];
							newEntityDamage = (float)infos["damage"];
							newEntityPower = (float)infos["power"];
							newEntityArmor = (float)infos["armor"];
							newEntityResistance = (float)infos["resistance"];
							newEntitySpeed = (float)infos["speed"];
							
							createEntity = true;
						}
						
						if(GUILayout.Button(">"))
						{
							if(currentEntity<worldEditor.entityInfos.Count-1)
							{
								currentEntity++;
							}
						}
				        GUILayout.EndHorizontal();
						
						worldEditor.brushEntity = worldEditor.entityInfos[currentEntity]["name"].ToString();
					}
				}
				else
				{
					//entity creation interface...
					GUILayout.BeginHorizontal();
					GUILayout.Label("Prefab: ");
					newEntityPrefab = GUILayout.TextField(newEntityPrefab);
					
					GUILayout.Label("Name: ");
					newEntityName = GUILayout.TextField(newEntityName);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("level: ");
					newEntityLevel = float.Parse(GUILayout.TextField(newEntityLevel.ToString()));
					
					GUILayout.Label("speed: ");
					newEntitySpeed = float.Parse(GUILayout.TextField(newEntitySpeed.ToString()));
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("hp: ");
					newEntityHp = float.Parse(GUILayout.TextField(newEntityHp.ToString()));
					
					GUILayout.Label("mp: ");
					newEntityMp = float.Parse(GUILayout.TextField(newEntityMp.ToString()));
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("dmg: ");
					newEntityDamage = float.Parse(GUILayout.TextField(newEntityDamage.ToString()));
					
					GUILayout.Label("power: ");
					newEntityPower = float.Parse(GUILayout.TextField(newEntityPower.ToString()));
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("armor: ");
					newEntityArmor = float.Parse(GUILayout.TextField(newEntityArmor.ToString()));
					
					GUILayout.Label("resist: ");
					newEntityResistance = float.Parse(GUILayout.TextField(newEntityResistance.ToString()));
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					newEntityIsHero = GUILayout.Toggle(newEntityIsHero, "Hero ");	
					newEntityIsImmortal = GUILayout.Toggle(newEntityIsImmortal, "Immortal ");	
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					if(GUILayout.Button("Cancel"))
						createEntity = false;
					
					if(GUILayout.Button("Save"))
					{
						Hashtable newEntity = new Hashtable();
						newEntity.Add("name", newEntityName);
						newEntity.Add("hero", newEntityIsHero);
						newEntity.Add("immortal", newEntityIsImmortal);
						newEntity.Add("prefab", newEntityPrefab);
						newEntity.Add("maxhp", newEntityMaxHp);
						newEntity.Add("hp", newEntityHp);
						newEntity.Add("maxmp", newEntityMaxMp);
						newEntity.Add("mp", newEntityMp);
						newEntity.Add("damage", newEntityDamage);
						newEntity.Add("power", newEntityPower);
						newEntity.Add("armor", newEntityArmor);
						newEntity.Add("resistance", newEntityResistance);
						newEntity.Add("speed", newEntitySpeed);
						
						if(worldEditor.entityInfosByName[newEntityName]!=null)
						{
							worldEditor.entityInfos.Remove(worldEditor.entityInfosByName[newEntityName] as Hashtable);
							worldEditor.entityInfosByName.Remove(newEntityName);
						}
						
						worldEditor.entityInfos.Add(newEntity);
						worldEditor.entityInfosByName.Add(newEntityName, newEntity);
						
						createEntity = false;
					}
					
					GUILayout.EndHorizontal();
				}
			}
			
			if(worldEditor.brushType==BrushType.skills)
			{
				//skills informations are defined in the editor
			}
			
			if(worldEditor.brushType==BrushType.items)
			{
				//items informations are defined in the editor
			}
			
			if(worldEditor.brushType==BrushType.map)
			{
				GUILayout.Label("Map properties:");
				
				if(GUILayout.Button("Load properties"))
				{
					mapInfosFogEnabled = (bool) worldEditor.mapInfos["fog"];
					
					for(int i=0; i<gameModes.Length; i++)
					{
						if(gameModes[i].Equals(worldEditor.mapInfos["gameMode"].ToString()))
						{
							currentGameMode = i;
							break;
						}
					}
					
					for(int i=0; i<cameraModes.Length; i++)
					{
						if(gameModes[i].Equals(worldEditor.mapInfos["cameraMode"].ToString()))
						{
							currentCameraMode = i;
							break;
						}
					}
					
					for(int i=0; i<12; i++)
					{
						mapInfosTeamsNames[i] = worldEditor.mapInfos["team_"+i].ToString();
						mapInfosTeamsPlayers[i] = (int)worldEditor.mapInfos["team_"+i+"_players"];
					}
				}
				
				if(GUILayout.Button("Save properties"))
				{
					Hashtable mapInfos = new Hashtable();
					mapInfos.Add("fog", mapInfosFogEnabled);
					mapInfos.Add("gameMode", gameModes[currentGameMode]);
					mapInfos.Add("cameraMode", cameraModes[currentCameraMode]);
					
					for(int i=0; i<12; i++)
					{
						mapInfos.Add("team_"+i, mapInfosTeamsNames[i]);
						mapInfos.Add("team_"+i+"_players", mapInfosTeamsPlayers[i]);
					}
				}
				
				mapInfosFogEnabled = GUILayout.Toggle(mapInfosFogEnabled, "Enable Fog ");	
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Game mode: ");
				currentGameMode = GUILayout.SelectionGrid(currentGameMode, gameModes, 5);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Camera mode: ");
				currentCameraMode = GUILayout.SelectionGrid(currentCameraMode, cameraModes, 5);
				GUILayout.EndHorizontal();
				
				for(int i=0; i<12; i++)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("team "+i+", players: ");
					mapInfosTeamsPlayers[i] = int.Parse(GUILayout.TextField(mapInfosTeamsPlayers[i]+""));
					
					GUILayout.Label("name: ");
					mapInfosTeamsNames[i] = GUILayout.TextField(mapInfosTeamsNames[i]+"");
					GUILayout.EndHorizontal();
				}
			}
			
			GUILayout.EndArea();	
		}
	}
	
	//temp map infos are stored here
	bool mapInfosFogEnabled = false;
	int[] mapInfosTeamsPlayers = new int[12];
	string[] mapInfosTeamsNames = new string[12];
	
	//we temporarly store the new unit's stats
	string newEntityPrefab = "";
	string newEntityName = "";
	float newEntityLevel = 0;
	float newEntityHp = 0;
	float newEntityMaxHp = 0;
	float newEntityMp = 0;
	float newEntityMaxMp = 0;
	float newEntityDamage = 0;
	float newEntityPower = 0;
	float newEntityArmor = 0;
	float newEntityResistance = 0;
	float newEntitySpeed = 0;
	bool newEntityIsHero = false;
	bool newEntityIsImmortal = false;
}

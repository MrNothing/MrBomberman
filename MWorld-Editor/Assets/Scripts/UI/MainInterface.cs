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
	
	string[] brushes = new string[]{ BrushType.selection.ToString(), BrushType.sculpt.ToString(), BrushType.paint.ToString(), BrushType.texturePaint.ToString(), BrushType.doodads.ToString()};
	string[] brushModes = new string[]{ "Insert", "Erase"};
	
	int mapWidth = 10;
	int mapHeight = 10;
	
	string tmpMapName = "Untitled";
	
	bool loadingInterface = false;
	
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
				worldEditor.ioManager.saveMap(Application.dataPath+"/Maps/"+worldEditor.mapName+"/", worldEditor.World);
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
			
			GUILayout.Label("-------");
			
			currentBrushType = GUILayout.SelectionGrid(currentBrushType, brushes, 3);
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
			
			GUILayout.EndArea();	
		}
	}
}

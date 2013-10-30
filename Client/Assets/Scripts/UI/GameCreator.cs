using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCreator : MonoBehaviour 
{
	public UICore core;
	
	bool _visible = false;

	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			mapListLabel.Visible = value;
			gameName.Visible = value;
			createGame.Visible = value;
			gameNameInfos.Visible = value;
			cancel.Visible = value;
			//mapName.Visible = value;
			
			foreach(MGUIButton b in maps)
				b.Visible = value;
			
			_visible = value;
		}
	}
	
	MGUIText mapName;
	MGUIText gameNameInfos;
	MGUITextfield gameName;
	MGUIText mapListLabel;
	MGUIButton createGame;
	MGUIButton cancel;
	
	List<MGUIButton> maps = new List<MGUIButton>();
	
	string selectedMap = string.Empty;
	
	// Use this for initialization
	void Start () 
	{
		
		gameNameInfos = (MGUIText) core.gui.setText("gameNameInfos", new Rect(-12, 13, 10, 1.5f), "Game Name", core.normalFont, Color.white);
		gameName = (MGUITextfield) core.gui.setTextField("gameNameb1", new Rect(-12, 10, 10, 1.5f), Vector2.zero, "newGame_"+Random.Range(0, 1000000), core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		createGame = (MGUIButton) core.gui.setButton("createGameb1", new Rect(-15, -10, 8, 1.5f), Vector2.zero, "Create Game", core.normalFont, Color.green, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		createGame.OnButtonPressed += new MGUIButton.ButtonPressed(createGamePressed);
		cancel = (MGUIButton) core.gui.setButton("cancelGameCreation", new Rect(-2, -10, 5, 1.5f), Vector2.zero, "Cancel", core.normalFont, Color.red, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		cancel.OnButtonPressed += new MGUIButton.ButtonPressed(cancelPressed);
		mapListLabel = (MGUIText) core.gui.setText("mapListLabel", new Rect(8, 14, 5, 1.5f), "Maps", core.normalFont, Color.white);
		
		core.io.createPathIfItDoesNotExist(Application.dataPath+"/Maps/");
		string[] files = core.io.getAllFoldersNamesInFolder(Application.dataPath+"/Maps/");
		
		for(int i=0; i<files.Length; i++)
		{
			maps.Add((MGUIButton)core.gui.setButton("b"+i, new Rect(10, 10-i*2.5f, 5, 1.5f), new Vector2(0, 0), files[i].Substring(files[i].LastIndexOf("/")+1), core.normalFont, Color.white, core.ButtonNormal, core.ButtonNormal, core.ButtonHover));
			maps[i].moveText(new Vector2(-1.5f, 0));
			maps[i].OnButtonPressed += new MGUIButton.ButtonPressed(onMapButtonPressed);
		}
		
		if(maps.Count>0)
			selectedMap = maps[0].Text;
		
		Visible = false;
	}
	
	void onMapButtonPressed(MGUIButton buttonPressed)
	{
		print("map selected: "+buttonPressed.Text);
		selectedMap = buttonPressed.Text;
	}
	
	void createGamePressed(MGUIButton buttonPressed)
	{
		if(selectedMap.Length>0)
		{
			Visible = false;
			core.errorInterface.showMessage("Creating game...", Color.cyan, false);
			
			Hashtable data = new Hashtable();
			data.Add("name", gameName.Text);
			data.Add("map", selectedMap);
			data.Add("isPrivate", false);
			
			core.networkManager.send(ServerEventType.createGame, HashMapSerializer.hashMapToData(data));
		}
		else
		{
			core.errorInterface.showMessage("You must select a map!", Color.red, true);
		}
	}
	
	void cancelPressed(MGUIButton buttonPressed)
	{
		Visible = false;
		core.lobby.Visible = true;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamesList : MonoBehaviour 
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
			if(value)
				core.networkManager.send(ServerEventType.gamesList, string.Empty);
			
			cancelJoinGameButton.Visible = value;
			
			foreach(MGUIButton b in gamesListBg)
				b.Visible = value;
			
			foreach(MGUIButton b in gamesListJoinB)
				b.Visible = value;
			
			_visible = value;
		}
	}
	
	MGUIButton cancelJoinGameButton;
	List<MGUIButton> gamesListBg = new List<MGUIButton>();
	List<MGUIButton> gamesListJoinB = new List<MGUIButton>();
	
	// Use this for initialization
	void Start () 
	{
		cancelJoinGameButton = (MGUIButton) core.gui.setButton("cancelJoinGameButton", new Rect(-15, -15, 6, 1.5f), Vector2.zero, "Cancel", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover); 
		cancelJoinGameButton.OnButtonPressed+=new MGUIButton.ButtonPressed(cancelPressed);
		
		Visible = false;
	}
	
	public void showGames(Hashtable infos)
	{
		foreach(MGUIButton b in gamesListBg)
			core.gui.removeElement(b.id);
			
		foreach(MGUIButton b in gamesListJoinB)
			core.gui.removeElement(b.id);
		
		gamesListBg.Clear();
		gamesListJoinB.Clear();
		
		int counter = 0;
		foreach(string s in infos.Keys)
		{
			Hashtable channel = (Hashtable) infos[s];
			MGUIButton but1 = (MGUIButton)core.gui.setButton("gameRoom_"+s, new Rect(-10, 10-counter*2, 30, 2), Vector2.zero, channel["name"]+" ["+channel["players"]+"/"+channel["maxPlayers"]+"]", core.normalFont, Color.white, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg);
			but1.setDepth(1);
			but1.custom = channel["name"];
			but1.OnButtonPressed+=new MGUIButton.ButtonPressed(joinGamePressed);
			gamesListBg.Add(but1);
			
			MGUIButton but2 = (MGUIButton)core.gui.setButton("gameRoom_"+s+"_b", new Rect(20, 10-counter*2, 5, 1.5f), Vector2.zero, "Join", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
			but2.custom = channel["name"];
			but2.OnButtonPressed+=new MGUIButton.ButtonPressed(joinGamePressed);
			gamesListJoinB.Add(but2);
			
			counter++;
		}
	}
	
	void joinGamePressed(MGUIButton button)
	{
		Hashtable roomRequest = new Hashtable();
		roomRequest.Add("name", button.custom);
		
		core.networkManager.send(ServerEventType.playerJoin, HashMapSerializer.hashMapToData(roomRequest));
		Visible = false;
		core.errorInterface.showMessage("Joining Game...", Color.cyan, false);
	}
	
	void cancelPressed(MGUIButton button)
	{
		Visible = false;
		core.lobby.Visible = true;
	}
}

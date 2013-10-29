using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLobby : MonoBehaviour {
	
	public UICore core;
	
	bool _visible = false;
	
	public string map="Error";
	
	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			launchGame.Visible = value;
			textArea.Visible = value;
			textField.Visible = value;
			
			foreach(List<MGUIButton> team in teams)
			{
				foreach(MGUIButton button in team)
				{
					button.Visible = value;
				}
			}
			
			foreach(MGUIButton button in teamLabels)
				button.Visible = value;
			
			enabled = value;
			_visible = value;
		}
	}
	
	public MGUITextArea textArea;
	MGUITextfield textField;
	
	MGUIButton launchGame;
	List<MGUIButton>[] teams = new List<MGUIButton>[12];
	List<MGUIButton> teamLabels = new List<MGUIButton>();
	
	// Use this for initialization
	void Start () {
		for(int i=0; i<teams.Length; i++)
			teams[i] = new List<MGUIButton>();
		
		for(int i=0; i<12; i++)
		{
			teamLabels.Add((MGUIButton)core.gui.setButton("teamLabel_"+i, new Rect(0, 0, 10, 1.5f), Vector2.zero, "Team "+(i+1), core.normalFont, Color.white, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg));
			teamLabels[i].OnButtonPressed += new MGUIButton.ButtonPressed(onTeamPressed);
			teamLabels[i].custom = i.ToString();
		}
		
		launchGame = (MGUIButton)core.gui.setButton("launchGameB", new Rect(10, -15, 6, 1.5f), Vector2.zero, "Start Game", core.normalFont, Color.green, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		launchGame.moveText(new Vector2(-2, 0));
		launchGame.OnButtonPressed += new MGUIButton.ButtonPressed(onStartGamePressed);
		
		string testText = "Selected map: "+map;
		textArea = (MGUITextArea) core.gui.setTextArea("gameLobbyTextArea", new Rect(-5, 17, 0, 0), 17, 30, testText, core.normalFont, Color.white);
		textField = (MGUITextfield) core.gui.setTextField("gameLobbyTextField", new Rect(5, -10, 15, 1.5f), Vector2.zero, string.Empty, core.normalFont, Color.white, core.whiteAlphaBg, core.whiteAlphaBg, core.whiteAlphaBg);
		textArea.setDepth(1);
		textField.setDepth(1);
		
		updateTeams();
		
		Visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Return))
		{
			if(textField.Text.Length>0 && !textField.Text.Equals(" "))
			{
				core.lobby.parseCommands(textField.Text, textArea);
				textField.Text = "";
			}
		}
	}
	
	void onStartGamePressed(MGUIButton button)
	{
		core.networkManager.send(ServerEventType.gameStart, string.Empty);
	}
	
	void onTeamPressed(MGUIButton button)
	{
		//request changing teams...
		core.networkManager.send(ServerEventType.playerTeam, button.custom.ToString());
	}
	
	void onPlayerPressed(MGUIButton button)
	{
		//TODO, show player interaction menu
	}
	
	public void applyTeams(Dictionary<string, int> playersByTeam)
	{
		clearTeams();
		
		foreach(string s in playersByTeam.Keys)
		{
			MGUIButton playerInfos = (MGUIButton)core.gui.setButton(s+"_tInfos", new Rect(0, -10, 15, 1.5f), Vector3.zero, s, core.normalFont, Color.cyan, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg);
			playerInfos.moveText(new Vector2(-2, 0));
			playerInfos.OnButtonPressed = new MGUIButton.ButtonPressed(onPlayerPressed);
			teams[playersByTeam[s]].Add(playerInfos);
		}
		
		updateTeams();
	}
	
	public void setPlayerToTeam(string player, int team)
	{
		//we need to find the player and remove him from his last team (if any)
		foreach(List<MGUIButton> teamList in teams)
		{
			foreach(MGUIButton button in teamList)
			{
				if(button.Text.Equals(player))
				{
					core.gui.removeElement(button.id);
					teamList.Remove(button);
					break;
				}
			}
		}
		
		MGUIButton playerInfos = (MGUIButton)core.gui.setButton(player+"_tInfos", new Rect(0, -10, 15, 1.5f), Vector3.zero, player, core.normalFont, Color.cyan, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg);
		playerInfos.moveText(new Vector2(-2, 0));
		playerInfos.OnButtonPressed = new MGUIButton.ButtonPressed(onPlayerPressed);
		teams[team].Add(playerInfos);
		
		updateTeams();
	}
	
	public void removePlayerFromTeam(string player)
	{
		//we need to find the player
		foreach(List<MGUIButton> team in teams)
		{
			foreach(MGUIButton button in team)
			{
				if(button.Text.Equals(player))
				{
					core.gui.removeElement(button.id);
					team.Remove(button);
					break;
				}
			}
		}
		
		updateTeams();
	}
	
	void clearTeams()
	{
		foreach(List<MGUIButton> team in teams)
		{
			foreach(MGUIButton button in team)
			{
				core.gui.removeElement(button.id);
			}
		}
		
		for(int i=0; i<teams.Length; i++)
			teams[i] = new List<MGUIButton>();
	}
	
	void updateTeams()
	{
		float Ypos = 17;
		for(int i=0; i<teams.Length; i++)
		{
			teamLabels[i].setPosition(-15, Ypos);
			Ypos-=2;
			
			List<MGUIButton> team = teams[i];
			for(int j=0; j<team.Count; j++)
			{
				team[j].setPosition(new Vector2(-15, Ypos));
				Ypos-=2;
			}
		}
	}
}

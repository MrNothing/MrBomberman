using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGame : MonoBehaviour {
	
	bool _visible = false;
	
	public UICore core;
	public TopDownCameraController rtsCameraController;
	public Light gamelight;
	
	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			gamelight.enabled = value;
			rtsCameraController.enabled = value;
			textField.Visible = value;
			textArea.Visible = value;
			enabled = value;
			_visible = value;
		}
	}
	
	MGUITextfield textField;
	public MGUITextArea textArea;
	
	//TODO, add an ingame interface for selected units and menu elements
	//TODO, add a minimap
	
	// Use this for initialization
	void Start () {
		string testText = "Ingame interface";
		textArea = (MGUITextArea) core.gui.setTextArea("gameChatTextArea", new Rect(-30, 5, 0, 0), 10, 30, testText, core.normalFont, Color.white);
		textField = (MGUITextfield) core.gui.setTextField("gameChatTextField", new Rect(-15, -12, 15, 1.5f), Vector2.zero, string.Empty, core.normalFont, Color.white, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg);
		textArea.setDepth(1);
		textField.setDepth(1);
		
		Visible = false;
	}
	
	bool showChat = false;
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
	
	public void applyTeams(Dictionary<string, int> playersByTeam)
	{
		/*clearTeams();
		
		foreach(string s in playersByTeam.Keys)
		{
			MGUIButton playerInfos = (MGUIButton)core.gui.setButton(s+"_tInfos", new Rect(0, -10, 15, 1.5f), Vector3.zero, s, core.normalFont, Color.cyan, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg);
			playerInfos.moveText(new Vector2(-2, 0));
			playerInfos.OnButtonPressed = new MGUIButton.ButtonPressed(onPlayerPressed);
			teams[playersByTeam[s]].Add(playerInfos);
		}
		
		updateTeams();*/
	}
}

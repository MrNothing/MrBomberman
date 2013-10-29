using UnityEngine;
using System.Collections;

public class MainLobby : MonoBehaviour {
	
	public UICore core;
	MGUI gui;
	
	bool _visible = false;

	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			button.Visible = value;
			textField.Visible = value;
			textArea.Visible = value;
			createGameButton.Visible = value;
			joinGameButton.Visible = value;
			this.enabled = value;
			_visible = value;
		}
	}
	
	MGUIButton button;
	MGUITextfield textField;
	public MGUITextArea textArea;
	MGUIButton createGameButton;
	MGUIButton joinGameButton;
	
	// Use this for initialization
	void Start () 
	{
		gui = core.gui;
		
		button = (MGUIButton)gui.setButton("but1", new Rect(0, -10, 5, 1.5f), new Vector2(0, 0), "Send", core.normalFont, Color.green, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		button.OnButtonPressed += new MGUIButton.ButtonPressed(onButton1pressed);
		
		//textfield
		textField = (MGUITextfield)gui.setTextField("textField", new Rect(-15, -10, 10, 1.5f), new Vector3(0, 0), "", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		
		//textarea
		textArea = (MGUITextArea)gui.setTextArea("textArea1", new Rect(-25, 14, 0, 0), 15, 40, "Bienvenue dans le premier prototype de bomberman utilisant MGUI", core.normalFont, Color.yellow);
		gui.insertText(textArea.id, "Type /help for a list of commands.", core.normalFont, Color.cyan);
		
		createGameButton = (MGUIButton)gui.setButton("createGame", new Rect(15, -10, 8, 1.5f), new Vector2(0, 0), "Create Game", core.normalFont, Color.green, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		createGameButton.OnButtonPressed += new MGUIButton.ButtonPressed(onCreateGameButtonPressed);
		
		joinGameButton = (MGUIButton)gui.setButton("joinGame", new Rect(15, -7, 8, 1.5f), new Vector2(0, 0), "Join Game", core.normalFont, Color.green, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		joinGameButton.OnButtonPressed += new MGUIButton.ButtonPressed(onJoinGameButtonPressed);
		
		Visible = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.Return))
		{
			if(textField.Text.Length>0 && !textField.Text.Equals(" "))
			{
				parseCommands(textField.Text, textArea);
				textField.Text = "";
			}
		}
	}
	
	void onButton1pressed(MGUIButton key)
	{
		if(textField.Text.Length>0)
		{
			parseCommands(textField.Text, textArea);
			textField.Text = "";
		}
	}
	
	void onCreateGameButtonPressed(MGUIButton key)
	{
		Visible = false;
		core.gameCreator.Visible = true;
	}
	
	void onJoinGameButtonPressed(MGUIButton key)
	{
		Visible = false;
		core.gamesList.Visible = true;
	}
	
	public void parseCommands(string command, MGUITextArea textArea)
	{
		if(command.IndexOf("/")==0)
		{
			if(command.IndexOf("/help")==0)
			{
				gui.insertText(textArea.id, "Commands:", core.normalFont, Color.cyan);
				gui.insertText(textArea.id, "/help", core.normalFont, Color.green);
				gui.insertText(textArea.id, "/login [name]", core.normalFont, Color.green);
				gui.insertText(textArea.id, "/clear", core.normalFont, Color.green);
				gui.insertText(textArea.id, "/channels", core.normalFont, Color.green);
				gui.insertText(textArea.id, "/games", core.normalFont, Color.green);
				gui.insertText(textArea.id, "/join [channel]", core.normalFont, Color.green);
				gui.insertText(textArea.id, "/leave", core.normalFont, Color.green);
				gui.insertText(textArea.id, "/w [player] [message]", core.normalFont, Color.green);
				return;
			}
			
			if(command.IndexOf("/clear")==0)
			{
				textArea.clear();
				return;
			}
			
			if(command.IndexOf("/channels")==0)
			{
				core.networkManager.send(ServerEventType.channelsList, string.Empty);
				return;
			}
			
			if(command.IndexOf("/games")==0)
			{
				core.networkManager.send(ServerEventType.gamesList, string.Empty);
				return;
			}
			
			if(command.IndexOf("/login ")==0)
			{
				command = command.Replace("/login ", "");
				core.networkManager.send(ServerEventType.login, command);
				return;
			}
			
			if(command.IndexOf("/leave")==0)
			{
				core.networkManager.send(ServerEventType.playerLeave, string.Empty);
				return;
			}
			
			if(command.IndexOf("/w ")==0)
			{
				command = command.Replace("/w ", "");
				string target = command.Substring(0,command.IndexOf(" "));
				string msg = command.Replace(target+" ", "");
				
				Hashtable message = new Hashtable();
				message.Add("target", target);
				message.Add("msg", msg);
				
				HashMapSerializer serializer = new HashMapSerializer();
				
				core.networkManager.send(ServerEventType.pm, serializer.hashMapToData(message));
				return;
			}
			
			if(command.IndexOf("/join ")==0)
			{
				command = command.Replace("/join ", "");
				
				Hashtable roomRequest = new Hashtable();
				roomRequest.Add("name", command);
				
				HashMapSerializer serializer = new HashMapSerializer();
				
				core.networkManager.send(ServerEventType.playerJoin, serializer.hashMapToData(roomRequest));
				return;
			}
			
			gui.insertText(textArea.id, "command not found!", core.normalFont, Color.red);
		}
		else
		{
			//send chat message
			core.networkManager.send(ServerEventType.chat, command);
		}
	}
	
}

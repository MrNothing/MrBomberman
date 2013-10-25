using UnityEngine;
using System.Collections;

public class ServerInterface : MonoBehaviour 
{
	Core core;
	public string command=string.Empty;
	public string logs=string.Empty;
	
	void Start()
	{
		core = GetComponent<Core>();
	}
	
	void OnGUI () 
	{
		GUI.Box(new Rect(0, 0, Screen.width, Screen.height-30), "MServer v1 Alpha");
		GUI.Label(new Rect(10, 20, Screen.width-10, Screen.height-50), logs);
		command = GUI.TextField(new Rect(0, Screen.height-27.5f, Screen.width, 25), command);
		
		//to force key detection on textfield, we need to use Event
		if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && command.Length>0)
		{
			parseCommand(command);
			command = string.Empty;
		}
	}
	
	void parseCommand(string _command)
	{
		if(_command.IndexOf("help")==0)
		{
			logs+="Commands: \n help, \n players, \n channels, \n games, \n clear \n";
			return;
		}
		
		if(_command.IndexOf("players")==0)
		{
			logs+="Connected players: "+core.players.Count+"\n";
			return;
		}
		
		if(_command.IndexOf("channels")==0)
		{
			logs+="active channels: "+core.channels.Count+"\n";
			return;
		}
		
		if(_command.IndexOf("games")==0)
		{
			logs+="active games: "+core.games.Count+"\n";
			return;
		}
		
		if(_command.IndexOf("clear")==0)
		{
			logs = string.Empty;
			return;
		}
		
		logs += "command not found: "+_command+"\n";
	}
}

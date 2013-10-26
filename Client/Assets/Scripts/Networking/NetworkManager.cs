using UnityEngine;
using System.Collections;

public enum ServerEventType
{
	//called when the player has been authenticated
	login = 0,
	
	//in game high frequency position informations
	position = 1,
	
	//simple chat messages (if the client has joined a channel)
	chat = 2, 
	
	//private messages
	pm = 3,
	
	//event called when a player joins a channel I am in
	playerJoin = 4,
	
	//event called when a player leaves a channel I am in
	playerLeave = 5,
	
	//all "heavier" messages that are specific to custom situations
	custom = 6,
	
	//server messages 
	serverMessage = 7,
	
	//channelsList
	channelsList = 8,
	
	//gamesList
	gamesList = 9,
	
	roomInfos = 10
}

public class NetworkManager : MonoBehaviour 
{
	public UICore core;
	
	HashMapSerializer serializer = new HashMapSerializer();
	
	public void Start()
	{
		
	}
	
	public void connect(string server, string port)
	{
		Network.Connect(server, int.Parse(port));
	}
	
	//this function is used to send data to the server
	public void send(ServerEventType eventType, string data)
	{
		object[] parameters = new object[]
		{
			(int)eventType, data
		};
		networkView.RPC("OnClientEvent", RPCMode.Server, parameters);
	}
	
	[RPC]
	void OnClientEvent(int eventType, string data)
	{
		
	}
	
	/*
	 * this function is used to handle data coming from the server
	 * 
	 * Every event message has an eventType, a clientId, and specific parameters
	 */
	
	[RPC]
	void OnServerEvent(int eventTypeInt, string data)
	{
		byte eventType = (byte)eventTypeInt;
		if(eventType==(byte)ServerEventType.login)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			core.errorInterface.showMessage("Logged in as: "+infos["name"]+"!", Color.green, true);
		}
		
		if(eventType==(byte)ServerEventType.serverMessage)
		{
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, data, core.normalFont, Color.red); 
			else
				core.errorInterface.showMessage(data, Color.red, true);
		}
		
		if(eventType==(byte)ServerEventType.playerJoin)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			string msg = infos["name"]+" has joined the Channel";
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.yellow); 
		}
		
		if(eventType==(byte)ServerEventType.roomInfos)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			string msg = "Joined the Channel: "+infos["name"];
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.yellow); 
		}
		
		if(eventType==(byte)ServerEventType.playerLeave)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			string msg = infos["name"]+" has left the Channel";
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.yellow); 
		}
		
		if(eventType==(byte)ServerEventType.position)
		{
			
		}
		
		if(eventType==(byte)ServerEventType.chat)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			string msg = infos["sender"]+": "+infos["msg"];
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.white); 
		}
		
		if(eventType==(byte)ServerEventType.pm)
		{
			
			Hashtable infos = serializer.dataToHashMap(data);
			
			string msg = infos["sender"]+" whispers: "+infos["msg"];
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.green);
		}
		
		if(eventType==(byte)ServerEventType.channelsList)
		{
			Hashtable infos = serializer.dataToHashMap(data);
				
			core.gui.insertText(core.lobby.textArea.id, "Channels:", core.normalFont, Color.cyan);
			
			foreach(string s in infos.Keys)
			{
				Hashtable channel = (Hashtable) infos[s];
				core.gui.insertText(core.lobby.textArea.id, channel["name"]+" ["+channel["players"]+"/"+channel["maxPlayers"]+"]", core.normalFont, Color.cyan);
			}
		}
		
		if(eventType==(byte)ServerEventType.gamesList)
		{
			Hashtable infos = serializer.dataToHashMap(data);
				
			core.gui.insertText(core.lobby.textArea.id, "Games:", core.normalFont, Color.cyan);
			
			foreach(string s in infos.Keys)
			{
				Hashtable channel = (Hashtable) infos[s];
				core.gui.insertText(core.lobby.textArea.id, channel["name"]+" ["+channel["players"]+"/"+channel["maxPlayers"]+"]", core.normalFont, Color.cyan);
			}
		}
	}
	
	void OnFailedToConnect(NetworkConnectionError error) 
	{
		core.startScreen.show();
	    Debug.Log("Could not connect to server: " + error);
	    core.errorInterface.showMessage("Connection failed: " + error, Color.red, true);
    }
	
	void OnConnectedToServer() 
	{
		core.startScreen.hide();
		core.lobby.Visible = true;
		
        Debug.Log("Connected to server");
		core.errorInterface.showMessage("Success!", Color.green, true);
    }
}

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
	custom = 6
	
	//server messages 
	serverMessage = 7
}

public class NetworkManager : MonoBehaviour 
{
	public UICore core;
	
	public void Start()
	{
		
	}
	
	public void connect(string server, string port)
	{
		Network.Connect(server, int.Parse(port));
	}
	
	//this function is used to send data to the server
	public void send(byte eventType, System.Object parameters)
	{
		object[] data = new object[]
		{
			eventType, parameters
		};
		networkView.RPC("onClientEvent", RPCMode.Server, data);
	}
	
	/*
	 * this function is used to handle data coming from the server
	 * 
	 * Every event message has an eventType, a clientId, and specific parameters
	 */
	
	[RPC]
	void OnServerEvent(System.Object parameters)
	{
		byte eventType = parameters[0];
		if(eventType==(byte)ServerEventType.login)
		{
			
		}
		
		if(eventType==(byte)ServerEventType.playerJoin)
		{
			
		}
		
		if(eventType==(byte)ServerEventType.playerLeave)
		{
			
		}
		
		if(eventType==(byte)ServerEventType.position)
		{
			
		}
		
		if(eventType==(byte)ServerEventType.chat)
		{
			
		}
		
		if(eventType==(byte)ServerEventType.pm)
		{
			
		}
		
		if(eventType==(byte)ServerEventType.custom)
		{
			
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	//roomInfos
	roomInfos = 10,
	
	//createGame
	createGame = 11,
	
	//unitInfos only works in game
	unitInfos = 12,
	
	//gameMap only works in the game lobby
	gameMap = 13,
	
	//playerTeam only works in the game lobby
	playerTeam = 14,
	
	//channelOwner
	channelOwner = 15,
	
	//startGame
	gameStart = 16,
	
	//playersByTeam
	playersByTeam = 17,
}

public class NetworkManager : MonoBehaviour 
{
	public UICore core;
	
	public string username = string.Empty;
	public int currentTeam = 0;
	
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
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			username = infos["name"].ToString();
			core.errorInterface.showMessage("Logged in as: "+infos["name"]+"!", Color.green, true);
		}
		
		if(eventType==(byte)ServerEventType.serverMessage)
		{
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, data, core.normalFont, Color.red); 
			else if(core.gameLobby.Visible)
				core.gui.insertText(core.gameLobby.textArea.id, data, core.normalFont, Color.red);
			else if(core.inGame.Visible)
				core.gui.insertText(core.inGame.textArea.id, data, core.normalFont, Color.red); 	
			else
				core.errorInterface.showMessage(data, Color.red, true);
		}
		
		if(eventType==(byte)ServerEventType.playerJoin)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			string msg = infos["name"]+" has joined the Channel";
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.yellow);
			if(core.gameLobby.Visible)
				core.gui.insertText(core.gameLobby.textArea.id, msg, core.normalFont, Color.yellow);
			if(core.inGame.Visible)
				core.gui.insertText(core.inGame.textArea.id, msg, core.normalFont, Color.yellow); 	
		}
		
		if(eventType==(byte)ServerEventType.roomInfos)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			string msg = "Joined the Channel: "+infos["name"];
			
			//game
			if(infos["type"].Equals("game"))
			{
				//show ingame interface...
				core.gamesList.Visible = false;
				core.gameLobby.Visible = false;
				core.lobby.Visible = false;
				core.inGame.Visible = true;
				core.gameManager.mapName = infos["map"].ToString();
				core.gameManager.loadMap(Application.dataPath+"/Maps/"+infos["map"].ToString()+"/mainData");
				core.inGame.textArea.clear();
				core.gui.insertText(core.inGame.textArea.id, msg, core.normalFont, Color.yellow); 	
			}
			
			//game lobby
			if(infos["type"].Equals("lobby"))
			{
				core.gamesList.Visible = false;
				core.lobby.Visible = false;
				core.gameLobby.Visible = true;
				core.inGame.Visible = false;
				
				core.errorInterface.showMessage("Success!", Color.green, true);
				
				core.gameLobby.map = infos["map"].ToString();
				core.gameLobby.textArea.clear();
				core.gui.insertText(core.gameLobby.textArea.id, msg, core.normalFont, Color.yellow); 
			}
			
			//main lobby
			if(infos["type"].Equals("chat"))
			{
				core.gameLobby.Visible = false;
				core.gamesList.Visible = false;
				core.lobby.Visible = true;
				core.inGame.Visible = false;
				
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.yellow); 	
			}
		}
		
		if(eventType==(byte)ServerEventType.playerLeave)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			string msg = infos["name"]+" has left the Channel";
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.yellow); 
			if(core.gameLobby.Visible)
			{
				core.gameLobby.removePlayerFromTeam(infos["name"].ToString());
				core.gui.insertText(core.gameLobby.textArea.id, msg, core.normalFont, Color.yellow);
			}
			if(core.inGame.Visible)
				core.gui.insertText(core.inGame.textArea.id, msg, core.normalFont, Color.yellow); 	
		}
		
		if(eventType==(byte)ServerEventType.chat)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			string msg = infos["sender"]+": "+infos["msg"];
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.white); 
			if(core.gameLobby.Visible)
				core.gui.insertText(core.gameLobby.textArea.id, msg, core.normalFont, Color.white);
			if(core.inGame.Visible)
				core.gui.insertText(core.inGame.textArea.id, msg, core.normalFont, Color.white); 	
				
		}
		
		if(eventType==(byte)ServerEventType.pm)
		{
			
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			string msg = infos["sender"]+" whispers: "+infos["msg"];
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.green);
			if(core.gameLobby.Visible)
				core.gui.insertText(core.gameLobby.textArea.id, msg, core.normalFont, Color.green);
			if(core.inGame.Visible)
				core.gui.insertText(core.inGame.textArea.id, msg, core.normalFont, Color.green); 	
				
		}
		
		if(eventType==(byte)ServerEventType.channelsList)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			if(core.lobby.Visible)
			{
				core.gui.insertText(core.lobby.textArea.id, "Channels:", core.normalFont, Color.cyan);
				
				foreach(string s in infos.Keys)
				{
					Hashtable channel = (Hashtable) infos[s];
					core.gui.insertText(core.lobby.textArea.id, channel["name"]+" ["+channel["players"]+"/"+channel["maxPlayers"]+"]", core.normalFont, Color.cyan);
				}
			}
		}
		
		if(eventType==(byte)ServerEventType.gamesList)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
				
			if(core.lobby.Visible)
			{
				core.gui.insertText(core.lobby.textArea.id, "Games:", core.normalFont, Color.cyan);
				
				foreach(string s in infos.Keys)
				{
					Hashtable channel = (Hashtable) infos[s];
					core.gui.insertText(core.lobby.textArea.id, channel["name"]+" ["+channel["players"]+"/"+channel["maxPlayers"]+"]", core.normalFont, Color.cyan);
				}
			}
		}
		
		if(eventType==(byte)ServerEventType.channelOwner)
		{
			string msg = data+" is now the owner of this channel";
			
			if(core.lobby.Visible)
				core.gui.insertText(core.lobby.textArea.id, msg, core.normalFont, Color.yellow); 
			if(core.gameLobby.Visible)
				core.gui.insertText(core.gameLobby.textArea.id, msg, core.normalFont, Color.yellow);
			if(core.inGame.Visible)
				core.gui.insertText(core.inGame.textArea.id, msg, core.normalFont, Color.yellow); 	
				
		}
		
		if(eventType==(byte)ServerEventType.playerTeam)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			if(core.gameLobby.Visible)
				core.gameLobby.setPlayerToTeam(infos["player"].ToString(), (int)infos["team"]);
			
			if(infos["player"].Equals(username))
			{
				currentTeam = (int) infos["team"];
			}
		}
		
		if(eventType==(byte)ServerEventType.playersByTeam)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			Dictionary<string, int> playerTeamPair = new Dictionary<string, int>();
			foreach(string player in infos.Keys)
			{
				playerTeamPair.Add(player, (int)infos[player]);
			}
			
			if(core.gameLobby.Visible)
				core.gameLobby.applyTeams(playerTeamPair);
			
			if(core.inGame.Visible)
				core.inGame.applyTeams(playerTeamPair);
		}
		
		if(eventType==(byte)ServerEventType.unitInfos)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			Hashtable entityInfos = (Hashtable)infos["infos"];
			
			GameObject newEntity = (GameObject) Instantiate(Resources.Load("Entities/"+entityInfos["prefab"], typeof(GameObject)), new Vector3((float)entityInfos["x"], (float)entityInfos["y"], (float)entityInfos["z"]), Quaternion.identity);
			Entity entityScript = newEntity.GetComponent<Entity>();
			entityScript.id = (int)infos["id"];
			entityScript.owner = infos["owner"].ToString();
			entityScript.team = (int)infos["team"];
			entityScript.spells = (Hashtable) infos["spells"];
			entityScript.infos = entityInfos;
			entityScript.destination = new Vector3((float)entityInfos["dx"], (float)entityInfos["dy"], (float)entityInfos["dz"]);
			core.gameManager.entities.Add(entityScript.id, entityScript);
			
		}
		
		if(eventType==(byte)ServerEventType.position)
		{
			Hashtable infos = HashMapSerializer.dataToHashMap(data);
			
			//TODO find a smoother way to set direct position.
			core.gameManager.entities[(int)infos["id"]].syncedPosition = new Vector3((float)infos["x"], (float)infos["y"], (float)infos["z"]);
			core.gameManager.entities[(int)infos["id"]].forceHighSync = Vector3.Distance(core.gameManager.entities[(int)infos["id"]].syncedPosition, core.gameManager.entities[(int)infos["id"]].transform.position)*2;
			core.gameManager.entities[(int)infos["id"]].destination = new Vector3((float)infos["dx"], (float)infos["dy"], (float)infos["dz"]);
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

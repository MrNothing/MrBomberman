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
	
	spell = 18,
	
	Zspell = 19,
	
	Tspell = 20,
	
	attack = 21,
	
	stats = 22, 
	
	hp = 23,
	
	mp = 24,
	
	//sent when my name is still recorded in an active game
	joinActiveGame = 25,
	
	declineActiveGame = 26,
}

public class Core : MonoBehaviour 
{
	
	//all connected players will be listed here.
	public Dictionary<int, Player> players = new Dictionary<int, Player>();
	
	//index players who have a username to avoid having the same name in the server
	Dictionary<string, int> playersByName = new Dictionary<string, int>();
	
	//all active channels are listed here:
	public Dictionary<int, Channel> channels = new Dictionary<int, Channel>();
	
	//index channels to avoid having the same channel name in the server
	Dictionary<string, int> channelsByName = new Dictionary<string, int>();
	
	//all non started games are listed here:
	public Dictionary<int, Lobby> games = new Dictionary<int, Lobby>();
	
	public Dictionary<int, GameRoom> startedGames = new Dictionary<int, GameRoom>();
	
	//This is used for serialization
	HashMapSerializer serializer = new HashMapSerializer();
	
	//This is used to load maps
	public IOManager io = new IOManager();
	
	//players that have been disconnected from a game they were in are stored here until the game is destroyed
	public Dictionary<string, int> disconnectedPlayersByGame = new Dictionary<string, int>();
	public Dictionary<int, List<string>> gamesByDisconnectedPlayersList = new Dictionary<int, List<string>>();
	
	SpellsManager spellManager = new SpellsManager();
	
	void Start () 
	{	
		Network.InitializeSecurity();
		Network.InitializeServer(100, 6600, true);
		
		//create the default channel
		Channel defaultChannel = new Channel(this, "Public", ChannelType.chat, 100, false);
		channels.Add(defaultChannel.Id, defaultChannel);
		channelsByName.Add(defaultChannel.Name, defaultChannel.Id);
		
		InvokeRepeating("run", 0.1f, 0.1f);
		
		/*
		GameObject origin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		origin.name = "origin";
		
		GameObject target = GameObject.Find("target");
		
		for(int i=0; i<8; i++)
		{
			GameObject satellite = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			satellite.name = i+"_satellite";
			satellite.transform.parent = origin.transform;
			
			spheres.Add(satellite);
		}*/
	}
	/*List<GameObject> spheres = new List<GameObject>();
	
	void Update()
	{
		GameObject target = GameObject.Find("target");
		GameObject origin = GameObject.Find("origin");
		float angle = Mathf.Atan2(target.transform.position.x-origin.transform.position.x, target.transform.position.z-origin.transform.position.z);
		float rayon = 1;
		
		for(int i=0; i<8; i++)
		{
			float x = Mathf.Sin(angle)*rayon;
			float y = Mathf.Cos(angle)*rayon;
			
			GameObject satellite = spheres[i];
			satellite.transform.position = new Vector3(x, 0, y);
			angle+=Mathf.PI/4;
		}
	}*/
	
	void run()
	{
		foreach(int i in startedGames.Keys)
		{
			startedGames[i].run();
		}
	}
	
	//send a message to all players in the server
	public void Send(ServerEventType type, string data)
	{
		foreach(int playerId in players.Keys)
		{
			players[playerId].Send(type, data);
		}
	}
	
	//called when a player connects to the server, the first thing we do is create a new player object and assign this player a unique session id
	void OnPlayerConnected(NetworkPlayer player) 
	{
        Player newPlayer = new Player(this, player);
		players.Add(player.GetHashCode(), newPlayer);
    }
	
	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		Network.RemoveRPCs(player);
        //Network.DestroyPlayerObjects(player);
		Player myPlayer = players[player.GetHashCode()];
		
		players.Remove(myPlayer.Id);
		
		//if i am logged
		if(myPlayer.Name.Length>0)
			playersByName.Remove(myPlayer.Name);
		
		//if my player has a channel
		if(myPlayer.Channel!=null)
		{
			if(myPlayer.Channel.Type==ChannelType.game)
			{
				disconnectedPlayersByGame.Add(myPlayer.Name, myPlayer.Channel.Id);
				
				try
				{
					gamesByDisconnectedPlayersList[myPlayer.Channel.Id].Add(myPlayer.Name);
				}
				catch
				{
					List<string> disconnectedPlayerInMyGame = new List<string>();
					disconnectedPlayerInMyGame.Add(myPlayer.Name);
					gamesByDisconnectedPlayersList.Add(myPlayer.Channel.Id, disconnectedPlayerInMyGame);
				}
			}
			
			myPlayer.Channel.removePlayer(myPlayer);
		}
    }
	
	//used for compatibility
	[RPC]
	void OnServerEvent(int eventType, string data)
	{
		
	}
	
	//used to handle client requests
	[RPC]
	void OnClientEvent(int eventTypeInt, string data, NetworkMessageInfo senderInfos)
	{
		//the type of event sent by the client
		byte eventType = (byte) eventTypeInt;
		
		//the Player object associated to this client's instance
		Player myPlayer = players[senderInfos.sender.GetHashCode()];
		
		//login requests
		if(eventType==(byte)ServerEventType.login)
		{
			string username = data;
			
			//no secure authentification for now...
			//string password = (string) data[1];
			if(myPlayer.Name.Length==0)
			{
				try
				{
					if(playersByName[username]>0)
					{
						myPlayer.Send(ServerEventType.serverMessage, "This username is already taken!");
					}
				}
				catch
				{
					myPlayer.Name = username;
					playersByName.Add(myPlayer.Name, myPlayer.Id);
				
					Hashtable playerInfos = new Hashtable();
					playerInfos.Add("name",myPlayer.Name);
					playerInfos.Add("id", myPlayer.Id);
					
					myPlayer.Send(ServerEventType.login, serializer.hashMapToData(playerInfos));	
				
					try
					{
						//i know this is messed up... if you know a better way to test if a key exists in a dictionary i'm all ears
						if(disconnectedPlayersByGame[myPlayer.Name]!=int.MaxValue)
						{
							Debug.Log("Player "+myPlayer.Name+" was in a game!!");
							myPlayer.Send(ServerEventType.joinActiveGame, string.Empty);
							myPlayer.Send(ServerEventType.serverMessage, "You were in the game "+channels[disconnectedPlayersByGame[myPlayer.Name]].Name);
						}
					}
					catch
					{
						Debug.Log("Player "+myPlayer.Name+" was not in a game...");	
					}
				}
			}
			else
			{
				myPlayer.Send(ServerEventType.serverMessage, "You are already logged in!");
			}
		}
		
		//channel leave requests
		if(eventType==(byte)ServerEventType.playerLeave)
		{
			if(myPlayer.Channel!=null)
			{
				myPlayer.Channel.removePlayer(myPlayer);
			}
			else
			{
				myPlayer.Send(ServerEventType.serverMessage, "You are not in a channel!");
			}
		}
		
		//channel join requests
		if(eventType==(byte)ServerEventType.playerJoin)
		{
			Hashtable rawChannel = serializer.dataToHashMap(data);
			if(rawChannel["id"]!=null)
			{
				int channelId = (int) rawChannel["id"];
				
				try
				{
					channels[channelId].addPlayer(myPlayer);
				}
				catch
				{
					myPlayer.Send(ServerEventType.serverMessage, "Channel not found!");
				}
			}
			else
			{
				string channelName = (string) rawChannel["name"];
				
				try
				{
					channels[channelsByName[channelName]].addPlayer(myPlayer);
				}
				catch
				{
					myPlayer.Send(ServerEventType.serverMessage, "Channel not found!");
				}
			}
		}
		
		if(eventType==(byte)ServerEventType.joinActiveGame)
		{
			try
			{
				channels[disconnectedPlayersByGame[myPlayer.Name]].addPlayer(myPlayer);
			}
			catch
			{
				
			}
		}
		
		if(eventType==(byte)ServerEventType.declineActiveGame)
		{
			try
			{
				gamesByDisconnectedPlayersList[disconnectedPlayersByGame[myPlayer.Name]].Remove(myPlayer.Name);
				channels[disconnectedPlayersByGame[myPlayer.Name]].Send(ServerEventType.serverMessage, myPlayer.Name+" has left the game!");
				disconnectedPlayersByGame.Remove(myPlayer.Name);
			}
			catch
			{
				myPlayer.Send(ServerEventType.serverMessage, "Your name was not found in the index!");
			}
		}
		
		if(eventType==(byte)ServerEventType.channelsList)
		{
			Hashtable _channels = new Hashtable();
			
			foreach(int channelId in channels.Keys)
			{
				if(!channels[channelId].IsPrivate)
				{
					Hashtable channelInfos = new Hashtable();
					channelInfos.Add("name", channels[channelId].Name);
					channelInfos.Add("id", channels[channelId].Id);
					channelInfos.Add("players", channels[channelId].getPlayersCount());
					channelInfos.Add("maxPlayers", channels[channelId].MaxPlayers);
					
					_channels.Add(channelId.ToString(),channelInfos);
				}
			}
			
			myPlayer.Send(ServerEventType.channelsList, serializer.hashMapToData(_channels));
		}
		
		if(eventType==(byte)ServerEventType.gamesList)
		{
			Hashtable _channels = new Hashtable();
			
			foreach(int channelId in games.Keys)
			{
				if(!channels[channelId].IsPrivate)
				{
					Hashtable channelInfos = new Hashtable();
					channelInfos.Add("name", channels[channelId].Name);
					channelInfos.Add("id", channels[channelId].Id);
					channelInfos.Add("players", channels[channelId].getPlayersCount());
					channelInfos.Add("maxPlayers", channels[channelId].MaxPlayers);
					
					_channels.Add(channelId.ToString(),channelInfos);
				}
			}
			
			myPlayer.Send(ServerEventType.gamesList, serializer.hashMapToData(_channels));
		}
		
		if(eventType==(byte)ServerEventType.chat)
		{
			if(myPlayer.Channel!=null)
			{
				Hashtable message = new Hashtable();
				message.Add("sender", myPlayer.Name);
				message.Add("msg", data);
				
				myPlayer.Channel.Send(ServerEventType.chat, serializer.hashMapToData(message));
			}
			else
			{
				myPlayer.Send(ServerEventType.serverMessage, "You are not in a Channel!"); 
			}
		}
		
		if(eventType==(byte)ServerEventType.pm)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			if(myPlayer.Channel!=null)
			{
				Hashtable message = new Hashtable();
				message.Add("sender", myPlayer.Name);
				message.Add("msg", infos["msg"]);
				
				try
				{
					players[playersByName[infos["target"].ToString()]].Send(ServerEventType.pm, serializer.hashMapToData(message));
				}
				catch
				{
					myPlayer.Send(ServerEventType.serverMessage, "Player not found!"); 
				}
			}
			else
			{
				myPlayer.Send(ServerEventType.serverMessage, "You are not in a Channel!"); 
			}
		}
		
		if(eventType==(byte)ServerEventType.createGame)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			bool allow = false;
			
			if(myPlayer.Name.Length>0)
			{
				if(myPlayer.Channel!=null)
				{
					if(myPlayer.Channel.Type==ChannelType.chat)
					{
						allow = true;
					}
					else
					{
						myPlayer.Send(ServerEventType.serverMessage, "You are already in a Game!");
					}
				}
				else
				{
					allow = true;
				}
			}
			else
			{
				myPlayer.Send(ServerEventType.serverMessage, "You are not logged in!");	
			}
			
			if(allow)
			{
				Lobby newGame = new Lobby(this, infos["name"].ToString(), 12, (bool)infos["isPrivate"], infos["map"].ToString());
				newGame.IsTemp = true;
				newGame.ChannelOwner = myPlayer.Id;
				
				channels.Add(newGame.Id, newGame);
				channelsByName.Add(newGame.Name, newGame.Id);
				
				games.Add(newGame.Id, newGame);
				
				newGame.addPlayer(myPlayer);
			}
		}
		
		if(eventType==(byte)ServerEventType.playerTeam)
		{
			if(myPlayer.Channel!=null)
			{
				if(myPlayer.Channel.Type==ChannelType.lobby)
				{
					((Lobby)myPlayer.Channel).setPlayerToTeam(myPlayer, int.Parse(data));
				}
				else
				{
					myPlayer.Send(ServerEventType.serverMessage, "You must be in a game lobby!");
				}
			}
			else
			{
				myPlayer.Send(ServerEventType.serverMessage, "You are not in a Game!");
			}
		}
		
		if(eventType==(byte)ServerEventType.gameStart)
		{
			if(myPlayer.Channel!=null)
			{
				if(myPlayer.Channel.Type==ChannelType.lobby)
				{
					if(myPlayer.Channel.ChannelOwner==myPlayer.Id)
					{
						//destroy the lobby and join the new game
						
						Lobby gameLobby = (Lobby)myPlayer.Channel;
						
						GameRoom newGame = new GameRoom(this, gameLobby.Name, gameLobby.MaxPlayers, gameLobby.IsPrivate, gameLobby.Map, gameLobby.GameType, gameLobby.rawMap);
						newGame.IsTemp = true;
							
						newGame.importTeams(gameLobby);
						
						List<Player> playersToMove = new List<Player>(gameLobby.Players);
						
						//kickAllPlayers implicitely destroys the lobby channel
						gameLobby.kickAllPlayers();
						
						newGame.addPlayers(playersToMove);
						
						channels.Add(newGame.Id, newGame);
						startedGames.Add(newGame.Id, newGame);
					}
					else
					{
						myPlayer.Send(ServerEventType.serverMessage, "You must be the room leader!");	
					}
				}
				else
				{
					myPlayer.Send(ServerEventType.serverMessage, "You must be in a game lobby!");
				}
			}
			else
			{
				myPlayer.Send(ServerEventType.serverMessage, "You are not in a Game!");
			}
		}
		
		if(eventType==(byte)ServerEventType.position)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			if(myPlayer.Channel!=null)
			{
				if(myPlayer.Channel.Type==ChannelType.game)
				{
					Entity myEntity = ((GameRoom)myPlayer.Channel).getEntity((int)infos["id"]);
					
					//if this is my entity and it is not already calculating a path...
					if(myEntity.Owner.Equals(myPlayer.Name))
					{
						if(myEntity.getFinalDestination().Substract(new B4.Vector3((float) infos["x"], 0, (float) infos["z"])).Magnitude()>myEntity.myGame.baseStep)
							myEntity.findPath(new Vector2((float) infos["x"], (float) infos["z"]));
					}
				}
			}
		}
		
		if(eventType==(byte)ServerEventType.spell)
		{
			Hashtable infos = serializer.dataToHashMap(data);
			
			if(myPlayer.Channel!=null)
			{
				if(myPlayer.Channel.Type==ChannelType.game)
				{
					if((infos["target"]+"").Length<=0)
						infos["target"] = "-1";
						
					Entity target = ((GameRoom)myPlayer.Channel).getEntity(int.Parse(infos["target"]+""));
					Entity author = ((GameRoom)myPlayer.Channel).getEntity((int)infos["author"]);
					
					if(author.Infos.spells.IndexOf(infos["name"].ToString()+",")>=0)
					{
						Hashtable spell = ((GameRoom)myPlayer.Channel).getSpellWithName(infos["name"].ToString());
						
						spellManager.useSpell(spell, author, target, new Vector3((float)infos["x"], 0, (float)infos["z"])); 
					}
					else
					{
						myPlayer.Send(ServerEventType.serverMessage, "Spell not found");
					}
				}
			}
		}
	}
	
	public void DestroyChannel(Channel channel)
	{
		print("destroying channel: "+channel.Name);
		
		channels.Remove(channel.Id);
		
		if(channel.Type==ChannelType.lobby)
			games.Remove(channel.Id);
		
		if(channel.Type==ChannelType.game)
			startedGames.Remove(channel.Id);
		
		if(channel.Type==ChannelType.chat || channel.Type==ChannelType.lobby)
			channelsByName.Remove(channel.Name);
		
		try
		{
			List<string> disconnectedPlayersInThisGame = gamesByDisconnectedPlayersList[channel.Id];
			foreach(string s in disconnectedPlayersInThisGame)
			{
				try
				{
					disconnectedPlayersByGame.Remove(s);
				}
				catch
				{
					
				}
			}
		}
		catch
		{
			
		}
	}
}

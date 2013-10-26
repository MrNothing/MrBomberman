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
	roomInfos = 10
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
	
	//all active games are listed here:
	public Dictionary<int, GameRoom> games = new Dictionary<int, GameRoom>();
	
	//This is used for serialization
	HashMapSerializer serializer = new HashMapSerializer();
	
	void Start () 
	{
		Network.InitializeSecurity();
		Network.InitializeServer(100, 6600, true);
		
		//create the default channel
		Channel defaultChannel = new Channel(this, "Public", ChannelType.chat, 100, false);
		channels.Add(defaultChannel.Id, defaultChannel);
		channelsByName.Add(defaultChannel.Name, defaultChannel.Id);
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
		
		//if i am logged
		if(myPlayer.Name.Length>0)
			playersByName.Remove(myPlayer.Name);
		
		//if my player has a channel
		if(myPlayer.Channel!=null)
			myPlayer.Channel.removePlayer(myPlayer);
		
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
	}
	
	public void DestroyChannel(Channel channel)
	{
		channelsByName.Remove(channel.Name);
		channels.Remove(channel.Id);
		
		if(channel.Type==ChannelType.game)
			games.Remove(channel.Id);
	}
}

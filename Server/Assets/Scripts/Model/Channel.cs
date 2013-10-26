using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ChannelType
{
	chat, lobby, game, score
}

public class Channel 
{
	int _id;

	string _name;

	List<Player> _players = new List<Player>();
	
	ChannelType _type;
	
	bool _isPrivate;
	
	bool _isTemp = false;

	int _maxPlayers;
	
	string _password;
	
	Core _core;

	public Channel(Core core, string name, ChannelType type, int maxPlayers, bool isPrivate)
	{
		_core = core;
		_id = this.GetHashCode();
		_name = name;
		_type = type;
		_maxPlayers = maxPlayers;
		_isPrivate = isPrivate;
	}
	
	public Channel(Core core, string name, ChannelType type, int maxPlayers, bool isPrivate, string password, bool isTemp)
	{
		_core = core;
		_id = this.GetHashCode();
		_name = name;
		_type = type;
		_maxPlayers = maxPlayers;
		_isPrivate = isPrivate;
		_password = password;
		_isTemp = isTemp;
	}
	
	public void addPlayer(Player player)
	{
		bool allowed;
		
		if(player.Name.Length==0)
		{
			//this player has not been authentificated, he needs a name to join a channel.
			player.Send(ServerEventType.serverMessage, "You have not logged in!");
			allowed = false;
		}
		else if(player.Channel!=null)
		{
			if(player.Channel.GetHashCode()==_id)
			{
				//this player is already in this channel (or something messed up pretty bad...)
				
				player.Send(ServerEventType.serverMessage, "You have already joined this channel!");
				allowed = false;
			}
			else
				allowed = true;
		}
		else
			allowed = true;
	
		if(allowed)
		{
			_players.Add(player);
			
			player.Channel = this;
			
			//we need to notify everyone that this player has joined and inform the player that he is in this room...
			
			//we will need the serializer
			HashMapSerializer serializer = new HashMapSerializer();
			
			//we send the player's name and id
			Hashtable playerInfos = new Hashtable();
			playerInfos.Add("name",player.Name);
			playerInfos.Add("id", player.Id);
			
			//we prepare a list of all players in the channel for the new player
			Hashtable playersList = new Hashtable();
			
			foreach(Player p in _players)
			{
				//we send the new player's informations to every players in the channel
				p.Send(ServerEventType.playerJoin, serializer.hashMapToData(playerInfos));
				
				//we store the interated player's infos
				Hashtable pInfos = new Hashtable();
				pInfos.Add("name", p.Name);
				pInfos.Add("id", p.Id);
				playersList.Add(p.Id, playersList);
			}
			
			//room infos:
			Hashtable roomInfos = new Hashtable();
			roomInfos.Add("name", Name);
			roomInfos.Add("id", Id);
			roomInfos.Add("type", Type);
			roomInfos.Add("players", playersList);
			player.Send(ServerEventType.roomInfos, serializer.hashMapToData(roomInfos));
			
		}
	}
	
	public void removePlayer(Player player)
	{
		if(_players.Count>0)
		{
			//we need to notify everyone that this player has left...
			
			//we send the player's name and id
			Hashtable playerInfos = new Hashtable();
			playerInfos.Add("name",player.Name);
			playerInfos.Add("id", player.Id);
			
			HashMapSerializer serializer = new HashMapSerializer();
			
			foreach(Player p in _players)
			{
				p.Send(ServerEventType.playerLeave, serializer.hashMapToData(playerInfos));
			}
		}
		else
		{
			if(_isTemp) //if this is a temp room, destroy it.
			{
				_core.DestroyChannel(this);
			}
		}
		
		_players.Remove(player);
		
		player.Channel = null;
		
	}
	
	//send a message to all players in the channel
	public void Send(ServerEventType type, string data)
	{
		foreach(Player p in _players)
		{
			p.Send(type, data);
		}
	}
	
	public int getPlayersCount()
	{
		return _players.Count;
	}
	
	public bool IsPrivate 
	{
		get 
		{
			return this._isPrivate;
		}
		set 
		{
			_isPrivate = value;
		}
	}

	public ChannelType Type 
	{
		get 
		{
			return this._type;
		}
		set 
		{
			_type = value;
		}
	}
	
	public string Name 
	{
		get 
		{
			return this._name;
		}
		set 
		{
			_name = value;
		}
	}
	
	public int MaxPlayers 
	{
		get 
		{
			return this._maxPlayers;
		}
		set 
		{
			_maxPlayers = value;
		}
	}
	
	public int Id 
	{
		get 
		{
			return this._id;
		}
		set 
		{
			_id = value;
		}
	}
	
	public bool IsTemp 
	{
		get 
		{
			return this._isTemp;
		}
		set 
		{
			_isTemp = value;
		}
	}	
}

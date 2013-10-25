using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ChannelType
{
	chat, lobby, game, score
}

public class Channel 
{
	string _name;

	List<Player> _players;
	
	ChannelType _type;
	
	bool _isPrivate;
	
	int _maxPlayers;

	public Channel(string name, ChannelType type, int maxPlayers, bool isPrivate)
	{
		_name = name;
		_type = type;
		_maxPlayers = maxPlayers;
		_isPrivate = isPrivate;
	}
	
	public void addPlayer(Player player)
	{
		if(player.Channel.GetHashCode()==this.GetHashCode())
		{
			//this player is already in this channel (or something messed up pretty bad...)
			
			object[] message = new object[]
			{
				"You have already joined this channel!"
			};
			
			player.Send(ServerEventType.serverMessage, message);
		}
		else
		{
			_players.Add(player);
			
			player.Channel = this;
			
			//we need to notify everyone that this player has joined...
			
			//we send the player's name and id
			object[] playerInfos = new object[]
			{
				player.Name, 
				player.Id
			};
			
			foreach(Player p in _players)
			{
				p.Send(ServerEventType.playerJoin, playerInfos);
			}
		}
	}
	
	public void removePlayer(Player player)
	{
		_players.Remove(player);
		
		player.Channel = null;
		
		//we need to notify everyone that this player has left...
		
		//we send the player's name and id
		object[] playerInfos = new object[]
		{
			player.Name, 
			player.Id
		};
		
		foreach(Player p in _players)
		{
			p.Send(ServerEventType.playerLeave, playerInfos);
		}
	}
	
	//send a message to all players in the channel
	public void Send(ServerEventType type, object[] data)
	{
		foreach(Player p in _players)
		{
			p.Send(type, data);
		}
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
}

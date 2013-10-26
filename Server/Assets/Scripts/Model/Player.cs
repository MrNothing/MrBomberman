using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player 
{
	int _id;
	
	//the nickname of the player in game
	string _name=string.Empty;
	
	//the active channel the player is in
	Channel _channel=null;
	
	//we keep a reference to the networkPlayer
	NetworkPlayer _networkPlayer;
	
	//player needs a reference to Core
	Core _core;
	
	public Player(Core core, NetworkPlayer networkPlayer)
	{
		_core = core;
		_id = networkPlayer.GetHashCode();
		_networkPlayer = networkPlayer;
	}
	
	public void Send(ServerEventType type, string data)
	{
		object[] parameters = new object[]
		{
			(int)type, data
		};
		
		_core.networkView.RPC("OnServerEvent", _networkPlayer, parameters);
	}
	
	public Channel Channel 
	{
		get 
		{
			return this._channel;
		}
		set 
		{
			_channel = value;
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

	public NetworkPlayer NetworkPlayer 
	{
		get 
		{
			return this._networkPlayer;
		}
		set 
		{
			_networkPlayer = value;
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
}

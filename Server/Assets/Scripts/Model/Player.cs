using UnityEngine;
using System.Collections;

public class Player 
{
	int _id;
	
	//the nickname of the player in game
	string _name;
	
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
	
	public void Send(ServerEventType type, object[] parameters)
	{
		object[] data = new object[]
		{
			type, parameters
		};
		
		_core.networkView.RPC("OnServerEvent", _networkPlayer, data);
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

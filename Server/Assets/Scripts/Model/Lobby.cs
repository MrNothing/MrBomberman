using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Lobby : Channel {
	
	string _map;
	GameTypes _gameType;

	List<List<string>> teams = new List<List<string>>();
	//how many players can a team handle
	Dictionary<int, int> teamPlayersPair = new Dictionary<int, int>();
	//what team is each player assigned to
	Dictionary<string, int> playerTeamPair = new Dictionary<string, int>();
	
	public Hashtable rawMap;

	public Lobby(Core core, string name, int maxPlayers, bool isPrivate, string map):base(core, name, ChannelType.lobby, maxPlayers, isPrivate)
	{
		_map = map;
		
		//map infos
		rawMap = core.io.loadMapInfos(Application.dataPath+"/Maps/"+map+"/mainData");
	
		Hashtable mapInfos = (Hashtable)rawMap["mapInfos"];
		
		try
		{
			_gameType = (GameTypes)Enum.Parse(typeof(GameTypes), mapInfos["gameMode"].ToString(), true);
		}
		catch
		{
			_gameType = GameTypes.dota;
		}
		
		MaxPlayers = 0;
		//set the teams
		for(int i=0; i<12; i++)
		{
			try
			{
				teams.Add(new List<string>());
				teamPlayersPair.Add(i, (int)mapInfos["team_"+i+"_players"]);
				MaxPlayers += (int)mapInfos["team_"+i+"_players"];
			}
			catch
			{
				teams.Add(new List<string>());
				teamPlayersPair.Add(i, 0);
				MaxPlayers += 0;
			}
		}
		
		if(MaxPlayers == 0)
		{
			teamPlayersPair[0] = 1;
			teamPlayersPair[1] = 1;
			MaxPlayers = 2;
		}
	}
	
	public void setPlayerToTeam(Player player, int team)
	{
		if(team>teams.Count-1)
		{
			player.Send(ServerEventType.serverMessage, "This team does not exist!");
			return;
		}
		
		if(teams[team].Count<teamPlayersPair[team])
		{
			try
			{
				int lastTeam = playerTeamPair[player.Name];
				teams[lastTeam].Remove(player.Name);
				playerTeamPair.Remove(player.Name);
			}
			catch
			{
				//i was not in a team, proceed...
			}
			
			playerTeamPair.Add(player.Name, team);
			teams[team].Add(player.Name);
			
			Hashtable infos = new Hashtable();
			infos.Add("player", player.Name);
			infos.Add("id", player.Id);
			infos.Add("team", team);
			
			HashMapSerializer serializer = new HashMapSerializer();
			Send(ServerEventType.playerTeam, serializer.hashMapToData(infos));
		}
		else
		{
			player.Send(ServerEventType.serverMessage, "The team is full!");
		}
	}
	
	void removePlayerFromTeams(Player player)
	{
		try
		{
			int lastTeam = playerTeamPair[player.Name];
			teams[lastTeam].Remove(player.Name);
			playerTeamPair.Remove(player.Name);
		}
		catch
		{
			//i was not in a team, proceed...
		}
	}
	
	public void onPlayerJoin(Player player)
	{
		//we need to determine if there is place in a team
		for(int i=0; i<teams.Count; i++)
		{
			List<string> team = teams[i];
			
			if(team.Count<teamPlayersPair[i])
			{
				//there is a place in this game join it
				setPlayerToTeam(player, i);
				break;
			}
		}
		
		sendPlayersByTeam(player);
	}
	
	public void onPlayerLeave(Player player)
	{
		removePlayerFromTeams(player);
	}
	
	public void sendPlayersByTeam(Player player)
	{
		Hashtable infos = new Hashtable();
		
		foreach(string s in playerTeamPair.Keys)
		{
			infos.Add(s, playerTeamPair[s]);
		}
		
		HashMapSerializer serializer = new HashMapSerializer();
		player.Send(ServerEventType.playersByTeam, serializer.hashMapToData(infos));
	}
	
	public Dictionary<string, int> PlayerTeamPair 
	{
		get 
		{
			return this.playerTeamPair;
		}
	}

	public Dictionary<int, int> TeamPlayersPair 
	{
		get 
		{
			return this.teamPlayersPair;
		}
	}

	public List<List<string>> Teams 
	{
		get 
		{
			return this.teams;
		}
	}	
	public GameTypes GameType 
	{
		get 
		{
			return this._gameType;
		}
	}

	public string Map 
	{
		get 
		{
			return this._map;
		}
	}	
}

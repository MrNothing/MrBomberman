using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameTypes
{
	rpg, dota, rts
}

public class GameRoom : Channel
{
	string _map;
	GameTypes _gameType;
	
	//walkable paths are determined with this dictionary
	public Dictionary<string, Vector3> mapVertices = new Dictionary<string, Vector3>();
	
	//all units present in the game are listed here
	Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
	
	//all entites's patterns are stored here
	Dictionary<string, EntityInfos> entitiesInfos = new Dictionary<string, EntityInfos>();
	
	//this is an indexed representation of the space entities
	public Dictionary<string, B4.ViewTile> worldSpace = new Dictionary<string, B4.ViewTile>();
	
	List<List<string>> teams = new List<List<string>>();
	//how many players can a team handle
	Dictionary<int, int> teamPlayersPair = new Dictionary<int, int>();
	//what team is each player assigned to
	Dictionary<string, int> playerTeamPair = new Dictionary<string, int>();
	
	//this is the default size of a "bloc" containing a viewTile.
	public float baseRefSize=5;
	
	//this is the default step size for the pathfinder
	public float baseStep = 0.5f;
	
	public GameRoom(Core core, string name, int maxPlayers, bool isPrivate, string map, GameTypes type):base(core, name, ChannelType.game, maxPlayers, isPrivate)
	{
		_map = map;
		_gameType = type;
		
		//Load the specific map informations
		Hashtable rawMap = core.io.loadMapInfos(Application.dataPath+"/Maps/"+map+"/mainData");
	
		Hashtable mapInfos = (Hashtable)rawMap["mapInfos"];
		
		Hashtable mapData = (Hashtable)rawMap["mapData"];
		
		Hashtable entityInfosByName = (Hashtable)rawMap["entityInfos"];
		Hashtable entitiesData = (Hashtable)rawMap["entities"];
		
		Hashtable skills = (Hashtable)rawMap["skills"];
		Hashtable items = (Hashtable)rawMap["items"];
		
		rebuildIndexedVertices(mapData);
		loadEntityInfos(entityInfosByName);
		loadEntities(entitiesData);
	}
	
	//main loop for game Rooms, this is where all the recursive game logic is handled (IA moves, stats etc)
	public void run()
	{
		
	}
	
	//a player's team cannot change during the game for now, (maybe layer? could be an interesting concept)
	public void setPlayerToTeam(Player player, int team)
	{
		playerTeamPair.Add(player.Name, team);
		teams[team].Add(player.Name);
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
		
		//when the player joins a game, he needs to get all the entities in his client as fast as possible 	
		sendAllEntities(player);
	}
	
	public void onPlayerLeave(Player player)
	{
		removePlayerFromTeams(player);
	}
	
	public void addEntity(string entity, string owner, Vector3 position, int team, bool notifyPlayers)
	{
		//we clone entityinfos to avoid unexpected stuff
		EntityInfos newEntityInfos = new EntityInfos(entitiesInfos[entity]);
		
		Entity newEntity = new Entity(this, owner, newEntityInfos, position, team);
		
		entities.Add(newEntity.Id, newEntity);
		
		newEntity.view.onMove();
		
		if(notifyPlayers)
		{
			//we notify every player the entity is in the game
			sendEntityInfos(newEntity);
		}
	}
	
	void loadEntities(Hashtable entitiesData)
	{
		foreach(string s in entitiesData.Keys)
		{
			Hashtable elementInfos = (Hashtable)entitiesData[s];
			addEntity(elementInfos["name"].ToString(), "", new Vector3((float)elementInfos["x"], (float)elementInfos["y"], (float)elementInfos["z"]), (int)elementInfos["team"], false);
		}
	}
	
	void loadEntityInfos(Hashtable infos)
	{
		entitiesInfos = new Dictionary<string, EntityInfos>();
		
		foreach(string s in infos.Keys)
		{
			entitiesInfos.Add(s, new EntityInfos((Hashtable) infos[s]));
		}
	}
	
	void sendEntityInfos(Entity entity)
	{
		Hashtable data = new Hashtable();
		data.Add("id", entity.Id);
		data.Add("owner", entity.Owner);
		data.Add("infos", entity.export());
		
		HashMapSerializer serializer = new HashMapSerializer();
		
		Send(ServerEventType.unitInfos, serializer.hashMapToData(data)); 
	}
	
	void sendEntityInfos(Entity entity, Player player)
	{
		Hashtable data = new Hashtable();
		data.Add("id", entity.Id);
		data.Add("owner", entity.Owner);
		data.Add("infos", entity.export());
		
		HashMapSerializer serializer = new HashMapSerializer();
		
		player.Send(ServerEventType.unitInfos, serializer.hashMapToData(data)); 
	}
	
	void sendAllEntities(Player player)	
	{
		foreach(int i in entities.Keys)
		{
			sendEntityInfos(entities[i], player);
		}
	}
	
	void rebuildIndexedVertices(Hashtable mapData)
	{
		mapVertices = new Dictionary<string, Vector3>();
		
		foreach(string s in mapData.Keys)
		{
			Hashtable elementInfos = (Hashtable)mapData[s];
			
			if(!s[0].Equals('d')) //if this is not a doodad...
			{
				Vector3 tilePosition = new Vector3((float)elementInfos["x"], (float)elementInfos["y"], (float)elementInfos["z"]);
				
				//we rebuild the vertices and set their position to world position.
				Hashtable vertices = (Hashtable) elementInfos["verts"];
				
				for(int i=0; i<vertices.Count; i++)
				{
					Hashtable vertInfos = (Hashtable) vertices[i];
					Vector3 worldVertice = tilePosition + new Vector3((float)vertInfos["x"], (float)vertInfos["y"], (float)vertInfos["z"]);
					try
					{
						mapVertices.Add(B4.Vector3Tools.toPosRefId(worldVertice, baseStep), worldVertice);
					}
					catch
					{
						//this vertice was already placed there, nothing to do...
					}
				}
				
			}
		}
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
	
	public void importTeams(Lobby lobby)
	{
		teams = lobby.Teams;
		playerTeamPair = lobby.PlayerTeamPair;
		teamPlayersPair = lobby.TeamPlayersPair;
	}
}

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
	public Dictionary<string, float> mapVertices = new Dictionary<string, float>();
	
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
	
	//spell infos
	public Hashtable skills;
	
	public GameRoom(Core core, string name, int maxPlayers, bool isPrivate, string map, GameTypes type, Hashtable rawMap):base(core, name, ChannelType.game, maxPlayers, isPrivate)
	{
		_map = map;
		_gameType = type;
	
		Hashtable mapInfos = (Hashtable)rawMap["mapInfos"];
		
		Hashtable mapData = (Hashtable)rawMap["mapData"];
		
		Hashtable entityInfosByName = (Hashtable)rawMap["entityInfos"];
		Hashtable entitiesData = (Hashtable)rawMap["entities"];
		
		skills = (Hashtable)rawMap["skills"];
		Hashtable items = (Hashtable)rawMap["items"];
		
		rebuildIndexedVertices(mapData);
		loadEntityInfos(entityInfosByName);
		loadEntities(entitiesData);
	}
	
	//main loop for game Rooms, this is where all the recursive game logic is handled (IA moves, stats etc)
	public void run()
	{
		foreach(int i in entities.Keys)
		{
			entities[i].run();
		}
	}
	
	public void onPlayerJoin(Player player)
	{
		sendPlayersByTeam(player);
		
		//when the player joins a game, he needs to get all the entities in his client as fast as possible 	
		sendAllEntities(player);
	}
	
	public void onPlayerLeave(Player player)
	{
		
	}
	
	public void addEntity(string entity, string owner, Vector3 position, int team, bool notifyPlayers)
	{
		//we clone entityinfos to avoid unexpected stuff
		EntityInfos newEntityInfos = new EntityInfos(entitiesInfos[entity]);
		
		Entity newEntity = new Entity(this, owner, newEntityInfos, position, team);
		
		//newEntity.controlledByPlayer = newEntityInfos.controllable;
		
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
			int tmpTeam;
			
			try
			{
				tmpTeam = (int)elementInfos["team"];	
			}
			catch
			{
				tmpTeam = 0;
			}
			
			addEntity(elementInfos["name"].ToString(), "", new Vector3((float)elementInfos["x"], (float)elementInfos["y"], (float)elementInfos["z"]), tmpTeam, false);
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
		data.Add("team", entity.team);
		data.Add("infos", entity.export());
		
		string tmpSpellString = entity.Infos.spells;
		Hashtable spells = new Hashtable();
		while(tmpSpellString.IndexOf(",")!=-1)
		{
			string tmpSpell = tmpSpellString.Substring(0, tmpSpellString.IndexOf(","));
			tmpSpellString = tmpSpellString.Substring(tmpSpell.Length+1, tmpSpellString.Length-tmpSpell.Length-1);
			spells.Add(spells, skills[spells]);
		}
		
		data.Add("spells", spells);
		
		HashMapSerializer serializer = new HashMapSerializer();
		
		Send(ServerEventType.unitInfos, serializer.hashMapToData(data)); 
	}
	
	void sendEntityInfos(Entity entity, Player player)
	{
		Hashtable data = new Hashtable();
		data.Add("id", entity.Id);
		data.Add("owner", entity.Owner);
		data.Add("team", entity.team);
		data.Add("infos", entity.export());
		
		string tmpSpellString = entity.Infos.spells;
		Hashtable spells = new Hashtable();
		while(tmpSpellString.IndexOf(",")!=-1)
		{
			string tmpSpell = tmpSpellString.Substring(0, tmpSpellString.IndexOf(","));
			tmpSpellString = tmpSpellString.Substring(tmpSpell.Length+1, tmpSpellString.Length-tmpSpell.Length-1);
			spells.Add(tmpSpell, skills[tmpSpell]);
		}
		
		data.Add("spells", spells);
		
		HashMapSerializer serializer = new HashMapSerializer();
		player.Send(ServerEventType.unitInfos, serializer.hashMapToData(data)); 
	}
	
	void sendAllEntities(Player player)	
	{
		bool foundStartUnit = false;
		foreach(int i in entities.Keys)
		{
			if(entities[i].team==playerTeamPair[player.Name] && entities[i].controlledByPlayer && !foundStartUnit)
			{
				entities[i].Owner = player.Name;
				entities[i].controlledByPlayer = false;
				foundStartUnit = true;
			}
			
			sendEntityInfos(entities[i], player);
		}
	}
	
	void rebuildIndexedVertices(Hashtable mapData)
	{
		mapVertices = new Dictionary<string, float>();
		
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
					Vector3 worldVertice2D = new Vector3(worldVertice.x, 0, worldVertice.z);
						
					try
					{
						mapVertices.Add(B4.Vector3Tools.toPosRefId(worldVertice2D, baseStep), worldVertice.y);
					}
					catch
					{
						//this vertice was already placed there, nothing to do...
					}
					
					//we set the 8 surrounding blocs to allow the pathfinder to work properly
					
					for(int k=-1; k<=1; k++)
					{
						for(int l=-1; l<=1; l++)
						{
							worldVertice = tilePosition + new Vector3((float)vertInfos["x"], (float)vertInfos["y"], (float)vertInfos["z"]);
							worldVertice2D = new Vector3(worldVertice.x+k*baseStep, 0, worldVertice.z+l*baseStep);
						
							try
							{
								mapVertices.Add(B4.Vector3Tools.toPosRefId(worldVertice2D, baseStep), worldVertice.y);
							}
							catch
							{
								//this vertice was already placed there, nothing to do...
							}
						}
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
		teams = new List<List<string>>(lobby.Teams);
		playerTeamPair = new Dictionary<string, int>(lobby.PlayerTeamPair);
		teamPlayersPair = new Dictionary<int, int>(lobby.TeamPlayersPair);
	}
	
	public void setPath(int tId, float x, float z)
	{
		entities[tId].findPath(new Vector2(x, z));
	}
	
	public string Map 
	{
		get 
		{
			return this._map;
		}
	}
}

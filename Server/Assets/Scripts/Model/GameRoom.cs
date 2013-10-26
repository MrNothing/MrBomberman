using UnityEngine;
using System.Collections;

public enum GameTypes
{
	none, bomberman, rts
}

public class GameRoom : Channel
{
	string _map;
	GameTypes _gameType;
	
	public GameRoom(Core core, string name, int maxPlayers, bool isPrivate, string map, GameTypes type):base(core, name, ChannelType.game, maxPlayers, isPrivate)
	{
		_map = map;
		_gameType = type;
	}
	
	//main loop for game Rooms, this is where all the recursive game logic is handled (IA moves, stats etc)
	public void run()
	{
		
	}
}

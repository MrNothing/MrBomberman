using UnityEngine;
using System.Collections;

public class GameRoom : Channel
{
	public GameRoom(string name, int maxPlayers, bool isPrivate):base(name, ChannelType.game, maxPlayers, isPrivate)
	{
		
	}
}

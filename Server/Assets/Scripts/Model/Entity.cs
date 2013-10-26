using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Entities include all game elements that have a position and stats 
 * */
public class Entity 
{
	
	string _id;
	int team = 0;
	
	Vector3 _position;
	List<Vector3> path = new List<Vector3>();
	
	EntityInfos _infos;
	
	GameRoom _myGame;
	
	public Entity(string id, EntityInfos infos, Vector3 position)
	{
		_id = id;
		_infos = infos;
		_position = position;
	}
	
	//main routine 
	public void run()
	{
		
	}
	
	void applyInfos()
	{
		
	}
	
	void regeneratePoints()
	{
		
	}
}

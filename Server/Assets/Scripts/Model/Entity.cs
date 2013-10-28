using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Entities include all game elements that have a position and stats 
 * */

public enum EntityStatus
{
	idle, walking, casting, autoAttacking, dead
}

public enum EntityType
{
	unit, hero, building
}

public class Entity 
{
	
	int _id;
	
	EntityType _type;

	string owner = string.Empty;
	
	//a pet will follow its master
	int master = 0;
	
	int _team = 0;

	//when should the unit attack if it is agressive
    public float viewRange = 5;
	
	#region pathfinding
	public B4.Vector3 position;
	public B4.Vector3 destination;
	
	B4.PathFinder pathfinder;
	List<B4.Vector3> paths = new List<B4.Vector3>();
	#endregion
	
	public EntityStatus status = EntityStatus.idle;
	
	#region stats
	EntityInfos _infos;
	bool immortal = false;
	#endregion
	
	GameRoom _myGame;
	
	#region entitiy awareness
	//This defines the amount of viewtiles to check
	public Vector3 checkRange = new Vector3(2,1,2);
	
	
	public B4.ViewsTilesManager view;
	
	//the unit's visible entities are stored here.
	public Hashtable visibleHeroes = new Hashtable();
	public Hashtable visibleUnits = new Hashtable();
	public Hashtable visibleEnemies = new Hashtable();
	public Hashtable visibleAllies = new Hashtable();
	#endregion
	
	public Entity(GameRoom game, string _owner, EntityInfos infos, Vector3 _position, int team)
	{
		_myGame = game;
		
		_id = GetHashCode();
		
		_type = EntityType.unit;
		
		_infos = infos;
		
		position = new B4.Vector3(_position.x, _position.y, _position.z);
		
		_team = team;
		
		owner = _owner;
		
		pathfinder = new B4.PathFinder(_myGame.mapVertices, _myGame.baseStep, 0.5f);
		
		view = new B4.ViewsTilesManager(this, position.smash(myGame.baseRefSize));
		
		//it is recommended to call onMove() once the entity has been indexed to avoid unexpected behaviour
		//view.onMove();
	}
	
	public Hashtable export()
	{
		Hashtable infos = _infos.export();
		infos.Add("type", _type);
		infos.Add("immortal", immortal);
		return infos;
	}
	
	int passiveRegenerationCounter=0;
	
	//main routine called every 100 ms
	public void run()
	{
		if(_infos.Hp>0)
		{
			if(status==EntityStatus.idle || status==EntityStatus.walking)
			{
				updateDestination();
				synchronizePosition();
			}
			
			if(passiveRegenerationCounter<=0)
			{
				applyPassiveRegeneration();
				passiveRegenerationCounter = 10;
			}
			else
				passiveRegenerationCounter --;
		}
		else
			status = EntityStatus.dead;
	}
	
	public void updateDestination()
	{
	 	if (isSynchronized() && paths.Count > 0)
        {
            destination = paths[paths.Count - 1];
            paths.RemoveAt(paths.Count - 1);
        }
	}
	
	//usually called every second
	void applyPassiveRegeneration()
	{
		if(_infos.Hp<getMaxHp())
			_infos.Hp+=getHpRegen();
		
		if(_infos.Mp<getMaxMp())
			_infos.Mp+=getMpRegen();
		
		if(_infos.Hp>getMaxHp())
			_infos.Hp=getMaxHp();
			
		if(_infos.Mp>getMaxMp())
			_infos.Mp = getMaxMp();
	}
	
	float getMaxHp()
	{
		return _infos.Stats.Hp+_infos.Bonuses.Hp;
	}
	
	float getMaxMp()
	{
		return _infos.Stats.Mp+_infos.Bonuses.Mp;
	}
	
	float getHpRegen()
	{
		return _infos.Stats.HpRegen+_infos.Bonuses.HpRegen;
	}
	
	float getMpRegen()
	{
		return _infos.Stats.MpRegen+_infos.Bonuses.MpRegen;
	}
	
	float getFrameSpeed()
	{
		return (_infos.Stats.RunSpeed+_infos.Bonuses.RunSpeed)/400f;
	}
	
	//called every 100 ms
	public void synchronizePosition()
	{
		if (!isSynchronized())
        {
            status = EntityStatus.walking;
           
            float calculatedSpeed = getFrameSpeed();

            if (position.x < destination.x)
                position.x += calculatedSpeed;
            if (position.x > destination.x)
                position.x -= calculatedSpeed;

            if (position.y < destination.y)
                position.y += calculatedSpeed;
            if (position.y > destination.y)
                position.y -= calculatedSpeed;

            if (position.z < destination.z)
                position.z += calculatedSpeed;
            if (position.z > destination.z)
                position.z -= calculatedSpeed;

            if (Math.Abs(position.z - destination.z) <= calculatedSpeed)
            {
                position.z = destination.z;
            }

            if (Math.Abs(position.x - destination.x) <= calculatedSpeed)
            {
                position.x = destination.x;
            }

            if (Math.Abs(position.y - destination.y) <= calculatedSpeed)
            {
                position.y = destination.y;
            }
        }
        else
            status = EntityStatus.idle;

	}
	
	public bool isSynchronized()
    {
        B4.Vector3 tmpPos = new B4.Vector3(position);
        //tmpPos.y = destination.y;
        return !(tmpPos.Substract(destination).Magnitude() > getFrameSpeed() && !destination.isZero());
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
	
	public string id 
	{
		get 
		{
			return this._id.ToString();
		}
		set 
		{
			_id = int.Parse(value);
		}
	}
	
	public EntityType type 
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
	
	public string Owner 
	{
		get 
		{
			return this.owner;
		}
		set 
		{
			owner = value;
		}
	}
	
	public GameRoom myGame 
	{
		get 
		{
			return this._myGame;
		}
		set 
		{
			//may not be edited
			return;
		}
	}	
	
	public int team 
	{
		get 
		{
			return this._team;
		}
		set 
		{
			//we need to refresh the view for everyone around me
			view.reset();
			_team = value;
		}
	}	
}

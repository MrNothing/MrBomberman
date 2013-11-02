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
	
	public bool controlledByPlayer = true;
	
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
	
	System.Random mainSeed = new System.Random();
	
	public Entity(GameRoom game, string _owner, EntityInfos infos, Vector3 _position, int team)
	{
		_myGame = game;
		
		_id = GetHashCode();
		
		_type = EntityType.unit;
		
		_infos = infos;
		
		position = new B4.Vector3(_position.x, _position.y, _position.z);
		destination = new B4.Vector3(position);
		
		_team = team;
		
		owner = _owner;
		
		pathfinder = new B4.PathFinder(_myGame.mapVertices, _myGame.baseStep, 0.5f);
		
		pathfinder.OnPathFound += new B4.PathFinder.PathFound(onPathFound);
		
		view = new B4.ViewsTilesManager(this, position.smash(myGame.baseRefSize));
		
		//it is recommended to call onMove() once the entity has been indexed to avoid unexpected behaviour
		//view.onMove();
	}
	
	public Hashtable export()
	{
		Hashtable infos = _infos.export();
		infos.Add("type", _type);
		infos.Add("immortal", immortal);
		infos.Add("x", position.x);
		infos.Add("y", position.y);
		infos.Add("z", position.z);
		infos.Add("dx", destination.x);
		infos.Add("dy", destination.y);
		infos.Add("dz", destination.z);
		return infos;
	}
	
	bool sendMovement = false;
	void _sendMovement()
	{
		Hashtable infos = new Hashtable();
		infos.Add("id", Id);
		infos.Add("x", position.x);
		infos.Add("y", position.y);
		infos.Add("z", position.z);
		infos.Add("dx", destination.x);
		infos.Add("dy", destination.y);
		infos.Add("dz", destination.z);
		HashMapSerializer serializer = new HashMapSerializer();
		myGame.Send(ServerEventType.position, serializer.hashMapToData(infos));
		
		sendMovement = false;
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
		
		if(sendMovement)
			_sendMovement();
	}
	
	public void updateDestination()
	{
		if (isSynchronized() && paths.Count > 0)
        {
            destination = paths[paths.Count - 1];
            paths.RemoveAt(paths.Count - 1);
			
			sendMovement = true;
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
	
	public float physicShield = 0;
    public void hitMeWithPhysic(Entity author, float dmg)
    {
        try
        {
            if (author.Id==Id)
                return;

            if (_infos.Hp > 0)
            {
                if (author.team.Equals(team))
                    return;

                bool crit = false;
                if ((mainSeed).Next(0, 100) < author.Infos.Stats.Crit + author.Infos.Bonuses.Crit)
                {
                    dmg = dmg * (150f+0) / 100f;
                    crit = true;
                }

                float armor = _infos.Stats.Armor + _infos.Bonuses.Armor;

                if (armor < 0)
                    armor = armor / 10;

                if (armor < -99)
                    armor = -99;

                float division = (armor) + (float)100;
                if (division <= 0)
                    division = 1;
                dmg = dmg * (100 / (division));

                float AbsorbedValue = physicShield;
                if (physicShield > dmg)
                {
                    physicShield = physicShield - dmg;
                }
                else
                {
                    physicShield = 0;
                }

                dmg -= AbsorbedValue;

                if (dmg < 0)
                    dmg = 0;
				
                _infos.Hp -= dmg;

                sendHps();
            }
        }
        catch(Exception e)
        {
            
        }
    }
	
	float magicShield = 0;
    public void hitMeWithMagic(Entity author, float dmg)
    {
        try
        {
            if (author.Id==Id)
                return;

            if (_infos.Hp > 0)
            {
                if (author.team.Equals(team))
                    return;

                bool crit = false;
                if ((mainSeed).Next(0, 100) < author.Infos.Stats.SpellCrit + author.Infos.Bonuses.SpellCrit)
                {
                    dmg = dmg * (150f+0) / 100f;
                    crit = true;
                }

                float armor = _infos.Stats.Resistance + _infos.Bonuses.Resistance;

                if (armor < 0)
                    armor = armor / 10;

                if (armor < -99)
                    armor = -99;

                float division = (armor) + (float)100;
                if (division <= 0)
                    division = 1;
                dmg = dmg * (100 / (division));

                float AbsorbedValue = magicShield;
                if (magicShield > dmg)
                {
                    magicShield = magicShield - dmg;
                }
                else
                {
                    magicShield = 0;
                }

                dmg -= AbsorbedValue;

                if (dmg < 0)
                    dmg = 0;
				
                _infos.Hp -= dmg;

                sendHps();
            }
        }
        catch(Exception e)
        {
            
        }
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
		return (_infos.Stats.RunSpeed+_infos.Bonuses.RunSpeed)/3000f;
	}
	
	//called every 100 ms
	public void synchronizePosition()
	{
		if (!isSynchronized())
        {
			status = EntityStatus.walking;
			
            float calculatedSpeed = getFrameSpeed();
			
			float angle = Mathf.Atan2(destination.x-position.x, destination.z-position.z);
			
			float speedx = Mathf.Sin(angle)*calculatedSpeed;
			float speedy = Mathf.Cos(angle)*calculatedSpeed;
			
			position.x += speedx;
			position.z += speedy;
			
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
	
	public bool thinking = false;
	public void findPath(Vector2 point)
	{
		pathfinder.initializeSearch(position.smash(myGame.baseStep), new B4.Vector3(point.x, position.y, point.y), 0.5f);
	}
	
	public void onPathFound(List<B4.Vector3> _paths)
	{
		paths = _paths;
		
		if(paths.Count>0)
		{
			destination = paths[paths.Count-1];
			sendMovement = true;
		}
	}
	
	public void sendStats()
	{
		HashMapSerializer serializer = new HashMapSerializer();
		Hashtable data = new Hashtable();
		data.Add("id", Id);
		data.Add("infos", _infos.export());
		_myGame.Send(ServerEventType.stats, serializer.hashMapToData(data));
	}
	
	public void sendHps()
	{
		HashMapSerializer serializer = new HashMapSerializer();
		Hashtable data = new Hashtable();
		data.Add("id", Id);
		data.Add("v", _infos.Hp);
		_myGame.Send(ServerEventType.hp, serializer.hashMapToData(data));
	}
	
	public void sendMps()
	{
		HashMapSerializer serializer = new HashMapSerializer();
		Hashtable data = new Hashtable();
		data.Add("id", Id);
		data.Add("v", _infos.Mp);
		_myGame.Send(ServerEventType.mp, serializer.hashMapToData(data));
	}
	
	public B4.Vector3 getFinalDestination()
	{
		if(paths.Count>0)
			return paths[0];
		else
			return position;
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
	
	public EntityInfos Infos
	{
		get 
		{
			return this._infos;
		}
		set 
		{
			_infos = value;
		}
	}
}

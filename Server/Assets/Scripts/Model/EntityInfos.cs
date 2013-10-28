using UnityEngine;
using System.Collections;

public class EntityInfos 
{
	string _name;
	string _prefab;
	
	int _level;
	
	float _hp=0;
	float _mp=0;

	EntityStats stats = new EntityStats();

	//if the entity has any active buffs or items
	EntityStats bonuses = new EntityStats();
	
	//if this is a hero, he will gain stats when he gains a level
	EntityStats perLevelBonuses = new EntityStats();
	
	public EntityInfos(Hashtable infos)
	{
		_name = infos["name"].ToString();
		_prefab = infos["prefab"].ToString();
		_level = (int)infos["level"];
		
		stats = new EntityStats();
		
		stats.Hp = (float) infos["maxhp"];
		stats.Mp = (float) infos["maxmp"];
		stats.Armor = (float) infos["armor"];
		stats.Damages = (float) infos["damage"];
		stats.HpRegen = (float) infos["hpregen"];
		stats.MpRegen = (float) infos["mpregen"];
		stats.Resistance = (float) infos["resistance"];
		stats.SpellPower = (float) infos["power"];
		//a loaded unit cannot be wearing items when it is instanciated
		bonuses = new EntityStats();
	}
	
	public EntityInfos(EntityInfos infos)
	{
		_name = infos._name;
		_prefab = infos._prefab;
		_level = infos._level;
		stats = new EntityStats(infos.Stats);
		bonuses = new EntityStats(infos.Bonuses);
	}
	
	public Hashtable export()
	{
		Hashtable newEntity = new Hashtable();
		newEntity.Add("name", Name);
		newEntity.Add("prefab", Prefab);
		newEntity.Add("level", Level);
		newEntity.Add("stats", stats.export());
		newEntity.Add("bonuses", bonuses.export());
		
		return newEntity;
	}
	
	

	public int Level 
	{
		get 
		{
			return this._level;
		}
		set 
		{
			_level = value;
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

	public string Prefab 
	{
		get 
		{
			return this._prefab;
		}
		set 
		{
			_prefab = value;
		}
	}

	public EntityStats Stats 
	{
		get 
		{
			return this.stats;
		}
		set 
		{
			stats = value;
		}
	}
	
	public EntityStats Bonuses
	{
		get 
		{
			return this.bonuses;
		}
		set 
		{
			bonuses = value;
		}
	}
	
	public float Hp 
	{
		get 
		{
			return this._hp;
		}
		set 
		{
			_hp = value;
		}
	}

	public float Mp 
	{
		get 
		{
			return this._mp;
		}
		set 
		{
			_mp = value;
		}
	}	
	
}

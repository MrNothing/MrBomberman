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
	
	public bool controllable = false;
	public string spells = string.Empty;
	
	public EntityInfos(Hashtable infos)
	{
		_name = infos["name"].ToString();
		_prefab = infos["prefab"].ToString();
		spells = infos["spells"].ToString();
		
		if(spells.Length>0 && spells.IndexOf(",")<0)
			spells+=",";
		
		controllable = (bool)infos["controllable"];
		
		
		try
		{
			_level = (int)infos["level"];
		}
		catch
		{
			_level = 1;
		}
		
		stats = new EntityStats();
		
		_hp = (float) infos["hp"];
		_mp = (float) infos["mp"];
		stats.Hp = (float) infos["maxhp"];
		stats.Mp = (float) infos["maxmp"];
		
		if((float)infos["maxhp"]==0)
		{
			stats.Hp = _hp;
			stats.Mp = _mp;
		}
		
		stats.Armor = (float) infos["armor"];
		stats.Damages = (float) infos["damage"];
		
		try
		{
			stats.HpRegen = (float) infos["hpregen"];
		}
		catch
		{
			stats.HpRegen = 0;
		}
		
		try
		{
			stats.MpRegen = (float) infos["mpregen"];
		}
		catch
		{
			stats.MpRegen = 0;
		}
		
		stats.Resistance = (float) infos["resistance"];
		stats.SpellPower = (float) infos["power"];
		stats.RunSpeed = (float) infos["speed"];
		
		//a loaded unit cannot be wearing items when it is instanciated
		bonuses = new EntityStats();
	}
	
	public EntityInfos(EntityInfos infos)
	{
		_name = infos._name;
		_prefab = infos._prefab;
		_level = infos._level;
		spells = infos.spells;
		_hp = infos.Hp;
		_mp = infos.Mp;
		controllable = infos.controllable;
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
		newEntity.Add("hp", _hp);
		newEntity.Add("mp", _mp);
		newEntity.Add("bonuses", bonuses.export());
		newEntity.Add("spells", spells);
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

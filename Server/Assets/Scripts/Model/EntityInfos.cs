using UnityEngine;
using System.Collections;

public class EntityInfos 
{
	string _name;
	string _prefab;
	
	int _level;
	
	float _hp;
	float _mp;
	
	float _strength;
	float _agility;
	float _intelligence;
	
	float _armor;
	float _resistance;
	float _spellPower;
	float _damages;
	
	float _runSpeed;
	//if the entity has any active buffs or items
	EntityInfos bonuses = new EntityInfos(false);
	
	public EntityInfos(EntityInfos infos)
	{
		_name = infos._name;
		_prefab = infos._prefab;
		_level = infos._level;
		_hp = infos._hp;
		_mp = infos._mp;
		_strength = infos._strength;
		_agility = infos._agility;
		_intelligence = infos._intelligence;
		_armor = infos._armor;
		_resistance = infos._resistance;
		_spellPower = infos._spellPower;
		_damages = infos._damages;
		_runSpeed = infos._runSpeed;
	}
	
	public EntityInfos(bool fillWithDebug)
	{
		if(fillWithDebug)
		{
			//fill debug values
			_name = "Undefined";
			_prefab = "test";
			
			_level = 0;
			
			_hp = 1;
			_mp = 1;
			
			_strength = 1;
			_agility = 1;
			_intelligence = 1;
			
			_armor = 0;
			_resistance = 0;
			_spellPower = 0;
			_damages = 0;
			
			_runSpeed = 200;
	
		}
		else
		{
			//fill with empty values
			_name = string.Empty;
			_prefab = string.Empty;
			
			_level = 0;
			
			_hp = 0;
			_mp = 0;
			
			_strength = 0;
			_agility = 0;
			_intelligence = 0;
			
			_armor = 0;
			_resistance = 0;
			_spellPower = 0;
			_damages = 0;
	
			_runSpeed = 0;
		}
	}
	
	public float Agility 
	{
		get 
		{
			return this._agility;
		}
		set 
		{
			_agility = value;
		}
	}

	public float Armor 
	{
		get 
		{
			return this._armor;
		}
		set 
		{
			_armor = value;
		}
	}

	public float Damages 
	{
		get 
		{
			return this._damages;
		}
		set 
		{
			_damages = value;
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

	public float Intelligence 
	{
		get 
		{
			return this._intelligence;
		}
		set 
		{
			_intelligence = value;
		}
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

	public float Resistance 
	{
		get 
		{
			return this._resistance;
		}
		set 
		{
			_resistance = value;
		}
	}

	public float RunSpeed 
	{
		get 
		{
			return this._runSpeed;
		}
		set 
		{
			_runSpeed = value;
		}
	}

	public float SpellPower 
	{
		get 
		{
			return this._spellPower;
		}
		set 
		{
			_spellPower = value;
		}
	}

	public float Strength 
	{
		get 
		{
			return this._strength;
		}
		set 
		{
			_strength = value;
		}
	}

	public EntityInfos Bonuses 
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
}

using System;
using System.Collections;

public class EntityStats
{
	float _hp=0;
	float _mp=0;
	
	float _hpRegen=0;
	float _mpRegen=0;
	
	//this wont be used as it complexifies the gameplay for no reason.
	//float _strength=0;
	//float _agility=0;
	//float _intelligence=0;
	
	float _armor=0;
	float _resistance=0;
	float _spellPower=0;
	float _damages=0;
	
	float _runSpeed=0;
	
	public EntityStats()
	{
		
	}
	
	public EntityStats(Hashtable infos)
	{
		_hp = (float)infos["hp"];
		_hpRegen = (float)infos["hpRegen"];
		_mpRegen = (float)infos["mpRegen"];
		_mp = (float)infos["mp"];
		//_strength = 0;
		//_agility = 0;
		//_intelligence = 0;
		_armor = (float)infos["armor"];
		_resistance = (float)infos["resistance"];
		_spellPower = (float)infos["power"];
		_damages = (float)infos["damage"];
		_runSpeed = (float)infos["speed"];
	}
	
	public EntityStats(EntityStats infos)
	{
		_hp = infos._hp;
		_hpRegen = infos._hpRegen;
		_mpRegen = infos._mpRegen;
		_mp = infos._mp;
		//_strength = infos._strength;
		//_agility = infos._agility;
		//_intelligence = infos._intelligence;
		_armor = infos._armor;
		_resistance = infos._resistance;
		_spellPower = infos._spellPower;
		_damages = infos._damages;
		_runSpeed = infos._runSpeed;
	}
	
	public Hashtable export()
	{
		Hashtable newEntity = new Hashtable();
		newEntity.Add("hp", Hp);
		newEntity.Add("mp", Mp);
		newEntity.Add("damage", Damages);
		newEntity.Add("power", SpellPower);
		newEntity.Add("armor", Armor);
		newEntity.Add("resistance", Resistance);
		newEntity.Add("speed", RunSpeed);
		return newEntity;
	}
	
	/*public float Agility 
	{
		get 
		{
			return this._agility;
		}
		set 
		{
			_agility = value;
		}
	}*/

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

	/*public float Intelligence 
	{
		get 
		{
			return this._intelligence;
		}
		set 
		{
			_intelligence = value;
		}
	}*/
	
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

	/*public float Strength 
	{
		get 
		{
			return this._strength;
		}
		set 
		{
			_strength = value;
		}
	}*/

	public float HpRegen 
	{
		get 
		{
			return this._hpRegen;
		}
		set 
		{
			_hpRegen = value;
		}
	}

	public float MpRegen 
	{
		get 
		{
			return this._mpRegen;
		}
		set 
		{
			_mpRegen = value;
		}
	}
}


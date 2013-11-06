using UnityEngine;
using System.Collections;

public class Spell 
{
	int _id;
	string _name;
	Texture2D _icon;
	float _currentCoolDown=0;
	float _coolDown=10;
	float _mana=0;
	string _description="No description loaded";
	int _usage = 0;
	string _type;

	public string Type 
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
	
	public float CoolDown 
	{
		get 
		{
			return this._coolDown;
		}
		set 
		{
			_coolDown = value;
		}
	}

	public float CurrentCoolDown 
	{
		get 
		{
			return this._currentCoolDown;
		}
		set 
		{
			_currentCoolDown = value;
		}
	}

	public string Description
	{
		get 
		{
			return this._description;
		}
		set 
		{
			_description = value;
		}
	}

	public Texture2D Icon 
	{
		get 
		{
			return this._icon;
		}
		set 
		{
			_icon = value;
		}
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

	public float Mana 
	{
		get 
		{
			return this._mana;
		}
		set 
		{
			_mana = value;
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

	public int Usage 
	{
		get 
		{
			return this._usage;
		}
		set 
		{
			_usage = value;
		}
	}	
	
	public Spell (string icon, string name, string description, SpellUsage spellUsage)
	{
		_icon = (Texture2D)Resources.Load("Icons/"+icon);
		_name = name;
		_description = description;
		_usage = (int)spellUsage;
		_currentCoolDown = 0;
		_coolDown = 0;
		_mana = 0;
	}
	
	public Spell(Hashtable spellInfos)
	{
		_icon = (Texture2D)Resources.Load("Icons/"+spellInfos["icon"]);
		_name = (string)spellInfos["name"];
		_description = (string)spellInfos["description"];
		try
		{
			_currentCoolDown = (float)spellInfos["cd"];
		}
		catch
		{
			_currentCoolDown=0;
		}
		_coolDown = (float)spellInfos["coolDown"];
		_mana = (float)spellInfos["mana"];
		_usage = int.Parse(spellInfos["usage"]+"");
		_type = spellInfos["type"].ToString();
	}
	
	public Hashtable export()
	{
		Hashtable infos = new Hashtable();
		infos.Add("name", _name);
		infos.Add("icon", _icon.name);
		infos.Add("description", _description);
		infos.Add("cd", _currentCoolDown);
		infos.Add("coolDown", _coolDown);
		infos.Add("mana", _mana);
		infos.Add("usage", _usage);
		return infos;
	}
}

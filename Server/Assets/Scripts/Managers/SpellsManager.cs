using UnityEngine;
using System.Collections;

public class SpellsManager {
	public void useSpell(Hashtable spell, Entity author, Entity target, Vector3 targetPoint)
	{
		if(author.Infos.Mp<(float)spell["mana"])
		{
			return;
		}
		
		if((float)spell["usage"]==0) //this is a zone spell
		{
			castZoneSpell(spell["name"].ToString(), author, targetPoint);
		}
		else
		{
			//cast target spell...
		}
	}
	
	void castZoneSpell(string spell, Entity author, Vector3 targetPoint)
	{
		if(spell.Equals("Bomb"))
		{
			Hashtable data = new Hashtable();
			data.Add("author", author.Id);
			data.Add("x", targetPoint.x);
			data.Add("z", targetPoint.z);
			data.Add("name", "Bomb");
			HashMapSerializer serializer = new HashMapSerializer();
			author.myGame.Send(ServerEventType.Zspell, serializer.hashMapToData(data)); 
		}
	}
}

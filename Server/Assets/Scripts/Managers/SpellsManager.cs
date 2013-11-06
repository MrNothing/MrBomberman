using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public enum SpellUsage
{
	zone = 0,
	target = 1,
	passive = 2,
	self = 3,
	showBuildUI = 4,
	build = 5,
	cancel = 6,
}

public class SpellsManager 
{
	public void useSpell(Hashtable spell, Entity author, Entity target, Vector3 targetPoint)
	{
		if(author.Infos.Mp<(float)spell["mana"])
		{
			return;
		}
		
		if((float)spell["usage"]==(float)SpellUsage.zone) //this is a zone spell
		{
			castZoneSpell(spell["name"].ToString(), author, targetPoint);
		}
		
		if((float)spell["usage"]==(float)SpellUsage.build)
		{
			string unitToBuild = spell["type"].ToString();
			
			Entity newBuilding = author.myGame.addEntity(author.myGame.getEntityNameWithPrefab(unitToBuild), author.Owner, targetPoint, author.team, false);
			newBuilding.beingBuilt = true;
			newBuilding.Infos.Hp = 1;
			author.myGame.sendEntityInfos(newBuilding);
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
			
			Hashtable spellObject = new Hashtable();
			spellObject.Add("type", "zoneSpell");
			spellObject.Add("author", author);
			spellObject.Add("damages", (float)100);
			spellObject.Add("targetPoint", targetPoint);
			spellObject.Add("range", (float)2);
			spellObject.Add("time", 20f);
			
			author.myGame.spellsQueue.Add(spellObject);
		}
	}
}

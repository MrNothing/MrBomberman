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
	invokeUnit = 7,
}

public class SpellsManager 
{
	public void useSpell(Hashtable spell, Entity author, Entity target, Vector3 targetPoint)
	{
		if((float)spell["usage"]!=(float)SpellUsage.build || (float)spell["usage"]!=(float)SpellUsage.invokeUnit)
		{
			try
			{
				if(author.myGame.goldByPlayer[author.Id]<(float)spell["effect1"])
				{
					if(author.Owner.Length>0)
					{
						Player myPlayer = author.getMyOwner();
						if(myPlayer!=null)
							myPlayer.Send(ServerEventType.serverMessage, "Not enough gold!");
					}
					return;
				}
				
				if(author.myGame.woodByPlayer[author.Id]<(float)spell["effect2"])
				{
					if(author.Owner.Length>0)
					{
						Player myPlayer = author.getMyOwner();
						if(myPlayer!=null)
							myPlayer.Send(ServerEventType.serverMessage, "Not enough wood!");
					}
					return;
				}
			}
			catch //effect 1 or 2 are not defined, ignore them
			{
				
			}
		}
		else
		{
			if(author.Infos.Mp<(float)spell["mana"])
			{
				if(author.Owner.Length>0)
				{
					Player myPlayer = author.getMyOwner();
					if(myPlayer!=null)
						myPlayer.Send(ServerEventType.serverMessage, "Not enough mana!");
				}
				return;
			}
		}
		
		if((float)spell["usage"]==(float)SpellUsage.zone) //this is a zone spell
		{
			castZoneSpell(spell["name"].ToString(), author, targetPoint);
		}
		
		//TODO, for these actions, money (or any other ressource) sould be checked
		
		/*
		 * invokeUnit: 
		 * params:  usage->spell usage type [float]
		 * 			type-> the prefab of the unit we want to build [string]
		 * 			effect1-> gold required [float]
		 * 			effect2-> wood required [float]
		 * */
		if((float)spell["usage"]==(float)SpellUsage.build)
		{
			string unitToBuild = spell["type"].ToString();
			
			Entity newBuilding = author.myGame.addEntity(author.myGame.getEntityNameWithPrefab(unitToBuild), author.Owner, targetPoint, author.team, false);
			newBuilding.beingBuilt = true;
			newBuilding.Infos.Hp = 1;
			author.myGame.sendEntityInfos(newBuilding);
		}
		
		/*
		 * invokeUnit: 
		 * params:  usage->spell usage type [float]
		 * 			type-> the prefab of the unit we want to invoke [string]
		 * 			mana->invoke time [float]
		 * 			effect1->gold required [float]
		 * 			effect2->wood required [float]
		 * */
		if((float)spell["usage"]==(float)SpellUsage.invokeUnit)
		{
			if(author.invokingQueue.Count<6)
			{
				Debug.Log("invoking unit: "+spell["type"].ToString());
				string unitToBuild = spell["type"].ToString();
				/*
				 * Warning! Mana is used to defined the time required to invoke the unit!
				 */
				author.invokingQueue.Add(new WaitingUnit(unitToBuild, (float)spell["mana"]));
				if(author.Owner.Length>0)
				{
					Hashtable data = new Hashtable();
					data.Add("type", "invokedUnit+");
					data.Add("unit", "unitToBuild");
					HashMapSerializer serializer = new HashMapSerializer();
					author.getMyOwner().Send(ServerEventType.custom, serializer.hashMapToData(data)); 
				}
			}
			else
			{
				if(author.Owner.Length>0)
				{
					Player myPlayer = author.getMyOwner();
					if(myPlayer!=null)
						myPlayer.Send(ServerEventType.serverMessage, "You cannot request any more units!");
				}
			}
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

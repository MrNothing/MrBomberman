using UnityEngine;
using System.Collections;

public class SpellsManager : MonoBehaviour 
{
	public Camera tCamera;
	public UICore core;
	MGUIText spellInfosMessage;
	Spell currentSpell=null;
	// Use this for initialization
	void Start () 
	{
		spellInfosMessage = (MGUIText) core.gui.setText("spellInfosMessage" , new Rect(-5, 5, 0, 0), "Select the target zone for this spell", core.normalFont, Color.yellow); 
		spellInfosMessage.Visible = false;
		enabled = false;
	}
	
	float hideMessageCounter = 0;
	void Update()	
	{
		if(Input.GetMouseButtonDown(0))
		{
			enabled = false;
			spellInfosMessage.Visible = false;
			
			if(currentSpell.Usage==0)
			{
				Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		
				RaycastHit hitFloor2;
				if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
				{
					Hashtable castInfos = new Hashtable();
					castInfos.Add("name", currentSpell.Name);
					castInfos.Add("target", string.Empty);
					castInfos.Add("author", core.inGame.selectedEntity.id);
					castInfos.Add("x", hitFloor2.point.x);
					castInfos.Add("z", hitFloor2.point.z);
					core.networkManager.send(ServerEventType.spell, HashMapSerializer.hashMapToData(castInfos));
				}
			}
			
			if(currentSpell.Usage==1)
			{
				Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		
				RaycastHit hitFloor2;
				if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
				{
					if(hitFloor2.collider.gameObject.GetComponent<Entity>())
					{
						Hashtable castInfos = new Hashtable();
						castInfos.Add("name", currentSpell.Name);
						castInfos.Add("author", core.inGame.selectedEntity.id);
						castInfos.Add("target", hitFloor2.collider.gameObject.GetComponent<Entity>().id);
						castInfos.Add("x", 0);
						castInfos.Add("z", 0);
						core.networkManager.send(ServerEventType.spell, HashMapSerializer.hashMapToData(castInfos));
					}
				}
			}
		}
		
		if(Input.GetMouseButtonDown(1))
		{
			enabled = false;
			spellInfosMessage.Visible = false;
		}
		
		if(hideMessageCounter>0)
		{
			hideMessageCounter-=Time.deltaTime;
			if(hideMessageCounter<=0)
			{
				spellInfosMessage.Visible = false;
				enabled = false;
			}
		}
	}
	
	public void activateSpell(Spell spell)
	{
		if(spell.Usage==0) //zone spell
		{
			spellInfosMessage.Text = "Select the target zone fot this spell";
		}
		
		if(spell.Usage==1)
		{
			spellInfosMessage.Text = "Select the target unit for this spell";
		}

		if(spell.Usage==2)
		{
			spellInfosMessage.Text = "This is a passive spell!";
			spellInfosMessage.Visible = true;
			hideMessageCounter = 1;
			enabled = true;
			return;
		}
		
		if(spell.Usage==3)
		{
			Hashtable castInfos = new Hashtable();
			castInfos.Add("name", spell.Name);
			castInfos.Add("target", string.Empty);
			castInfos.Add("x", 0);
			castInfos.Add("z", 0);
			core.networkManager.send(ServerEventType.spell, HashMapSerializer.hashMapToData(castInfos));
			return;
		}
		
		spellInfosMessage.Visible = true;
		currentSpell = spell;
		enabled = true;
	}
}

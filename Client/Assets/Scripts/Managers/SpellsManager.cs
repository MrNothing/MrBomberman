using UnityEngine;
using System.Collections;

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
	buyItem = 8,
}

public class SpellsManager : MonoBehaviour 
{
	public Camera tCamera;
	public UICore core;
	
	public GameObject zoneSpell3DView;
	public Material buildingMaterial;
	
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
		if(currentSpell.Usage == (int)SpellUsage.zone || currentSpell.Usage == (int)SpellUsage.target)
		{
			//attach spell range 3D interface to the selected entity
		}
		
		if(Input.GetMouseButtonDown(0))
		{
			enabled = false;
			spellInfosMessage.Visible = false;
			remove3DSpellInfo();
			
			if(currentSpell.Usage==(int)SpellUsage.zone || currentSpell.Usage==(int)SpellUsage.build)
			{
				Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		
				RaycastHit hitFloor2;
				if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
				{
					Hashtable castInfos = new Hashtable();
					castInfos.Add("name", currentSpell.Name);
					castInfos.Add("target", string.Empty);
					castInfos.Add("author", core.inGame.selectedEntities[core.inGame.activeEntity].id);
					castInfos.Add("x", hitFloor2.point.x);
					castInfos.Add("z", hitFloor2.point.z);
					core.networkManager.send(ServerEventType.spell, HashMapSerializer.hashMapToData(castInfos));
				}
			}
			
			if(currentSpell.Usage==(int)SpellUsage.target)
			{
				Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		
				RaycastHit hitFloor2;
				if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
				{
					if(hitFloor2.collider.gameObject.GetComponent<Entity>())
					{
						Hashtable castInfos = new Hashtable();
						castInfos.Add("name", currentSpell.Name);
						castInfos.Add("author", core.inGame.selectedEntities[core.inGame.activeEntity].id);
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
			remove3DSpellInfo();
		}
		
		
		if(hideMessageCounter>0)
		{
			hideMessageCounter-=Time.deltaTime;
			if(hideMessageCounter<=0)
			{
				spellInfosMessage.Visible = false;
				enabled = false;
				remove3DSpellInfo();
			}
		}
	}
	
	public void activateSpell(Spell spell)
	{
		currentSpell = spell;
		
		Debug.Log("sending spell: "+spell.Usage);
		
		if(spell.Usage==(int)SpellUsage.zone) //zone spell
		{
			spellInfosMessage.Text = "Select the target zone fot this spell";
		}
		
		if(spell.Usage==(int)SpellUsage.target)
		{
			spellInfosMessage.Text = "Select the target unit for this spell";
		}

		if(spell.Usage==(int)SpellUsage.passive)
		{
			spellInfosMessage.Text = "This is a passive spell!";
			spellInfosMessage.Visible = true;
			hideMessageCounter = 1;
			enabled = true;
			return;
		}
		
		if(spell.Usage==(int)SpellUsage.self)
		{
			Hashtable castInfos = new Hashtable();
			castInfos.Add("name", spell.Name);
			castInfos.Add("target", string.Empty);
			castInfos.Add("author", core.inGame.selectedEntities[core.inGame.activeEntity].id);
			castInfos.Add("x", 0f);
			castInfos.Add("z", 0f);
			core.networkManager.send(ServerEventType.spell, HashMapSerializer.hashMapToData(castInfos));
			return;
		}
		
		if(spell.Usage==(int)SpellUsage.build)
		{
			spellInfosMessage.Text = "Select the zone you want to build in";
			insert3DBuildInfo();
		}
		
		if(spell.Usage == (int)SpellUsage.invokeUnit)
		{
			Hashtable castInfos = new Hashtable();
			castInfos.Add("name", spell.Name);
			castInfos.Add("target", string.Empty);
			castInfos.Add("author", core.inGame.selectedEntities[core.inGame.activeEntity].id);
			castInfos.Add("x", 0f);
			castInfos.Add("z", 0f);
			core.networkManager.send(ServerEventType.spell, HashMapSerializer.hashMapToData(castInfos));
			return;
		}
		
		if(spell.Usage == (int)SpellUsage.buyItem)
		{
			Hashtable castInfos = new Hashtable();
			castInfos.Add("name", spell.Name);
			castInfos.Add("target", string.Empty);
			castInfos.Add("author", core.inGame.selectedEntities[core.inGame.activeEntity].id);
			castInfos.Add("x", 0f);
			castInfos.Add("z", 0f);
			core.networkManager.send(ServerEventType.spell, HashMapSerializer.hashMapToData(castInfos));
			return;
		}
		
		spellInfosMessage.Visible = true;
		enabled = true;
	}
	
	GameObject spellInfo3D;
	
	void insert3DSpellInfo()
	{
		
	}
	
	void insert3DBuildInfo()
	{
		spellInfo3D = (GameObject)Instantiate(Resources.Load("Entities/"+currentSpell.Type, typeof(GameObject)), Vector3.zero, Quaternion.identity);
		spellInfo3D.GetComponent<Entity>().enabled = false;
		spellInfo3D.collider.enabled = false;
		spellInfo3D.AddComponent<FollowMouse>();
		setBuildingMaterial(spellInfo3D);
	}
	
	public void setBuildingMaterial(GameObject myGo)
	{
		Transform[] allChildren = myGo.GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren) {
		 	try
			{
				Material transpClone = new Material(Shader.Find("Particles/Additive"));
				transpClone.SetColor("_TintColor", new Color(1, 1, 1, 0.1f));
				transpClone.mainTexture = child.renderer.material.mainTexture;
				child.renderer.material = transpClone;
			}
			catch
			{
				//this GameObject has no material...	
			}
		}
	}
	
	void remove3DSpellInfo()
	{
		try
		{
			Destroy(spellInfo3D);
		}
		catch
		{
			
		}
	}
}

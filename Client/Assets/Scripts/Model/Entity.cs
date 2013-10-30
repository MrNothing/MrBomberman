using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {
	
	public int id;
	public int team;
	public string owner;
	
	public GameObject Main;
	
	public bool agressive = false;
	
	#region position synchronisation
	public Vector3 destination;
	Vector3 lastPosition = Vector3.zero;
	Vector3 lastRealPosition = Vector3.zero;
	
	float desiredY = 0;
	
	[HideInInspector]
	public float forceHighSync = 0;
	[HideInInspector]
	public Vector3 syncedPosition;
	[HideInInspector]
	public float syncSmoothRate = 8;
	
	public float runSpeed = 10;
	#endregion
	
	public float viewRange = 2.5f;
	
	public float hp = 1;
	public float mp = 0;
	
	public Hashtable infos;
	
	public Texture2D icon;
	
	#region animation
	[HideInInspector]
	public string currentAnim;
	[HideInInspector]
	public string mixedAnim;
	
	[HideInInspector]
	public float animationCounter = 0;
	
	public string[] idleAnims = new string[3];
	public string[] runAnims = new string[2]; //0=walk 1=run 
	public string[] attackAnims = new string[3];
	public string[] incantAnims = new string[5]; //for special skills...
	public string[] deadAnims = new string[1];
	
	public AudioClip[] dialogSounds;
	public AudioClip[] attackSounds;
	public AudioClip[] deathSounds;
	
	public float runAnimSpeed=1;
	#endregion
	
	float currentRot=0;
	float iRotation=0;
	float rotationFact = 8; //how smooth should the rotation be
	
	public float rotationOffset = 0;
	
	Vector3 initialScale;
	Vector3 initialRot;
	Vector3 initialMainPos;
	
	bool dead = false;
	bool moving = false;
	
	public Hashtable spells;
	public List<Spell> _spells = new List<Spell>();
	UICore core;
	// Use this for initialization
	void Start () 
	{
		GameObject GO = (GameObject)GameObject.Find("GUI Camera");
		core = GO.GetComponent<UICore>();
		
		foreach(string s in spells.Keys)
		{
			_spells.Add(new Spell((Hashtable)spells[s]));
		}
	}
	
	float fogCounter = 0;
	float terrainPointCounter = 0;
	
	// Update is called once per frame
	void Update () 
	{
		//if this entity is in the player's team, clear the fog
		if(core.networkManager.currentTeam==team)
		{
			if(fogCounter<=0)
			{
				core.gameManager.clearFog(transform.position, viewRange);
				fogCounter = 1;
			}
			else
				fogCounter-=Time.deltaTime*2;
		}
		
		synchronizePosition(destination);
		
		if(terrainPointCounter<=0)
		{
			try
			{
				TerrainPoint currentPoint = ((FogTileHandler)((Hashtable)core.gameManager.world[core.gameManager.getNearestTile(transform.position)])["FogTileHandler"]).getNearestTerrainPoint(transform.position);
				desiredY = currentPoint.VerticeHeight;
				
				if(currentPoint.FogAlpha>=255)
				{
					if(visible)
					{
						hideAllChildrens();
						visible = false;
					}
				}
				else
				{
					if(!visible)
					{
						showAllChildrens();
						visible = true;
					}
				}
			}
			catch
			{
				desiredY = 0;
			}
			
			terrainPointCounter = 0.5f;
		}
		else
			terrainPointCounter-=Time.deltaTime*2;
		
		if(hp<=0)
		{
			if(dead == false)
			{
				try
				{
					if(!dead)
					{
						//AudioSource.PlayClipAtPoint(deathSounds[UnityEngine.Random.Range(0, deathSounds.Length-1)], transform.position);
						
						//print("i am dead! id: "+id);
						Main.animation.Play(deadAnims[0]); //dead
						//animation[infos.charName+"_Dead"].speed = 1;
					}
				}
				catch
				{
					
				}
				dead = true;
			}
			return;
		}
		else
		{
			dead = false;
		}
		
		Vector3 tmpTrans = transform.localPosition;
		Vector3 tmpPos = transform.position;
		
		if((Mathf.Abs(tmpTrans.x-lastPosition.x)>0 || Mathf.Abs(tmpTrans.z-lastPosition.z)>0))
		{
			moving = true;
			
			float deltaX = tmpPos.x - lastRealPosition.x;
	        float deltaY = tmpPos.z - lastRealPosition.z;
	        float deltaZ = tmpPos.y - lastRealPosition.y;
	        
	       	float turnAngle = (rotationOffset+180*Mathf.Atan2(deltaY, -deltaX)/Mathf.PI); 
			
			//swimIRot = 180*Mathf.Atan(deltaZ)/Mathf.PI;
			
			//if((tmpTrans-lastPosition).magnitude>0.05f)
			iRotation = turnAngle;
			
			float animSpeed = ((tmpTrans-lastPosition).magnitude/0.2f);
			
			currentAnim = runAnims[0]; //walk
				
			Main.animation[currentAnim].speed = animSpeed*runAnimSpeed;
				
		}
		else
		{
			moving = false;
			
			currentAnim = idleAnims[0];
			
			Main.animation[currentAnim].speed = 1;
		}
		
		if(animationCounter<=0)
			Main.animation.CrossFade(currentAnim);
		else
			Main.animation.CrossFade(mixedAnim);
		
		lastPosition = transform.localPosition;
		lastRealPosition = transform.position;
		
		if(forceLookAt>0)
		{
			lookAt();
			forceLookAt--;
		}
		
		//We set the rotation of the Main model realtively to the entity's movements
		
		if(calculateDifferenceBetweenAngles(currentRot, iRotation)>0)
			currentRot +=Mathf.Abs(calculateDifferenceBetweenAngles(currentRot, iRotation))/rotationFact;
		else
			currentRot -=Mathf.Abs(calculateDifferenceBetweenAngles(currentRot, iRotation))/rotationFact;
		
		Main.transform.eulerAngles = new Vector3(0, currentRot, 0);	
		
		if(animationCounter>0)
			animationCounter-=60f*Time.deltaTime;
	}
	
	private float calculateDifferenceBetweenAngles(float firstAngle, float secondAngle)
	{
        float difference = secondAngle - firstAngle;
        while (difference < -180) difference += 360;
        while (difference > 180) difference -= 360;
        return difference;
	}
	
	private bool visible=true;
	public void hideAllChildrens()
	{
		Transform[] allChildren = GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren) {
		 	try
			{
				child.renderer.enabled = false;
			}
			catch
			{
				
			}
		}
	}
	
	public void showAllChildrens()
	{
		Transform[] allChildren = GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren) {
		 	try
			{
				child.renderer.enabled = true;
			}
			catch
			{
				
			}
		}
	}
	
	void synchronizePosition(Vector3 destination)
	{
		Vector3 tmpPos = transform.position;
		
		if(tmpPos.y<desiredY)
			tmpPos.y+=Mathf.Abs(tmpPos.y-desiredY)/8;
		
		if(tmpPos.y>desiredY)
			tmpPos.y-=Mathf.Abs(tmpPos.y-desiredY)/8;
		
		if(Mathf.Abs(tmpPos.y-desiredY)<0.01f)
			tmpPos.y = desiredY;
		
		if(forceHighSync<=0)
		{
			/*if(Vector3.Distance(transform.position,destination)>Time.deltaTime*runSpeed)
		 		transform.Translate(Vector3.Normalize(destination-transform.position)*Time.deltaTime*runSpeed);
			else
				transform.position = destination;*/
			
			if(tmpPos.x<destination.x)
				tmpPos.x+=runSpeed*Time.deltaTime;
			
			if(tmpPos.x>destination.x)
				tmpPos.x-=runSpeed*Time.deltaTime;
			
			if(tmpPos.z<destination.z)
				tmpPos.z+=runSpeed*Time.deltaTime;
			
			if(tmpPos.z>destination.z)
				tmpPos.z-=runSpeed*Time.deltaTime;
			
			if(Mathf.Abs(tmpPos.x-destination.x)<runSpeed*Time.deltaTime)
				tmpPos.x = destination.x;
			
			if(Mathf.Abs(tmpPos.z-destination.z)<runSpeed*Time.deltaTime)
				tmpPos.z = destination.z;
		}
		else
		{
			if(tmpPos.x<syncedPosition.x)
				tmpPos.x+=Mathf.Abs(tmpPos.x-syncedPosition.x)/syncSmoothRate;
			
			if(tmpPos.x>syncedPosition.x)
				tmpPos.x-=Mathf.Abs(tmpPos.x-syncedPosition.x)/syncSmoothRate;
			
			if(tmpPos.z<syncedPosition.z)
				tmpPos.z+=Mathf.Abs(tmpPos.z-syncedPosition.z)/syncSmoothRate;
			
			if(tmpPos.z>syncedPosition.z)
				tmpPos.z-=Mathf.Abs(tmpPos.z-syncedPosition.z)/syncSmoothRate;
			
			forceHighSync--;
		}
		
		transform.position = tmpPos;
	}
	
	[HideInInspector]
	public GameObject target = null;
	
	[HideInInspector]
	public Vector3 targetPos = Vector3.zero;
	
	[HideInInspector]
	public int forceLookAt = 0;
	
	public void lookAt()
	{
		try
		{
			if(targetPos!=Vector3.zero)
			{
				float deltaX = transform.position.x - targetPos.x;
		        float deltaY = transform.position.z - targetPos.z;
		        
		       	float turnAngle = (rotationOffset+180*Mathf.Atan2(deltaY, -deltaX)/Mathf.PI); 
				iRotation = turnAngle+180;
			}
			else
			{
				float deltaX = transform.position.x - target.transform.position.x;
		        float deltaY = transform.position.z - target.transform.position.z;
		        
		       	float turnAngle = (rotationOffset+180*Mathf.Atan2(deltaY, -deltaX)/Mathf.PI); 
				iRotation = turnAngle+180;
			}
		}
		catch
		{
			target = null;
		}
	}
	
	public string getName()
	{
		return infos["name"].ToString();
	}
	
	public int getLevel()
	{
		return (int)infos["level"];
	}
	
	public float getMaxHp()
	{
		print ("stats: "+infos["stats"]+" bonuses: "+infos["bonuses"]);
		
		Hashtable stats = (Hashtable) infos["stats"];
		Hashtable bonuses = (Hashtable) infos["bonuses"];
		
		foreach(string s in stats.Keys)
			print("stat: "+s+"="+stats[s]);
		//print("hp:"+stats["hp"]);
		//print("hp+:"+bonuses["hp"]);
		
		return ((float)(((Hashtable)infos["stats"])["hp"]))+((float)(((Hashtable)infos["bonuses"])["hp"]));
	}
	
	public float getMaxMp()
	{
		return ((float)(((Hashtable)infos["stats"])["mp"]))+((float)(((Hashtable)infos["bonuses"])["mp"]));
	}
}

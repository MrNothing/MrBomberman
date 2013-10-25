using UnityEngine;
using System.Collections;

public class BombeScript : MonoBehaviour {
	
	[SerializeField]
	private Transform _pad1;
	
	float Temps_Avant_Explosion;
	
	GameObject Bombe;
	
	GameObject bobombe;
	
	private RaycastHit hit;
	
	public Transform pad1
	{
		get {return _pad1;}
		set { _pad1=value;}
	}
	float startTime=0.0f;
	
	// Use this for initialization
	void Start () {
		
		Temps_Avant_Explosion=Time.time;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("StartTime : "+startTime);
		
		startTime=startTime+Time.deltaTime;
		
	if (startTime>5.0f)
		{
		/****Lancement de rayon vers le haut***/
		Vector3 direction_rayon;
		
		direction_rayon.x=0.0f;
		direction_rayon.y=0.0f;
		direction_rayon.z=1.5f;
		
	if (Physics.Raycast(this.transform.position,direction_rayon,out hit,1.0f))
	{
			if (hit.collider.tag=="Bloc Destructible")
			{
				Destroy(hit.collider.gameObject);
			}
			else if (hit.collider.tag=="Player")
			{
				Destroy(hit.collider.gameObject);
			//MonoBehaviour scriptpersonnage=	hit.collider.gameObject.GetComponent(DeplacementPersonnageScript);
				
				//scriptpersonnage.
				
				
			}

	}
					/********Rayon vers le bas*************/
		direction_rayon.x=0.0f;
		direction_rayon.y=0.0f;
		direction_rayon.z=-1.5f;
		
	if (Physics.Raycast(this.transform.position,direction_rayon,out hit,1.0f))
	{
			if (hit.collider.tag=="Bloc Destructible")
			{
				Destroy(hit.collider.gameObject);
			}
			else if (hit.collider.tag=="Player")
			{
				Destroy(hit.collider.gameObject);
			//MonoBehaviour scriptpersonnage=	hit.collider.gameObject.GetComponent(DeplacementPersonnageScript);
				
				//scriptpersonnage.
				
				
			}

	}
					/***************Rayon gauche******************/
		
		direction_rayon.x=-1.5f;
		direction_rayon.y=0.0f;
		direction_rayon.z=0f;
		
	if (Physics.Raycast(this.transform.position,direction_rayon,out hit,1.0f))
	{
			if (hit.collider.tag=="Bloc Destructible")
			{
				Destroy(hit.collider.gameObject);
			}
			else if (hit.collider.tag=="Player")
			{
				Destroy(hit.collider.gameObject);
			//MonoBehaviour scriptpersonnage=	hit.collider.gameObject.GetComponent(DeplacementPersonnageScript);
				
				//scriptpersonnage.
				
				
			}

	}
				/*******rayon droite ********/
		
		direction_rayon.x=1.5f;
		direction_rayon.y=0.0f;
		direction_rayon.z=0.0f;
		
	if (Physics.Raycast(this.transform.position,direction_rayon,out hit,1.0f))
	{
			if (hit.collider.tag=="Bloc Destructible")
			{
				Destroy(hit.collider.gameObject);
			}
			else if (hit.collider.tag=="Player")
			{
				Destroy(hit.collider.gameObject);
			//MonoBehaviour scriptpersonnage=	hit.collider.gameObject.GetComponent(DeplacementPersonnageScript);
				
				//scriptpersonnage.
				
				
			}

	}
			Destroy(this.gameObject);
	}
	}
}

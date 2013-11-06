using UnityEngine;
using System.Collections;

public class DeplacementPersonnageScript : MonoBehaviour {


	[SerializeField]
	private Transform _pad1;

	[SerializeField]
	private float _padSpeed=4f;
	
	[SerializeField]
	private Transform prefabBombe;  //

	public Transform PrefabBombe {
		get {
			return this.prefabBombe;
		}
		set {
			prefabBombe = value;
		}
	}
	public float padSpeed
	{
		get {return _padSpeed;}
		set { _padSpeed=value;}
	}

	public Transform pad1
	{
		get {return _pad1;}
		set { _pad1=value;}
	}


	private bool Deplacement_Bas=false;
	private bool Deplacement_Haut=false;
	private bool Deplacement_Gauche=false;
	private bool Deplacement_Droite=false;
	private bool Bombe_poser=false;
	

	private RaycastHit hit;  // objet remplit si il y a collision entre le rayon de position et un mur
	
	private int Stock_Bombe;

	public int StockBombe {
		get {
			return this.Stock_Bombe;
		}
		set {
			Stock_Bombe = value;
		}
	}

	private int Bombe_a_terre;

	public int BombeATerre {
		get {
			return this.Bombe_a_terre;
		}
		set {
			Bombe_a_terre = value;
		}
	}	
	private bool Est_en_vie;

	// Use this for initialization
	void Start () {

		Deplacement_Bas=false;
		Deplacement_Haut=false;
		Deplacement_Gauche=false;
		Deplacement_Droite=false;
		Bombe_poser=false;
		Stock_Bombe=10;
		Bombe_a_terre=0;
		Est_en_vie=true;

	}

	// Update is called once per frame
	void Update () {
		if (Est_en_vie)
		{
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			
			Bombe_poser=true;
			
		}else{
			Bombe_poser=false;
		}
		
		/***************Déplacement Bas**********************/
		if (Input.GetKey(KeyCode.DownArrow))
		{
			Vector3 direction_rayon;
			direction_rayon.x=0.0f;
			direction_rayon.y=0.0f;
			direction_rayon.z=-0.4f;
			
			//Debug.DrawRay(_pad1.position,direction_rayon,Color.red);
			
			Deplacement_Bas=true;
			
			// On lance un rayon devant le personnage pour vérifier s'il y a quelque chose devant
			if (Physics.Raycast(_pad1.transform.position,direction_rayon,out hit,0.4f))
			{
				// Si le rayon touche un bloc indestructible, une bordure ou un bloc destructible alors on ne fait rien. 
				//sinon on ce déplace
				//si le hit.collider.tag ne fonctionne pas on peux essayer par le hit.collider.name
				
				if (hit.collider.tag=="Bloc Indestructible" || hit.collider.tag=="Bordure" || hit.collider.tag=="Bloc Destructible")
					{
						Deplacement_Bas=false;
					}
			}
		}else { Deplacement_Bas=false;	}
		
		/***************Déplacement HAUT************************/
		if (Input.GetKey(KeyCode.UpArrow))
		{
			Vector3 direction_rayon;
			direction_rayon.x=0.0f;
			direction_rayon.y=0.0f;
			direction_rayon.z=0.4f;
			
			//Debug.DrawRay(_pad1.position,direction_rayon,Color.red);
			
			Deplacement_Haut=true;
			
			// On lance un rayon devant le personnage pour vérifier s'il y a quelque chose devant
			if (Physics.Raycast(_pad1.transform.position,direction_rayon,out hit,0.4f))
			{
				// Si le rayon touche un bloc indestructible, une bordure ou un bloc destructible alors on ne fait rien. 
				//sinon on ce déplace
				//si le hit.collider.tag ne fonctionne pas on peux essayer par le hit.collider.name
				
				if (hit.collider.tag=="Bloc Indestructible" || hit.collider.tag=="Bordure" || hit.collider.tag=="Bloc Destructible")
					{
						Deplacement_Haut=false;
					}
			}
		}else { Deplacement_Haut=false;	}
	
		
		/*********************Déplacement Gauche ***************************/
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			Vector3 direction_rayon;
			direction_rayon.x=-0.4f;
			direction_rayon.y=0.0f;
			direction_rayon.z=0.0f;
			
			//Debug.DrawRay(_pad1.position,direction_rayon,Color.red);
			
			Deplacement_Gauche=true;
			
			// On lance un rayon devant le personnage pour vérifier s'il y a quelque chose devant
			if (Physics.Raycast(_pad1.transform.position,direction_rayon,out hit,0.4f))
			{
				// Si le rayon touche un bloc indestructible, une bordure ou un bloc destructible alors on ne fait rien. 
				//sinon on ce déplace
				//si le hit.collider.tag ne fonctionne pas on peux essayer par le hit.collider.name
				
				if (hit.collider.tag=="Bloc Indestructible" || hit.collider.tag=="Bordure" || hit.collider.tag=="Bloc Destructible")
					{
						Deplacement_Gauche=false;
					}
			}
		}else { Deplacement_Gauche=false;	}
		
		
		/*********************Déplacement Droite ***************************/
		
		if (Input.GetKey(KeyCode.RightArrow))
		{
			Vector3 direction_rayon;
			direction_rayon.x=0.4f;
			direction_rayon.y=0.0f;
			direction_rayon.z=0.0f;
			
			//Debug.DrawRay(_pad1.position,direction_rayon,Color.red);
			
			Deplacement_Droite=true;
			
			// On lance un rayon devant le personnage pour vérifier s'il y a quelque chose devant
			if (Physics.Raycast(_pad1.transform.position,direction_rayon,out hit,0.4f))
			{
				// Si le rayon touche un bloc indestructible, une bordure ou un bloc destructible alors on ne fait rien. 
				//sinon on ce déplace
				//si le hit.collider.tag ne fonctionne pas on peux essayer par le hit.collider.name
				
				if (hit.collider.tag=="Bloc Indestructible" || hit.collider.tag=="Bordure" || hit.collider.tag=="Bloc Destructible")
					{
						Deplacement_Droite=false;
					}
			}
		}else { Deplacement_Droite=false;	}
			
		}
		
	}

	void FixedUpdate (){
		
		if (Bombe_poser)
		{
			if (Bombe_a_terre<Stock_Bombe)
			{
				Instantiate(prefabBombe,_pad1.position,_pad1.rotation) ;
				Bombe_a_terre=Bombe_a_terre+1;
			}
		}

		if (Deplacement_Bas)
		{	
			Vector3 m_Down;
			m_Down.x=0.0f;
			m_Down.y=0.0f;
			m_Down.z=-1.0f;

			//_pad1.Translate(Vector3.down* _padSpeed*Time.deltaTime);
			pad1.Translate(	m_Down *_padSpeed*Time.deltaTime);

		}

		if (Deplacement_Haut)
		{	
			Vector3 m_Up;
			m_Up.x=0.0f;
			m_Up.y=0.0f;
			m_Up.z=1.0f;

			_pad1.Translate(m_Up* _padSpeed*Time.deltaTime);

		}

		if (Deplacement_Gauche)
		{	
			Vector3 m_Left;
			m_Left.x=-1.0f;
			m_Left.y=0.0f;
			m_Left.z=0.0f;
			//_pad1.Translate(Vector3.left* _padSpeed*Time.deltaTime);
			_pad1.Translate(m_Left* _padSpeed*Time.deltaTime);
		}

		if (Deplacement_Droite)
		{	
			Vector3 m_Right;
			m_Right.x=1.0f;
			m_Right.y=0.0f;
			m_Right.z=0.0f;
			//	_pad1.Translate(Vector3.right* _padSpeed*Time.deltaTime);
			_pad1.Translate(m_Right* _padSpeed*Time.deltaTime);
		}
	}
	void Augmentation_capacité_bombe()
	{
		Stock_Bombe=Stock_Bombe+1;	
	}
	
	void bombe_en_moins()
	{
		Bombe_a_terre=Bombe_a_terre-1;
		
	}
	
	void mort()
	{
	Est_en_vie=false;	
	}

}
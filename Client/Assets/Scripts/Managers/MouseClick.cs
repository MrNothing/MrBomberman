using UnityEngine;
using System.Collections;

public class MouseClick : MonoBehaviour 
{

	public UICore core;
	public Camera tCamera;
	
	public Color controlledColor;
	public Color allyColor;
	public Color neutralColor;
	public Color foeColor;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(core.inGame.Visible && Input.GetMouseButtonUp(0))
			onObjectClicked(Input.mousePosition);
		
		if(core.inGame.Visible && Input.GetMouseButtonUp(1))
			requestMoveAtMousePointOrAttackEntity();
	}
	
	void onObjectClicked(Vector3 position)
	{
		Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hitFloor2;
		if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
		{
			if(hitFloor2.collider.gameObject.GetComponent<Entity>())
			{
				Entity tmpEntity = hitFloor2.collider.gameObject.GetComponent<Entity>();
				core.gameManager.selectionCircle.transform.position = tmpEntity.transform.position+new Vector3(0, 0.1f, 0);
				core.inGame.selectedEntity = tmpEntity;
				core.inGame.UnitAvatar.Texture = core.blackAlphaBg;
				
				if(tmpEntity.owner.Equals(core.networkManager.username))
				{
					core.gameManager.selectionCircle.renderer.material.SetColor("_TintColor", controlledColor);
				}
				else
				{
					if(tmpEntity.team.Equals(core.networkManager.currentTeam))
						core.gameManager.selectionCircle.renderer.material.SetColor("_TintColor", allyColor);
					else
					{
						if(tmpEntity.agressive)
							core.gameManager.selectionCircle.renderer.material.SetColor("_TintColor", foeColor);
						else
							core.gameManager.selectionCircle.renderer.material.SetColor("_TintColor", neutralColor);
					}	
				}
			}
		}
	}
	
	void requestMoveAtMousePointOrAttackEntity()
	{
		if(core.inGame.selectedEntity.owner.Equals(core.networkManager.username))
		{
			Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit hitFloor2;
			if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
			{
				if(hitFloor2.collider.gameObject.GetComponent<Entity>())
				{
					Hashtable castInfos = new Hashtable();
					castInfos.Add("author", core.inGame.selectedEntity.id);
					castInfos.Add("target", hitFloor2.collider.gameObject.GetComponent<Entity>().id);
					core.networkManager.send(ServerEventType.attack, HashMapSerializer.hashMapToData(castInfos));
				}
				else
				{
					Hashtable infos = new Hashtable();
					infos.Add("id", core.inGame.selectedEntity.id);
					infos.Add("x", hitFloor2.point.x);
					infos.Add("z", hitFloor2.point.z);
					
					core.networkManager.send(ServerEventType.position, HashMapSerializer.hashMapToData(infos));
				}
			}
		}
	}
}

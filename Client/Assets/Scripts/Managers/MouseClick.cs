using UnityEngine;
using System.Collections;

public class MouseClick : MonoBehaviour 
{

	public UICore core;
	public Camera tCamera;
	
	public Texture2D rectangleIcon;
	MGUIImage selectionRectangle;
	// Use this for initialization
	void Start () 
	{
		selectionRectangle = (MGUIImage)core.gui.setImage("selectionRectangle", new Rect(0, 0, 1, 1), Vector2.zero, rectangleIcon);
		selectionRectangle.Visible = false;
	}
	
	int wait=0;
	bool dragging = false;
	float draggingCounter = 0;
	// Update is called once per frame
	void Update () 
	{
		if(core.inGame.Visible && Input.GetMouseButtonUp(0) && wait<=0)
		{
			onObjectClicked(Input.mousePosition);
			wait = 3;
		}
		
		if(core.inGame.Visible && Input.GetMouseButtonUp(1) && wait<=0)
		{
			requestMoveAtMousePointOrAttackEntity();
			wait = 3;
		}
		
		wait--;
		
		if(Input.GetMouseButton(0))
		{
			draggingCounter+=Time.deltaTime;
			if(draggingCounter>0.01f && (Input.mousePosition-lastMouse2dPosition).magnitude>0)
			{
				if(!dragging)
					onDragStarted();
				
				dragging = true;
			}
		}
		else
		{
			draggingCounter = 0;
			
			if(dragging)
				onDragReleased();
			
			dragging = false;
		}
		
		if(dragging)
		{
			Vector3 mousePosition = core.gui.GUICamera.ScreenToWorldPoint(Input.mousePosition);
			setQuaternionSize(selectionRectangle._container, new  Vector2(mousePosition.x-lastMousePosition.x, mousePosition.y-lastMousePosition.y));
			selectionRectangle.Container.transform.position = new Vector3(lastMousePosition.x, lastMousePosition.y, selectionRectangle.Container.transform.position.z);
		}
	}
	
	Vector3 lastMouse2dPosition;
	Vector3 lastMousePosition;
	Vector3 lastGameMousePosition;
	void onDragStarted()
	{
		lastMouse2dPosition = Input.mousePosition;
		lastMousePosition = core.gui.GUICamera.ScreenToWorldPoint(Input.mousePosition);
		
		RaycastHit hitFloor2;
		Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
		{
			lastGameMousePosition = hitFloor2.point;
		}
		
		selectionRectangle.Visible = true;

	}
	
	void onDragReleased()
	{
		Vector3 gameMousePosition = Vector3.zero;
		
		RaycastHit hitFloor2;
		Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
		{
			gameMousePosition = hitFloor2.point;
		}
		
		bool found = false;
		
		foreach(int i in core.gameManager.entities.Keys)
		{
			Entity e = core.gameManager.entities[i];
			
			if(isInside(e.transform.position.x, lastGameMousePosition.x, gameMousePosition.x) && isInside(e.transform.position.z, lastGameMousePosition.z, gameMousePosition.z) && e.owner.Equals(core.networkManager.username))
			{
				if(!found && !Input.GetKey(KeyCode.LeftShift))
					core.inGame.selectedEntities.Clear();
				
				try
				{
					core.inGame.selectedEntities.Remove(e);
				}
				catch
				{
					
				}
				
				core.inGame.selectedEntities.Add(e);
				core.inGame.UnitAvatar.Texture = core.blackAlphaBg;
				
				if(core.inGame.selectedEntities.Count>9)
					break;
			
				found = true;
			}
		}
		
		
		
		selectionRectangle.Visible = false;
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
				
				if(!Input.GetKey(KeyCode.LeftShift))
					core.inGame.selectedEntities.Clear();
				else
				{
					if(core.inGame.selectedEntities.Count>9)
						return;
				}
				
				//core.inGame.forceReload = true;
				if((Input.GetKey(KeyCode.LeftShift) && tmpEntity.owner.Equals(core.networkManager.username)) || !Input.GetKey(KeyCode.LeftShift))
				{
					if(!Input.GetKey(KeyCode.LeftShift))
					{
						try
						{
							core.inGame.selectedEntities.Remove(tmpEntity);
						}
						catch
						{
						}
						core.inGame.selectedEntities.Add(tmpEntity);
						core.inGame.UnitAvatar.Texture = core.blackAlphaBg;
					}
					else
					{
						int lastCount = core.inGame.selectedEntities.Count;
						core.inGame.selectedEntities.Remove(tmpEntity);
							
						if(lastCount==core.inGame.selectedEntities.Count)
						{
							core.inGame.selectedEntities.Add(tmpEntity);
							core.inGame.UnitAvatar.Texture = core.blackAlphaBg;
						}
					}
				}
			}
		}
	}
	
	void requestMoveAtMousePointOrAttackEntity()
	{
		
		{
			Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit hitFloor2;
			if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
			{
				if(hitFloor2.collider.gameObject.GetComponent<Entity>())
				{
					foreach(Entity selectedEntity in core.inGame.selectedEntities)
					{
						if(selectedEntity.owner.Equals(core.networkManager.username))
						{
							Hashtable castInfos = new Hashtable();
							castInfos.Add("author", selectedEntity.id);
							castInfos.Add("target", hitFloor2.collider.gameObject.GetComponent<Entity>().id);
							core.networkManager.send(ServerEventType.attack, HashMapSerializer.hashMapToData(castInfos));
						}
					}
				}
				else
				{
					int counter = 0;
					foreach(Entity selectedEntity in core.inGame.selectedEntities)
					{
						if(selectedEntity.owner.Equals(core.networkManager.username))
						{
							Hashtable infos = new Hashtable();
							infos.Add("id", selectedEntity.id);
							infos.Add("x", hitFloor2.point.x+core.inGame.formations[counter].x);
							infos.Add("z", hitFloor2.point.z+core.inGame.formations[counter].y);
							
							counter++;
							
							core.networkManager.send(ServerEventType.position, HashMapSerializer.hashMapToData(infos));
						}
					}
				}
			}
		}
	}
	
	void setQuaternionSize(GameObject bar, Vector2 size)
	{
		MeshFilter myFilter = bar.GetComponent<MeshFilter>();
		
		Vector3[] lastVertices = myFilter.mesh.vertices;
		Vector3[] vertices=null;
		
		if(size.x>=0 && size.y>=0)
		{
			vertices = new Vector3[]
			{
				new Vector3(size.x, 0, size.y),
				new Vector3(size.x, 0, 0),
				new Vector3(0, 0, size.y),
				new Vector3(0, 0, 0),
			};
		}
		
		if(size.x<0 && size.y>=0)
		{
			vertices = new Vector3[]
			{
				new Vector3(0, 0, size.y),
				new Vector3(0, 0, 0),
				new Vector3(size.x, 0, size.y),
				new Vector3(size.x, 0, 0),
			};
		}
		
		if(size.x>=0 && size.y<0)
		{
			vertices = new Vector3[]
			{
				new Vector3(size.x, 0, 0),
				new Vector3(size.x, 0, size.y),
				new Vector3(0, 0, 0),
				new Vector3(0, 0, size.y),
			};
		}
		
		if(size.x<0 && size.y<0)
		{
			vertices = new Vector3[]
			{
				new Vector3(0, 0, 0),
				new Vector3(0, 0, size.y),
				new Vector3(size.x, 0, 0),
				new Vector3(size.x, 0, size.y),
			};
		}
		
		myFilter.mesh.vertices = vertices;
		myFilter.mesh.RecalculateBounds();
		myFilter.mesh.RecalculateNormals();
	}
	
	bool isInside(float _value, float _start, float _end)
	{
		if(_end>_start)
		{
			if(_value>_start && _value<_end)
				return true;
			else
				return false;
		}
		else
		{
			if(_value<_start && _value>_end)
				return true;
			else
				return false;
		}
	}
}

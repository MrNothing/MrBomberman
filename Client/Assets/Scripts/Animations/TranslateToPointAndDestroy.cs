using UnityEngine;
using System.Collections;

public class TranslateToPointAndDestroy : MonoBehaviour 
{
	
	public bool isTransform = false;
	public bool isLocal = false;
	public float speed = 1;
	public Vector3 target = Vector3.zero;
	public Transform targetAsTransform;
	
	public GameObject instantiateOnDestroy;
	
	Vector3 initialPosition;
	
	UICore core;
	// Use this for initialization
	void Start () 
	{
		initialPosition = transform.position;
		core = ((GameObject)GameObject.Find("GUI Camera")).GetComponent<UICore>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(core.gameManager.gamePaused)
			return;
		
		if(!isTransform)
		{
			transform.LookAt(target);
			transform.Translate(Vector3.forward*speed*Time.deltaTime*60);
			
			if(Vector3.Distance(transform.position, target)<speed)
			{
				try
				{
					Instantiate(instantiateOnDestroy, transform.position, Quaternion.identity);
				}
				catch
				{
					
				}
				Destroy(gameObject);
			}
		}
		else if(isLocal)
		{
			transform.LookAt(initialPosition+target);
			transform.Translate(Vector3.forward*speed*Time.deltaTime*60);
			
			if(Vector3.Distance(transform.position, initialPosition+target)<speed)
			{
				try
				{
					Instantiate(instantiateOnDestroy, transform.position, Quaternion.identity);
				}
				catch
				{
					
				}
				Destroy(gameObject);
			}
		}
		else
		{
			Vector3 targetPoint;
			try
			{
				targetPoint = targetAsTransform.position+new Vector3(0, targetAsTransform.collider.bounds.size.y/2, 0);
			}
			catch
			{
				targetPoint = targetAsTransform.position;
			}
			transform.LookAt(targetPoint);
			transform.Translate(Vector3.forward*speed*Time.deltaTime*60);
			
			if(Vector3.Distance(transform.position, targetPoint)<speed)
			{
				try
				{
					Instantiate(instantiateOnDestroy, transform.position, Quaternion.identity);
				}
				catch
				{
					
				}
				Destroy(gameObject);
			}
		}
	}
}

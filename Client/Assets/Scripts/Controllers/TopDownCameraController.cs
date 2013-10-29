using UnityEngine;
using System.Collections;
using System;

public class TopDownCameraController : MonoBehaviour 
{
	public float currentDistance = 52;
	public float minDistance = 5;
	public float maxDistance = 52;
	public float cameraSpeed = 30;
	public float smoothIterations = 5;
	public Transform theCamera;
	public LayerMask collisionLayers;
	
	private Vector3 lastPosition;
	
	// Use this for initialization
	void Start () 
	{
		lastPosition = transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(Input.mousePosition.x>Screen.width-10)
			transform.Translate(cameraSpeed*Time.deltaTime, 0, 0);
		
		if(Input.mousePosition.x<10)
			transform.Translate(-cameraSpeed*Time.deltaTime, 0, 0);
		
		if(Input.mousePosition.y>Screen.height-10)
			transform.Translate(0, 0, cameraSpeed*Time.deltaTime);
		
		if(Input.mousePosition.y<10)
			transform.Translate(0, 0, -cameraSpeed*Time.deltaTime);
		
		currentDistance += Input.GetAxis("Mouse ScrollWheel")*5;
		
		if(currentDistance<minDistance)
			currentDistance=minDistance;
		
		if(currentDistance>maxDistance)
			currentDistance=maxDistance;

		RaycastHit hit;
		Ray theRay = new Ray(transform.position, theCamera.forward);
		if (Physics.Raycast(theRay,out hit, 100f, collisionLayers.value)) 
		{
				float distanceToGround = (float)hit.distance;
				
				if(distanceToGround>currentDistance)
				{
					transform.Translate(0, -Mathf.Abs(distanceToGround-currentDistance)/smoothIterations, 0);
				}
				
				if(distanceToGround<currentDistance)
				{
					transform.Translate(0, Mathf.Abs(distanceToGround-currentDistance)/smoothIterations, 0);
				}
			
			lastPosition = transform.position;
		}
		else
		{
			translateToPoint(lastPosition);
		}
	}
	
	void translateToPoint(Vector3 point)
	{
		if(transform.position.x<point.x)
			transform.Translate(Mathf.Abs(transform.position.x-point.x), 0, 0);
		
		if(transform.position.x>point.x)
			transform.Translate(-Mathf.Abs(transform.position.x-point.x), 0, 0);
		
		if(transform.position.z<point.z)
			transform.Translate(0, 0, Mathf.Abs(transform.position.z-point.z));
		
		if(transform.position.z>point.z)
			transform.Translate(0, 0, -Mathf.Abs(transform.position.z-point.z));
	}
}

using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {
	
	Transform myTransform;
	Camera tCamera;
	// Use this for initialization
	void Start () {
		tCamera = GameObject.Find("Main Camera").camera;
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = tCamera.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hitFloor2;
		if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 100f)) 
		{
			myTransform.position = hitFloor2.point;
		}
	}
}

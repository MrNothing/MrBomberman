using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
	
	public Vector3 rotation;
	public bool isLocal=false;
	
	UICore core;
	
	// Use this for initialization
	void Start () 
	{
		core = ((GameObject)GameObject.Find("GUI Camera")).GetComponent<UICore>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(core.gameManager.gamePaused)
			return;
		
		if(isLocal)
			transform.localEulerAngles+=rotation;
		else
			transform.Rotate(rotation);
	}
}

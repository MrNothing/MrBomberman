using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	
	public Vector3 rotation;
	public bool isLocal=false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(isLocal)
			transform.localEulerAngles+=rotation;
		else
			transform.Rotate(rotation);
	}
}

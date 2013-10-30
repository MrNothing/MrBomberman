using UnityEngine;
using System.Collections;

public class TranslateToPointAndDestroy : MonoBehaviour {
	
	public bool isTransform = false;
	public bool isLocal = false;
	public float speed = 1;
	public Vector3 target = Vector3.zero;
	public Transform targetAsTransform;
	
	public GameObject instantiateOnDestroy;
	
	Vector3 initialPosition;
	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isTransform)
		{
			transform.LookAt(target);
			transform.Translate(Vector3.forward*speed);
			
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
			transform.Translate(Vector3.forward*speed);
			
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
			transform.LookAt(targetAsTransform.position);
			transform.Translate(Vector3.forward*speed);
			
			if(Vector3.Distance(transform.position, targetAsTransform.position)<speed)
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

using UnityEngine;
using System.Collections;

public class DestroyAfterDelay : MonoBehaviour
{
	public float timeToWait = 1;
	public GameObject instantiateOnDestroy;
	
	// Use this for initialization
	void Start () 
	{
		Invoke("destroyMe", timeToWait);
	}
	
	void destroyMe()
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

using UnityEngine;
using System.Collections;

public class DestroyAfterDelay : MonoBehaviour
{
	public float timeToWait = 1;
	public GameObject instantiateOnDestroy;
	
	float countedTime = 0;
	UICore core;
	// Use this for initialization
	void Start () 
	{
		core = ((GameObject)GameObject.Find("GUI Camera")).GetComponent<UICore>();
	}
	
	void Update()
	{
		if(core.gameManager.gamePaused)
			return;
		
		countedTime+=Time.deltaTime;
		
		if(countedTime>=timeToWait)
		{
			destroyMe();
		}
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

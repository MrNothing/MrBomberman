using UnityEngine;
using System.Collections;

public class GameLobby : MonoBehaviour {
	
	bool _visible = false;

	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			_visible = value;
		}
	}
	
	public MGUITextArea textArea;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using System;

/*
 * Connector contains all methods to interact with the server...
 * */

public class StartScreen : MonoBehaviour 
{
	//we keep a reference to UICore to be able to create GUI elements easily.
	public UICore core;
	MGUI gui;
	
	MGUIText title;
	MGUITextfield serverTextField;
	//MGUITextfield localportTextField;
	//MGUIButton startServerButton;
	MGUIButton connectButton;
	// Use this for initialization
	void Start () 
	{
		gui = core.gui;
		
		//title
		title = (MGUIText) gui.setText("titleText", new Rect(-10, 10, 0, 0), "Bomberman version 0 (alpha)", core.normalFont, Color.white);
		
		//server string
		serverTextField = (MGUITextfield) gui.setTextField("server", new Rect(-10, 0, 10, 1.5f), new Vector3(0, 0), "127.0.0.1:6600", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		
		//server string
		//localportTextField = (MGUITextfield) gui.setTextField("localServerPort", new Rect(-10, -5, 10, 1.5f), new Vector3(0, 0), "6600", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		
		//Play Button
		connectButton = (MGUIButton)gui.setButton("playBut", new Rect(10, 0, 7, 2.5f), new Vector2(0, 0), "Connect", core.normalFont, Color.green, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		connectButton.OnButtonPressed += new MGUIButton.ButtonPressed(OnConnectButton);
		
		//Server Button
		/*startServerButton = (MGUIButton)gui.setButton("serverBut", new Rect(10, -5, 7, 1.5f), new Vector2(0, 0), "Start Server", core.normalFont, Color.green, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		startServerButton.moveText(new Vector2(-2, 0));
		startServerButton.OnButtonPressed += new MGUIButton.ButtonPressed(OnCreateServerButton);
		*/
		
		serverTextField.setDepth(10);
		connectButton.setDepth(10);
		//startServerButton.setDepth(10);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnConnectButton(MGUIButton key)
	{
		if((gui.getElement("server") as MGUITextfield).Text.Length>0)
		{
			try
			{
				string port = serverTextField.Text.Substring(serverTextField.Text.IndexOf(":")+1);
				string server = serverTextField.Text.Replace(":"+port, string.Empty);
				
				Debug.Log("server: "+server+" port: "+port);
				
				core.networkManager.connect(server, port);
				
				//hide interface, once I have connected, go to lobby
				hide();
				
				core.errorInterface.showMessage("Connecting...", Color.cyan, false);
				Debug.Log("Connecting...");
			}
			catch(Exception e)
			{
				core.errorInterface.showMessage("Error: "+e.Message, Color.red, true);
				Debug.Log("Error: "+e.Message);
			}
		}
	}
	
	/*void OnCreateServerButton(int key)
	{
		try
		{
			Network.InitializeSecurity();
			Network.InitializeServer(100, int.Parse(localportTextField.Text), false);
			startServerButton.activated = false;
			
			//hide interface and go to lobby using the local connection.
			hide();
			
			core.errorInterface.showMessage("Server started!", Color.green, true);
			Debug.Log("Server started on port: "+localportTextField.Text);
		}
		catch(Exception e)
		{
			core.errorInterface.showMessage("Error: "+e.Message, Color.red, true);
			Debug.Log("Error: "+e.Message);
		}
	}*/
	
	public void hide()
	{
		serverTextField.Visible = false;
		//localportTextField.Visible = false;
		connectButton.Visible = false;
		//startServerButton.Visible = false;
		title.Visible = false;
	}
	
	public void show()
	{
		serverTextField.Visible = true;
		//localportTextField.Visible = true;
		connectButton.Visible = true;
		//startServerButton.Visible = true;
		title.Visible = true;
	}
}

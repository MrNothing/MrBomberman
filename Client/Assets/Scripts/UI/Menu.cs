using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : MonoBehaviour 
{
	
	public UICore core;
	
	public Texture2D bgImage;
	
	MGUIImage bg;
	List<MGUIButton> menuButtons = new List<MGUIButton>();
	
	bool _visible;

	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			_visible = value;
			bg.Visible = value;
			
			foreach(MGUIButton b in menuButtons)
				b.Visible = value;
		}
	}	
	
	
	// Use this for initialization
	void Start () 
	{
		bg = (MGUIImage) core.gui.setImage("menu_BG", new Rect(0, -1.5f, 9, 12), Vector2.zero, bgImage);
		bg.setDepth(0.5f);
		
		MGUIButton leaveGameB = (MGUIButton) core.gui.setButton("menu_leave_game_B", new Rect(0, 0.5f, 7, 2.5f), Vector2.zero, "Leave Channel", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		leaveGameB.moveText(new Vector2(-2, 0));
		leaveGameB.OnButtonPressed = new MGUIButton.ButtonPressed(onLeaveGameBPressed);
		menuButtons.Add(leaveGameB);
		
		MGUIButton optionsB = (MGUIButton) core.gui.setButton("menu_options_B", new Rect(0, 5, 7, 2.5f), Vector2.zero, "Options", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		optionsB.OnButtonPressed = new MGUIButton.ButtonPressed(onOptionsBPressed);
		menuButtons.Add(optionsB);
		
		MGUIButton closeAppB = (MGUIButton) core.gui.setButton("menu_closeApp_B", new Rect(0, -4.5f, 7, 2.5f), Vector2.zero, "Close Program", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		closeAppB.moveText(new Vector2(-2, 0));
		closeAppB.OnButtonPressed = new MGUIButton.ButtonPressed(onCloseApplicationBPressed);
		menuButtons.Add(closeAppB);
		
		MGUIButton closeB = (MGUIButton) core.gui.setButton("menu_close_B", new Rect(0, -9, 7, 2.5f), Vector2.zero, "Cancel", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		closeB.OnButtonPressed = new MGUIButton.ButtonPressed(onCloseBPressed);
		menuButtons.Add(closeB);
		
		Visible = false;
	}
	
	void onLeaveGameBPressed(MGUIButton but)
	{
		if(core.networkManager.currentChannel.Equals("Public"))
		{
			core.networkManager.send(ServerEventType.playerLeave, string.Empty);
		}
		else
		{
			//join the Public channel.
			Hashtable roomRequest = new Hashtable();
			roomRequest.Add("name", "Public");
			
			core.networkManager.send(ServerEventType.playerJoin, HashMapSerializer.hashMapToData(roomRequest));
		}
		Visible = false;
	}
	
	void onCloseApplicationBPressed(MGUIButton but)
	{
		Application.Quit();
	}
	
	void onOptionsBPressed(MGUIButton but)
	{
		core.options.Visible = true;
		Visible = false;
	}
	
	void onCloseBPressed(MGUIButton but)
	{
		Visible = false;
	}
}

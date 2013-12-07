using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Options : MonoBehaviour 
{
	public UICore core;
	
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
			
			foreach(MGUIElement b in elements)
				b.Visible = value;
		}
	}
	
	public Texture2D bgImage;
	
	MGUIImage bg;
	List<MGUIElement> elements = new List<MGUIElement>();
	// Use this for initialization
	void Start () 
	{
		MGUIText title = (MGUIText) core.gui.setText("options_title", new Rect(-3, 10f, 0, 0), "Options", core.normalFont, Color.white);
		elements.Add(title);
		
		MGUIButton closeB = (MGUIButton) core.gui.setButton("options_close_B", new Rect(9, -12f, 5, 1.5f), Vector2.zero, "Cancel", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		closeB.OnButtonPressed = new MGUIButton.ButtonPressed(onCloseBPressed);
		elements.Add(closeB);
		
		string fullScreenInfo = "Enable Full Screen";
		
		if(Screen.fullScreen)
			fullScreenInfo = "Disable Full Screen";
		
		MGUIButton toggleFullscreenB = (MGUIButton) core.gui.setButton("options_toggleFullscreenB", new Rect(0, 5f, 14, 2.5f), Vector2.zero, fullScreenInfo, core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		toggleFullscreenB.OnButtonPressed = new MGUIButton.ButtonPressed(onToggleFullscreenBPressed);
		elements.Add(toggleFullscreenB);
		
		MGUIText resInfo = (MGUIText) core.gui.setText("options_resInfo", new Rect(-5, 2f, 0, 0), "Resolutions:", core.normalFont, Color.white);
		elements.Add(resInfo);
		
		Resolution[] resolutions = Screen.resolutions;
		
		for (int i=0; i<resolutions.Length; i++) 
		{
			Resolution res = resolutions[i];
			MGUIButton tmpRes = (MGUIButton) core.gui.setButton("options_res_"+res.width+"_"+res.height, new Rect(0, -1f-(2*i), 10, 1.5f), Vector2.zero, res.width+"x"+res.height, core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
			tmpRes.OnButtonPressed = new MGUIButton.ButtonPressed(setResolution);
			tmpRes.custom = res;
			elements.Add(tmpRes);
		}
		
		int resFact = resolutions.Length-4;
		
		if(resFact<0)
			resFact = 0;
		
		bg = (MGUIImage) core.gui.setImage("options_BG", new Rect(0, -1.5f+resFact*2, 18, 15+resFact), Vector2.zero, bgImage);
		bg.setDepth(0.5f);
		
		move(new Vector2(0, resFact));
		
		Visible = false;
	}
	
	void setResolution(MGUIButton but)
	{
		Screen.SetResolution (((Resolution)but.custom).width, ((Resolution)but.custom).height, Screen.fullScreen);
		
	}
	
	void onToggleFullscreenBPressed(MGUIButton but)
	{
		Screen.fullScreen = !Screen.fullScreen;
		
		if(Screen.fullScreen)
			but.Text = "Disable Full Screen";
		else
			but.Text = "Enable Full Screen";
	}
	
	void onCloseBPressed(MGUIButton but)
	{
		Visible = false;
	}
	
	void move(Vector2 movement)
	{
		//bg._container.transform.Translate(new Vector3(movement.x, movement.y, 0));
		foreach(MGUIElement b in elements)
			b._container.transform.Translate(new Vector3(movement.x, movement.y, 0));
	}
}

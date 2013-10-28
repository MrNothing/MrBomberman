using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ErrorInterface : MonoBehaviour 
{
	public UICore core;
	MGUI gui;
	
	MGUIImage bgImage;
	MGUITextArea textArea;
	MGUIButton closeButton;
	
	// Use this for initialization
	void Start () 
	{
		gui = core.gui;
		
		bgImage = (MGUIImage) gui.setImage("bgImage", new Rect(0, 0, 10, 6), Vector2.zero, core.ButtonNormal);
		textArea = (MGUITextArea) gui.setTextArea("errTextArea", new Rect(-9, 4, 10, 5), 7, 12, "", core.normalFont, Color.white);
		closeButton = (MGUIButton) gui.setButton("errButton", new Rect(0, -3, 5, 2f), Vector2.zero, "Close", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		closeButton.OnButtonPressed += new MGUIButton.ButtonPressed(hide);
		
		bgImage.setDepth(-0.5f);
		closeButton.setDepth(-0.6f);
		
		bgImage.Visible = false;
		textArea.Visible = false;
		closeButton.Visible = false;
	}
	
	public void showMessage(string message, Color color, bool showCloseButton)
	{
		textArea.clear();
		gui.insertText(textArea.id, message, core.normalFont, color);
		bgImage.Visible = true;
		textArea.Visible = true;
		
		if(showCloseButton)
			closeButton.Visible = true;
		else
			closeButton.Visible = false;
	}
	
	public void hide(MGUIButton key)
	{
		bgImage.Visible = false;
		textArea.Visible = false;
		closeButton.Visible = false;
	}
}

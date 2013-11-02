using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DialogAction
{
	//do nothing
	none, 
	
	//close the dialog Interface
	cancel,
	
	//joins the channel i was invited to
	joinChannel,
	
	//joins the game i was disconnected from
	joinActiveGame,
	
	//declines the invitation to join an active game
	declineActiveGame,
}

public class DialogOption
{
	public string label;
	public DialogAction action;
	public Object contextObject;
	
	public DialogOption(string _label, DialogAction _action, Object _contextObject)
	{
		label = _label;
		action = _action;
		contextObject = _contextObject;
	}
}

public class ErrorInterface : MonoBehaviour 
{
	public UICore core;
	MGUI gui;
	
	MGUIImage bgImage;
	MGUITextArea textArea;
	MGUIButton closeButton;
	List<MGUIButton> dialogButtons = new List<MGUIButton>();
	
	// Use this for initialization
	void Start () 
	{
		gui = core.gui;
		
		bgImage = (MGUIImage) gui.setImage("bgImage", new Rect(0, 0, 10, 6), Vector2.zero, core.ButtonNormal);
		textArea = (MGUITextArea) gui.setTextArea("errTextArea", new Rect(-9, 4, 10, 5), 7, 12, "", core.normalFont, Color.white);
		closeButton = (MGUIButton) gui.setButton("errButton", new Rect(0, -3, 5, 2f), Vector2.zero, "Close", core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover);
		closeButton.OnButtonPressed += new MGUIButton.ButtonPressed(hide);

		//bgImage.setDepth(-0.5f);
		//closeButton.setDepth(-0.6f);
		
		bgImage.Visible = false;
		textArea.Visible = false;
		closeButton.Visible = false;
	}
	
	void destroyDialogButtons()
	{
		foreach(MGUIButton b in dialogButtons)
			core.gui.removeElement(b.id);
	}
	
	public void showMessage(string message, Color color, bool showCloseButton)
	{
		destroyDialogButtons();
		textArea.clear();
		gui.insertText(textArea.id, message, core.normalFont, color);
		bgImage.Visible = true;
		textArea.Visible = true;
		
		if(showCloseButton)
			closeButton.Visible = true;
		else
			closeButton.Visible = false;
	}
	
	public void showMessage(string message, Color color, DialogOption[] options)
	{
		destroyDialogButtons();
		textArea.clear();
		gui.insertText(textArea.id, message, core.normalFont, color);
		bgImage.Visible = true;
		textArea.Visible = true;
		closeButton.Visible = false;
		
		for(int i=0; i<options.Length; i++)
		{
			dialogButtons.Add((MGUIButton) gui.setButton("dialogB_"+i, new Rect(-4.6f+i*9, -3, 4, 2f), Vector2.zero, options[i].label, core.normalFont, Color.white, core.ButtonNormal, core.ButtonDown, core.ButtonHover));
			dialogButtons[i].custom = options[i];
			dialogButtons[i].setDepth(-0.5f);
			dialogButtons[i].OnButtonPressed += new MGUIButton.ButtonPressed(onDialogPressed);
			dialogButtons[i].moveText(new Vector2(-0.7f, 0));
		}
	}
	
	public void hide(MGUIButton key)
	{
		bgImage.Visible = false;
		textArea.Visible = false;
		closeButton.Visible = false;
		destroyDialogButtons();
	}
	
	void onDialogPressed(MGUIButton button)
	{
		DialogOption option = (DialogOption)button.custom;
		
		if(option.action==DialogAction.cancel)
			hide(null);
		
		if(option.action==DialogAction.joinActiveGame)
		{
			core.networkManager.send(ServerEventType.joinActiveGame, string.Empty);
			hide(null);
		}
		
		if(option.action==DialogAction.declineActiveGame)
		{
			core.networkManager.send(ServerEventType.declineActiveGame, string.Empty);
			hide(null);
		}
		
		if(option.action==DialogAction.joinChannel)
		{
			
		}
	}
}

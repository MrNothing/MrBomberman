using UnityEngine;
using System.Collections;

/*
 * This script contains most of the assets for creating GUI elements using MGUI
 * */
public class UICore : MonoBehaviour 
{
	
	public MGUI gui;
	public NetworkManager networkManager;
	public IOManager io = new IOManager();
	
	public ErrorInterface errorInterface;
	public StartScreen startScreen;
	public MainLobby lobby;
	public GameCreator gameCreator;
	public GamesList gamesList;
	public GameLobby gameLobby;
	
	public InGame inGame;
	
	public GameManager gameManager;
	public SpellsManager spellsManager;
	
	public Menu menu;
	public Options options;
	
	public Font normalFont;
	public Texture2D ButtonNormal;
	public Texture2D ButtonHover;
	public Texture2D ButtonDown;
	
	public Texture2D blackAlphaBg;
	public Texture2D whiteAlphaBg;
	
	public Transform gameCameraTransform;
	
	void Start()
	{
		gameCameraTransform = GameObject.Find("Main Camera").transform;
	}
	
	void Update()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			menu.Visible = true;
		}
	}
	/*
	 *  These are just tests for the MGUI class
	// Use this for initialization
	void Start () 
	{
		//tests:
		
		//text
			//gui.setText("text1", new Rect(-2, 2, 0, 0), "Blablabala", testFont, Color.red);
		
		//image
			//gui.setImage("image1", new Rect(5, 6, 15, 4), new Vector3(0, 0), blackAlphaBg);
		
		//button
			MGUIButton button = (MGUIButton)gui.setButton("but1", new Rect(10+5, 0, 5, 1.5f), new Vector2(0, 0), "Send", normalFont, Color.green, ButtonNormal, ButtonDown, ButtonHover);
			button.OnButtonPressed += new MGUIButton.ButtonPressed(onButton1pressed);
		
		//textfield
			gui.setTextField("textField", new Rect(0, 0, 10, 1.5f), new Vector3(0, 0), "tappez du texte ici...", normalFont, Color.white, ButtonNormal, ButtonDown, ButtonHover);
		
		//textarea
			gui.setTextArea("textArea1", new Rect(-10, 10, 0, 0), 5, 30, "Bienvenue dans le premier prototype de bomberman utilisant MGUI", normalFont, Color.cyan);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.Return))
		{
			if((gui.getElement("textField") as MGUITextfield).Text.Length>0 && !(gui.getElement("textField") as MGUITextfield).Text.Equals(" "))
			{
				gui.insertText("textArea1", (gui.getElement("textField") as MGUITextfield).Text, normalFont, Color.red);
				(gui.getElement("textField") as MGUITextfield).Text = "";
			}
		}
	}
	
	void onButton1pressed(int key)
	{
		if((gui.getElement("textField") as MGUITextfield).Text.Length>0)
		{
			gui.insertText("textArea1", (gui.getElement("textField") as MGUITextfield).Text, normalFont, Color.red);
			(gui.getElement("textField") as MGUITextfield).Text = "";
		}
	}
	*/
}

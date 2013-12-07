using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGame : MonoBehaviour 
{
	
	public Texture2D unitInfosBg;
	public Texture2D unitAvatarBg;
	public Texture2D unitSpellsBg;
	bool _visible = false;
	
	public UICore core;
	public TopDownCameraController rtsCameraController;
	public Light gamelight;
	
	public GameObject selectionCircle;
	public Color controlledColor;
	public Color allyColor;
	public Color neutralColor;
	public Color foeColor;
	
	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			gamelight.enabled = value;
			rtsCameraController.enabled = value;
			textField.Visible = value;
			textArea.Visible = value;
			UnitInfosName.Visible = value;
			UnitInfosAdditionalInfos.Visible = value;
			UnitInfosMp.Visible = value;
			UnitInfosHp.Visible = value;
			UnitAvatarBg.Visible = value;
			UnitAvatarBg2.Visible = value;
			UnitAvatar.Visible = value;
			UnitInfosBg.Visible = value;
			UnitInfosSpellBg.Visible = value;
			
			foreach(MGUIButton b in spells)
				b.Visible = value;
			
			spellInfosHover.Visible = value;
			
			enabled = value;
			_visible = value;
		}
	}
	
	public List<Entity> selectedEntities = new List<Entity>();
	public int activeEntity = 0;
	
	MGUITextfield textField;
	public MGUITextArea textArea;
	
	MGUIText UnitInfosName;
	MGUIText UnitInfosAdditionalInfos;
	
	MGUIText UnitInfosMp;
	MGUIText UnitInfosHp;
	
	MGUIImage UnitAvatarBg;
	MGUIImage UnitAvatarBg2;
	public MGUIImage UnitAvatar;
	MGUIImage UnitInfosBg;
	MGUIImage UnitInfosSpellBg;
	
	MGUITextArea spellInfosHover;
	
	List<MGUIButton> spells = new List<MGUIButton>();
	List<MGUIButton> entities = new List<MGUIButton>();
	public Vector2[] formations;
	
	//TODO, add an ingame interface for selected units and menu elements
	//TODO, add a minimap
	
	// Use this for initialization
	void Start () 
	{
		
		string testText = "Ingame interface";
		textArea = (MGUITextArea) core.gui.setTextArea("gameChatTextArea", new Rect(-30, 12.5f, 0, 0), 10, 30, testText, core.normalFont, Color.white);
		textField = (MGUITextfield) core.gui.setTextField("gameChatTextField", new Rect(-15, -4.55f, 15, 1.5f), Vector2.zero, string.Empty, core.normalFont, Color.white, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg);
		textArea.setDepth(1);
		textField.setDepth(1);
		
		UnitInfosBg = (MGUIImage) core.gui.setImage("unitInfosBg", new Rect(0, -17, 15, 9), Vector2.zero, unitInfosBg);
		UnitAvatarBg = (MGUIImage) core.gui.setImage("UnitAvatarBg", new Rect(-20.7f, -12.4f, 6, 6), Vector2.zero, unitAvatarBg);
		UnitAvatarBg2 = (MGUIImage) core.gui.setImage("UnitAvatarBg2", new Rect(-20.7f, -21.7f, 6, 6), Vector2.zero, unitAvatarBg);
		UnitAvatarBg.setDepth(11);
		UnitAvatarBg2.setDepth(10);
		UnitAvatar = (MGUIImage) core.gui.setImage("UnitAvatar", new Rect(-20.7f, -12.4f, 4, 4), Vector2.zero, core.blackAlphaBg);
		
		UnitInfosName = (MGUIText) core.gui.setText("UnitInfosName", new Rect(-12.66f, -11.37f, 0, 0), "UnitName", core.normalFont, Color.white);
		UnitInfosAdditionalInfos = (MGUIText) core.gui.setText("UnitInfosAdditionalInfos", new Rect(-12.66f, -12.5f, 0, 0), "Level ??? Unit", core.normalFont, Color.cyan);
		
		UnitInfosHp = (MGUIText) core.gui.setText("UnitInfosHp", new Rect(-25.42f, -16.9f, 0, 0), "9999/9999", core.normalFont, Color.green);
		UnitInfosMp = (MGUIText) core.gui.setText("UnitInfosMp", new Rect(-25.42f, -18.5f, 0, 0), "9999/9999", core.normalFont, Color.blue);
		
		UnitInfosSpellBg = (MGUIImage) core.gui.setImage("UnitInfosSpellBg", new Rect(25.2f, -14, 15, 9), Vector2.zero, unitSpellsBg);
		UnitInfosSpellBg.setDepth(2);
		
		spellInfosHover = (MGUITextArea) core.gui.setTextArea("spellInfosHover", new Rect(10.6f, 1, 0, 0), 6, 15, "", core.normalFont, Color.white);
		
		Visible = false;
	}
	
	bool showChat = false;
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Return))
		{
			if(textField.Text.Length>0 && !textField.Text.Equals(" "))
			{
				core.lobby.parseCommands(textField.Text, textArea);
				textField.Text = "";
			}
		}
	}
	
	[HideInInspector]
	public bool forceReload=false;
	
	int lastEntitiesSize = 0;
	
	void LateUpdate()
	{
		if(selectedEntities.Count>1)
		{
			if(activeEntity>=selectedEntities.Count)
				activeEntity = selectedEntities.Count-1;
				
			if(!UnitAvatar.Texture.Equals(selectedEntities[activeEntity].icon) || lastEntitiesSize!=selectedEntities.Count || forceReload)
			{	
				lastEntitiesSize = selectedEntities.Count;
				resetSelectionCricles();
				resetInterface();
				
				int counterX = 0;
				int counterY = 0;
				
				for(int i=0; i<selectedEntities.Count; i++)
				{
					Entity entity = selectedEntities[i];
					
					float scale = 1;
					
					if(i==activeEntity)
						scale = 1.3f;
					
					MGUIButton tmpBut = (MGUIButton)core.gui.setButton("selectedUnit_"+i, new Rect(counterX*4-10.75356f,counterY*4-12.9444f, 1.8f*scale, 1.8f*scale), Vector2.zero, "", core.normalFont, Color.white, entity.icon, entity.icon, entity.icon); 
					tmpBut.custom = i;
					tmpBut.OnButtonPressed+=new MGUIButton.ButtonPressed(onEntityIconPressed);
					tmpBut.setDepth(-0.5f);
					entities.Add(tmpBut);
					
					if(counterX>3)
					{
						counterX=0;
						counterY--;
					}
					else
						counterX++;
					GameObject newSelectionCricle = (GameObject) Instantiate(selectionCircle);
					core.gameManager.selectionCircles.Add(newSelectionCricle);
					core.gameManager.selectionCircles[i].transform.localScale = Vector3.one*((selectedEntities[i].collider.bounds.size.x/0.5f)*2.4f);
					core.gameManager.selectionCircles[i].transform.position = selectedEntities[i].transform.position+new Vector3(0, 0.1f, 0);
					core.gameManager.selectionCircles[i].transform.parent = selectedEntities[i].transform;
				
					if(selectedEntities[i].owner.Equals(core.networkManager.username))
					{
						newSelectionCricle.renderer.material.SetColor("_TintColor", controlledColor);
					}
					else
					{
						if(selectedEntities[i].team.Equals(core.networkManager.currentTeam))
							newSelectionCricle.renderer.material.SetColor("_TintColor", allyColor);
						else
						{
							if(selectedEntities[i].agressive)
								newSelectionCricle.renderer.material.SetColor("_TintColor", foeColor);
							else
								newSelectionCricle.renderer.material.SetColor("_TintColor", neutralColor);
						}	
					}
					
				}
				
				UnitAvatar.Texture = selectedEntities[activeEntity].icon;
				
				UnitInfosHp.Text = selectedEntities[activeEntity].hp+"/"+selectedEntities[activeEntity].getMaxHp();
				UnitInfosMp.Text = selectedEntities[activeEntity].mp+"/"+selectedEntities[activeEntity].getMaxMp();
				
				
				if(!forceReload)
					reloadSpellsUI();
			}
		}
		else if(selectedEntities.Count==1)
		{
			if(!UnitAvatar.Texture.Equals(selectedEntities[0].icon) || forceReload)
			{	
				resetSelectionCricles();
				activeEntity = 0;
				
				foreach(MGUIButton b in entities)
					core.gui.removeElement(b.id);
				
				entities.Clear();
				
				UnitAvatar.Texture = selectedEntities[0].icon;
			
				UnitInfosName.Text = selectedEntities[0].getName();
				UnitInfosAdditionalInfos.Text = "Level "+selectedEntities[0].getLevel();
				UnitInfosHp.Text = selectedEntities[0].hp+"/"+selectedEntities[0].getMaxHp();
				UnitInfosMp.Text = selectedEntities[0].mp+"/"+selectedEntities[0].getMaxMp();
				
				GameObject newSelectionCricle = (GameObject) Instantiate(selectionCircle);
				
				if(selectedEntities[0].owner.Equals(core.networkManager.username))
				{
					newSelectionCricle.renderer.material.SetColor("_TintColor", controlledColor);
				}
				else
				{
					if(selectedEntities[0].team.Equals(core.networkManager.currentTeam))
						newSelectionCricle.renderer.material.SetColor("_TintColor", allyColor);
					else
					{
						if(selectedEntities[0].agressive)
							newSelectionCricle.renderer.material.SetColor("_TintColor", foeColor);
						else
							newSelectionCricle.renderer.material.SetColor("_TintColor", neutralColor);
					}	
				}
				
				core.gameManager.selectionCircles.Add(newSelectionCricle);
				
				core.gameManager.selectionCircles[0].transform.localScale = Vector3.one*((selectedEntities[0].collider.bounds.size.x/0.5f)*2.4f);
				core.gameManager.selectionCircles[0].transform.position = selectedEntities[0].transform.position+new Vector3(0, 0.1f, 0);
				core.gameManager.selectionCircles[0].transform.parent = selectedEntities[0].transform;
				
				if(!forceReload)
					reloadSpellsUI();
				
				forceReload = false;
				
				}
		}
		else
		{
			if(!UnitAvatar.Texture.Equals(core.blackAlphaBg))
			{
				resetInterface();
			}
		}
	}
	
	void resetInterface()
	{
		foreach(MGUIButton b in entities)
			core.gui.removeElement(b.id);
		
		entities.Clear();
		
		UnitAvatar.Texture = core.blackAlphaBg;
	
		UnitInfosName.Text = "";
		UnitInfosAdditionalInfos.Text = "";
		UnitInfosHp.Text = "";
		UnitInfosMp.Text = "";
		resetSelectionCricles();
		
		foreach(MGUIButton b in spells)
			core.gui.removeElement(b.id);
		
		spells.Clear();
	}
	
	void resetSelectionCricles()
	{
		foreach(GameObject o in core.gameManager.selectionCircles)
		{
			Destroy(o);
		}
		
		core.gameManager.selectionCircles.Clear();
		
	}
	
	public void applyTeams(Dictionary<string, int> playersByTeam)
	{
		/*clearTeams();
		
		foreach(string s in playersByTeam.Keys)
		{
			MGUIButton playerInfos = (MGUIButton)core.gui.setButton(s+"_tInfos", new Rect(0, -10, 15, 1.5f), Vector3.zero, s, core.normalFont, Color.cyan, core.blackAlphaBg, core.blackAlphaBg, core.blackAlphaBg);
			playerInfos.moveText(new Vector2(-2, 0));
			playerInfos.OnButtonPressed = new MGUIButton.ButtonPressed(onPlayerPressed);
			teams[playersByTeam[s]].Add(playerInfos);
		}
		
		updateTeams();*/
	}
	
	void onSpellPressed(MGUIButton button)
	{
		Spell mySpell = (Spell)button.custom;
		
		if(mySpell.Usage<=3) //if this is a regular spell
			core.spellsManager.activateSpell((Spell)button.custom);
		
		if(mySpell.Usage == (int)SpellUsage.cancel)
			reloadSpellsUI();
		
		if(mySpell.Usage == (int)SpellUsage.showBuildUI)
			showBuildingUI();
		
		if(mySpell.Usage == (int)SpellUsage.build
			|| mySpell.Usage == (int) SpellUsage.invokeUnit
			|| mySpell.Usage == (int) SpellUsage.buyItem)
			core.spellsManager.activateSpell((Spell)button.custom);
	}
	
	void onEntityIconPressed(MGUIButton button)
	{
		if(activeEntity!=(int)button.custom)
		{
			activeEntity = (int)button.custom;
			UnitAvatar.Texture = core.blackAlphaBg;
		}
		else
		{
			Entity chosenEntity = selectedEntities[(int)button.custom];
			selectedEntities.Clear();
			selectedEntities.Add(chosenEntity);
			UnitAvatar.Texture = core.blackAlphaBg;
			activeEntity = 0;
		}
	}
	
	void reloadSpellsUI()
	{
		foreach(MGUIButton b in spells)
			core.gui.removeElement(b.id);
				
		spells.Clear();
				
		if(selectedEntities[activeEntity].owner.Equals(core.networkManager.username))
		{
			int counterX = 0;
			int counterY = 0;
			
			foreach(Spell s in selectedEntities[activeEntity]._spells)
			{
				MGUIButton tmpSpell = (MGUIButton)core.gui.setButton("spell_"+s.Name, new Rect(counterX*4+14.13f,counterY*4-9.97f, 1.8f, 1.8f), Vector2.zero, "", core.normalFont, Color.white, s.Icon, s.Icon, s.Icon); 
				tmpSpell.custom = s;
				tmpSpell.setDepth(1);
				tmpSpell.OnButtonPressed += new MGUIButton.ButtonPressed(onSpellPressed);
				tmpSpell.OnMouseOver += new MGUIButton.MouseOver(onMouseOverSpell);
				tmpSpell.OnMouseOut += new MGUIButton.MouseOut(onMouseOutOfSpell);
				tmpSpell.captureMouseOver = true;
				spells.Add(tmpSpell);
				counterX++;
			}
			
			if(selectedEntities[activeEntity]._buildings.Count>0)
			{
				Spell buildSpell = new Spell("build_Humans", "Build", "Construct a building", SpellUsage.showBuildUI);
				MGUIButton tmpSpell = (MGUIButton)core.gui.setButton("spell_Build", new Rect(counterX*4+14.13f,counterY*4-9.97f, 1.8f, 1.8f), Vector2.zero, "", core.normalFont, Color.white, buildSpell.Icon, buildSpell.Icon, buildSpell.Icon); 
				tmpSpell.custom = buildSpell;
				tmpSpell.setDepth(1);
				tmpSpell.OnButtonPressed += new MGUIButton.ButtonPressed(onSpellPressed);
				tmpSpell.OnMouseOver += new MGUIButton.MouseOver(onMouseOverSpell);
				tmpSpell.OnMouseOut += new MGUIButton.MouseOut(onMouseOutOfSpell);
				tmpSpell.captureMouseOver = true;
				spells.Add(tmpSpell);
			}
		}
	}
	
	void showBuildingUI()
	{
		foreach(MGUIButton b in spells)
			core.gui.removeElement(b.id);
				
		spells.Clear();
		
		int counterX = 0;
		int counterY = 0;
		foreach(Spell s in selectedEntities[activeEntity]._buildings)
		{
			MGUIButton tmpSpell = (MGUIButton)core.gui.setButton("spell_"+s.Name, new Rect(counterX*4+14.13f,counterY*4-9.97f, 1.8f, 1.8f), Vector2.zero, "", core.normalFont, Color.white, s.Icon, s.Icon, s.Icon); 
			tmpSpell.custom = s;
			tmpSpell.setDepth(1);
			tmpSpell.OnButtonPressed += new MGUIButton.ButtonPressed(onSpellPressed);
			tmpSpell.OnMouseOver += new MGUIButton.MouseOver(onMouseOverSpell);
			tmpSpell.OnMouseOut += new MGUIButton.MouseOut(onMouseOutOfSpell);
			tmpSpell.captureMouseOver = true;
			spells.Add(tmpSpell);
			counterX++;
		}
		
		{
			Spell cancelSpell = new Spell("cancel", "Cancel", "Return", SpellUsage.cancel);
			MGUIButton tmpSpell = (MGUIButton)core.gui.setButton("spell_cancel", new Rect(counterX*4+14.13f,counterY*4-9.97f, 1.8f, 1.8f), Vector2.zero, "", core.normalFont, Color.white, cancelSpell.Icon, cancelSpell.Icon, cancelSpell.Icon); 
			tmpSpell.custom = cancelSpell;
			tmpSpell.setDepth(1);
			tmpSpell.OnButtonPressed += new MGUIButton.ButtonPressed(onSpellPressed);
			tmpSpell.OnMouseOver += new MGUIButton.MouseOver(onMouseOverSpell);
			tmpSpell.OnMouseOut += new MGUIButton.MouseOut(onMouseOutOfSpell);
			tmpSpell.captureMouseOver = true;
			spells.Add(tmpSpell);
		}
	}
	
	void onMouseOverSpell(MGUIButton button)
	{
		spellInfosHover.clear();
		Spell spell = (Spell)button.custom;
		core.gui.insertText(spellInfosHover.id, spell.Name, core.normalFont, Color.white); 
		core.gui.insertText(spellInfosHover.id, spell.Description, core.normalFont, Color.cyan); 
	}
	
	void onMouseOutOfSpell(MGUIButton button)
	{
		spellInfosHover.clear();
	}
}

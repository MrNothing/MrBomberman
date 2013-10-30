/*
 *	MGUI: Musarais GUI by Boris Musarais, this code may not be used without the approval of its author.
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This file contains all classes required to use MGUI, attach this to a GameObject and keep a refrence to MGUI 
 * recommended fuctions: setText, setTextField, setTextArea, setButton, setImage, insertText
 * */

public enum MGUIType
{
	element, text, textField, image, button, textArea
}

public class MGUIElement
{
	string _id;
	public MGUIType _type;
	public GameObject _container;
	Rect _bounds;
	bool _visible = true;
	
	//a custom container, usefull sometimes...
	public System.Object custom;
	
	public MGUIElement(string id, Rect bounds, GameObject container)
	{
		_id = id;
		_container = container;
		_bounds = bounds;
		_type = MGUIType.element;
	}
	
	public string id 
	{
		get 
		{
			return this._id;
		}
		set 
		{
			//id may not be modified
			return;
		}
	}
	
	public MGUIType Type 
	{
		get {
			return this._type;
		}
		set {
			//type may not be modified
			return;
		}
	}
	
	public GameObject Container 
	{
		get 
		{
			return this._container;
		}
		set 
		{
			_container = value;
		}
	}
	
	public Rect Bounds 
	{
		get 
		{
			return this._bounds;
		}
		set 
		{
			Container.transform.localPosition = new Vector3(value.x, value.y, Container.transform.position.z);
			_bounds = value;
		}
	}
	
	public bool Visible 
	{
		get 
		{
			return this._visible;
		}
		set 
		{
			//if i have a renderer, toggle its status based on "visible"'s value
			try
			{
				Container.renderer.enabled = value;
			}
			catch
			{
				
			}
			
			if(Type==MGUIType.button)
			{
				((MGUIButton)this)._textContainer.renderer.enabled = value;
				((MGUIButton)this)._bgContainer.renderer.enabled = value;
			}
			
			if(Type==MGUIType.textField)
			{
				((MGUITextfield)this)._textContainer.renderer.enabled = value;
				((MGUITextfield)this)._bgContainer.renderer.enabled = value;
			}
			
			if(Type==MGUIType.textArea)
			{
				((MGUITextArea)this).setVisibility(value);
			}
			
			_visible = value;
		}
	}
	
	public void setDepth(float depth)
	{
		_container.transform.localPosition = _container.transform.localPosition+new Vector3(0, 0, depth);
	}
	
	public void setPosition(float x, float y)
	{
		setPosition(new Vector2(x, y));
	}
	
	public void setPosition(Vector2 position)
	{
		_container.transform.localPosition = new Vector3(position.x, position.y, _container.transform.localPosition.z);
	}
}

public class MGUIText:MGUIElement
{
	string _text;
	public GameObject _textContainer;

	public MGUIText(string id, Rect bounds, GameObject container, GameObject textContainer, string text):base(id, bounds, container)
	{
		_textContainer = textContainer;
		_text = text;
		_type = MGUIType.text;
	}
	
	public string Text 
	{
		get {
			return this._text;
		}
		set {
			_textContainer.GetComponent<TextMesh>().text = value;
			_text = value;
		}
	}
	
	public void moveText(Vector2 offset)
	{
		_textContainer.transform.Translate(offset);
	}
}

public class MGUITextArea:MGUIElement
{
	public List<MGUIText> Text;
	
	//height is an integer since it is based on lines...
	int _height;
	float _width;
	public float lineHeight=1.5f;

	public MGUITextArea(string id, Rect bounds, GameObject container, int height, float width, MGUIText firstText):base(id, bounds, container)
	{
		_height = height;
		_width = width;
		
		Text = new List<MGUIText>();
		
		if(firstText!=null)
			Text.Add(firstText);
		
		_type = MGUIType.textArea;
	}
	
	public void refreshText()
	{
		//show visible lines only
		
		int startIndex = Text.Count-_height;
		
		if(startIndex<0)
			startIndex = 0;
		
		for(int i=0; i<startIndex; i++)
		{
			Text[i].Container.renderer.enabled = false;
		}
		
		for(int i=startIndex; i<Text.Count; i++)
		{
			Text[i].Container.renderer.enabled = true;
			Text[i].Container.transform.position = Container.transform.position+new Vector3(0, -(i-startIndex)*lineHeight, 0);
		}
	}
	
	public void setVisibility(bool status)
	{
		if(!status)
		{
			for(int i=0; i<Text.Count; i++)
			{
				Text[i].Container.renderer.enabled = status;
			}
		}
		else
		{
			refreshText();
		}
	}
	
	public void clear()
	{
		for(int i=0; i<Text.Count; i++)
		{
			UnityEngine.GameObject.Destroy(Text[i].Container);
		}
		
		Text.Clear();
	}
	
	public int Height 
	{
		get 
		{
			return this._height;
		}
		set 
		{
			_height = value;
		}
	}

	public float Width 
	{
		get 
		{
			return this._width;
		}
		set 
		{
			_width = value;
		}
	}	
}

public class MGUITextfield:MGUIText
{
	public bool mouseOver = false;
	public bool focused = false;
	
	Texture2D _normal;
	Texture2D _onMouseOver;
	Texture2D _onMouseDown;
	
	public GameObject _bgContainer;
	
	public MGUITextfield(string id, Rect bounds, GameObject container, GameObject bgContainer, GameObject textContainer, string text, Texture2D normal, Texture2D onMouseOver, Texture2D onMouseDown):base(id, bounds, container, textContainer, text)
	{
		_normal = normal;
		_onMouseDown = onMouseDown;
		_onMouseOver = onMouseOver;
		_bgContainer = bgContainer;
		
		_type = MGUIType.textField;
	}
	
	public void triggerNormalImage()
	{
		focused = false;
		_bgContainer.renderer.material.SetTexture("_MainTex", _normal);
	}
	
	public void triggerMouseOverImage()
	{
		_bgContainer.renderer.material.SetTexture("_MainTex", _onMouseOver);
		mouseOver = true;
	}
	
	public void triggerMouseOut()
	{
		if(mouseOver)
		{
			if(!focused)
				triggerNormalImage();
			
			mouseOver = false;
		}
	}
	
	public void triggerMouseDown(int button)
	{
		focused = true;
		_bgContainer.renderer.material.SetTexture("_MainTex", _onMouseDown);	
	}
}

public class MGUIButton:MGUIText
{
	public bool activated = true;
	public bool mouseOver = false;
	Texture2D _normal;
	Texture2D _onMouseOver;
	Texture2D _onMouseDown;
	
	public GameObject _bgContainer;
	
	public delegate void ButtonPressed(MGUIButton button);
	public ButtonPressed OnButtonPressed;
	
	public MGUIButton(string id, Rect bounds, GameObject container, GameObject bgContainer, GameObject textContainer, string text, Texture2D normal, Texture2D onMouseOver, Texture2D onMouseDown):base(id, bounds, container, textContainer, text)
	{	
		_normal = normal;
		_onMouseDown = onMouseDown;
		_onMouseOver = onMouseOver;
		_bgContainer = bgContainer;
		_type = MGUIType.button;
	}
	
	void raiseButtonPressed(MGUIButton button)
	{
		OnButtonPressed(button);
	}
	
	public void triggerNormalImage()
	{
		_bgContainer.renderer.material.SetTexture("_MainTex", _normal);
	}
	
	public void triggerMouseOverImage()
	{
		if(activated)
			_bgContainer.renderer.material.SetTexture("_MainTex", _onMouseOver);
		
		mouseOver = true;
	}
	
	public void triggerMouseOut()
	{
		if(mouseOver)
		{
			triggerNormalImage();
			mouseOver = false;
		}
	}
	
	public void triggerMouseDown(int button)
	{
		if(activated)
		{
			raiseButtonPressed(this);
			_bgContainer.renderer.material.SetTexture("_MainTex", _onMouseDown);
		}
	}
}

public class MGUIImage:MGUIElement
{
	public Texture2D _texture;

	public Texture2D Texture 
	{
		get 
		{
			return this._texture;
		}
		set 
		{
			_container.renderer.material.SetTexture("_MainTex", value);
			_texture = value;
		}
	}	
	public MGUIImage(string id, Rect bounds, GameObject container, Texture2D texture):base(id, bounds, container)
	{
		_texture = texture;
		_type = MGUIType.image;
	}
}

public class MGUI : MonoBehaviour 
{
	
	public Camera GUICamera;
	public LayerMask guiLayers;
	public bool _enableMouseOverCheck = false;
	Dictionary<string, MGUIElement> elements = new Dictionary<string, MGUIElement>();
	
	string focus=string.Empty;
	
	// Use this for initialization
	void Start() 
	{
	
	}
	
	void LateUpdate()
	{
		//if the feature is enabled, check if mouse is over any button
		if(_enableMouseOverCheck)
			checkIfMouseIsOverAnyButton();
		
		//check if I am pressing or releasing any button or textfields
		if(Input.GetMouseButtonDown(1)) 
			checkIfIAmPressingAnyButton(1);
		
		if(Input.GetMouseButtonDown(0)) 
			checkIfIAmPressingAnyButton(0);
		
		if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
			checkIfIAmReleasingAnyButton(0);
		
		//if I am focusing a textfield, capture my keyboad input...
		if(focus.Length>0)
		{
			MGUIElement focusedElement = (MGUIElement) elements[focus];
			
			if(focusedElement.Visible)
			{	
				if(focusedElement.Type==MGUIType.textField)
				{
					MGUITextfield focusedField = (MGUITextfield) elements[focus];
					
					if(Input.anyKey)
					{
						if(Input.GetKey(KeyCode.Backspace))
						{
							if(focusedField.Text.Length>0)
								focusedField.Text = focusedField.Text.Remove(focusedField.Text.Length-1);
						}
						else
						{
							if(Input.inputString.Length>0 && !Input.GetKey(KeyCode.Return))
								focusedField.Text+=Input.inputString;
						}
					}
				}
				
				if(focusedElement.Type==MGUIType.button)
				{
					MGUIButton focusedButton = (MGUIButton) elements[focus];
					
					if(Input.inputString.Length>0)
					{
						if(Input.GetKey(KeyCode.Return))
						{
							//if i am focusing a button, click it
							focusedButton.triggerMouseDown(0);
						}
					}
				}
			}
		}
	}
	
	void checkIfMouseIsOverAnyButton()
	{
		foreach(string s in elements.Keys)
		{
			if(elements[s].Type==MGUIType.button && elements[s].Visible)
			{
				MGUIButton myButton = (MGUIButton) elements[s];
				
				//we get the position of the mouse in the world...
				Vector3 mousePosition = GUICamera.ScreenToWorldPoint(Input.mousePosition);
				
				//we need to get the position of each vertice so we can test if the mouse is in the rectangle...
				MeshFilter myFilter = myButton._bgContainer.GetComponent<MeshFilter>();
				
				Rect buttonBounds = new Rect(	
												myButton._bgContainer.transform.position.x-myFilter.mesh.vertices[0].x, 
												myButton._bgContainer.transform.position.y-myFilter.mesh.vertices[0].z, 
												myFilter.mesh.vertices[0].x-myFilter.mesh.vertices[3].x, 
												myFilter.mesh.vertices[0].z-myFilter.mesh.vertices[3].z
											);
				
				//Debug.Log("Button position: "+buttonBounds);
				//Debug.Log("Mouse position: "+mousePosition);
				
				if(buttonBounds.Contains(mousePosition))
					myButton.triggerMouseOverImage();
				else
					myButton.triggerMouseOut();
			}
			
			if(elements[s].Type==MGUIType.textField && elements[s].Visible)
			{
				MGUITextfield myButton = (MGUITextfield) elements[s];
				
				//we get the position of the mouse in the world...
				Vector3 mousePosition = GUICamera.ScreenToWorldPoint(Input.mousePosition);
				
				//we need to get the position of each vertice so we can test if the mouse is in the rectangle...
				MeshFilter myFilter = myButton._bgContainer.GetComponent<MeshFilter>();
				
				Rect buttonBounds = new Rect(	
												myButton._bgContainer.transform.position.x-myFilter.mesh.vertices[0].x, 
												myButton._bgContainer.transform.position.y-myFilter.mesh.vertices[0].z, 
												myFilter.mesh.vertices[0].x-myFilter.mesh.vertices[3].x, 
												myFilter.mesh.vertices[0].z-myFilter.mesh.vertices[3].z
											);
				
				//Debug.Log("Button position: "+buttonBounds);
				//Debug.Log("Mouse position: "+mousePosition);
				
				if(buttonBounds.Contains(mousePosition))
					myButton.triggerMouseOverImage();
				else
					myButton.triggerMouseOut();
			}
		}
	}
	
	void checkIfIAmPressingAnyButton(int mouseButton)
	{
		foreach(string s in elements.Keys)
		{
			if(elements[s].Type==MGUIType.button && elements[s].Visible)
			{
				MGUIButton myButton = (MGUIButton) elements[s];
				
				//we get the position of the mouse in the world...
				Vector3 mousePosition = GUICamera.ScreenToWorldPoint(Input.mousePosition);
				
				//we need to get the position of each vertice so we can test if the mouse is in the rectangle...
				MeshFilter myFilter = myButton._bgContainer.GetComponent<MeshFilter>();
				
				Rect buttonBounds = new Rect(	
												myButton._bgContainer.transform.position.x-myFilter.mesh.vertices[0].x, 
												myButton._bgContainer.transform.position.y-myFilter.mesh.vertices[0].z, 
												myFilter.mesh.vertices[0].x-myFilter.mesh.vertices[3].x, 
												myFilter.mesh.vertices[0].z-myFilter.mesh.vertices[3].z
											);
				
				//Debug.Log("Button position: "+buttonBounds);
				//Debug.Log("Mouse position: "+mousePosition);
				
				if(buttonBounds.Contains(mousePosition))
					myButton.triggerMouseDown(mouseButton);
			}
			
			if(elements[s].Type==MGUIType.textField && elements[s].Visible)
			{
				MGUITextfield myButton = (MGUITextfield) elements[s];
				
				//we get the position of the mouse in the world...
				Vector3 mousePosition = GUICamera.ScreenToWorldPoint(Input.mousePosition);
				
				//we need to get the position of each vertice so we can test if the mouse is in the rectangle...
				MeshFilter myFilter = myButton._bgContainer.GetComponent<MeshFilter>();
				
				Rect buttonBounds = new Rect(	
												myButton._bgContainer.transform.position.x-myFilter.mesh.vertices[0].x, 
												myButton._bgContainer.transform.position.y-myFilter.mesh.vertices[0].z, 
												myFilter.mesh.vertices[0].x-myFilter.mesh.vertices[3].x, 
												myFilter.mesh.vertices[0].z-myFilter.mesh.vertices[3].z
											);
				
				//Debug.Log("Button position: "+buttonBounds);
				//Debug.Log("Mouse position: "+mousePosition);
				
				if(buttonBounds.Contains(mousePosition))
				{
					//Debug.Log("Focus is now in: "+s);
					focus = s;
					myButton.triggerMouseDown(mouseButton);
				}
				else
				{
					if(focus.Equals(s))
					{
						focus = string.Empty;
						myButton.triggerNormalImage();
					}
				}
			}
		}
	}
	
	void checkIfIAmReleasingAnyButton(int mouseButton)
	{
		foreach(string s in elements.Keys)
		{
			if(!focus.Equals(s))
			{
				if(elements[s].Type==MGUIType.button && elements[s].Visible)
				{
					MGUIButton myButton = (MGUIButton) elements[s];
					myButton.triggerNormalImage();
				}
				
				if(elements[s].Type==MGUIType.textField && elements[s].Visible)
				{
					MGUITextfield myButton = (MGUITextfield) elements[s];
					myButton.triggerNormalImage();
				}
			}
		}
	}
	
	public MGUIElement setText(string id, Rect boundaries, string text, Font font, Color color)
	{
		GameObject container = new GameObject();
		container.transform.parent = GUICamera.transform;
		container.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 1);
		container.name = id;
		//container.layer = guiLayers.value;
		
		TextMesh textMeshComponent = container.AddComponent<TextMesh>();
		textMeshComponent.text = text;
		textMeshComponent.font = font;
		
		container.AddComponent<MeshRenderer>();
		textMeshComponent.renderer.material = font.material;
		textMeshComponent.renderer.material.color = color;
		
		elements.Add(id, new MGUIText(id, boundaries, container, container, text));
		return elements[id];
	}
	
	//used for textArea
	MGUIText setUntrackedText(Rect boundaries, string text, Font font, Color color)
	{
		GameObject container = new GameObject();
		container.transform.parent = GUICamera.transform;
		container.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 1);
		container.name = "textElement";
		//container.layer = guiLayers.value;
		
		TextMesh textMeshComponent = container.AddComponent<TextMesh>();
		textMeshComponent.text = text;
		textMeshComponent.font = font;
		
		container.AddComponent<MeshRenderer>();
		textMeshComponent.renderer.material = font.material;
		textMeshComponent.renderer.material.color = color;
		
		return new MGUIText("", boundaries, container, container, text);
	}
	
	//Used to inert text to a TextArea, it is highly recommended to use this function to do this.
	public MGUIElement setTextArea(string id, Rect boundaries, int height, float width, string text, Font font, Color color)
	{
		GameObject container = new GameObject();
		container.transform.parent = GUICamera.transform;
		container.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 1);
		container.name = id;
		
		elements.Add(id, new MGUITextArea(id, boundaries, container, height, width, null));
		
		if(text.Length>0)
		{
			insertText(id, text, font, color);
		}
		
		return elements[id];
	}
	
	public void insertText(string textAreaId, string text, Font font, Color color)
	{
		if(text.Length==0 || text.Equals(" "))
			return;
		
		MGUITextArea textArea = (MGUITextArea)elements[textAreaId];
		
		//check if i have reached the maximum amount (will have to lack precision...) and divide it if it is too long
		
		float currentWidth = 0;
		string currentText = string.Empty;
		string lastWord = string.Empty;
		
		for(int i=0; i<text.Length; i++)
		{
			if(currentWidth<textArea.Width)
			{
				currentWidth += 1;
				currentText += text[i].ToString();
				
				if(!text[i].Equals(' '))
				{
					if(lastWord.Length>0)
					{
						if(!lastWord[lastWord.Length-1].Equals(' '))
							lastWord += text[i].ToString();
					}
					else
						lastWord = string.Empty;
				}
			}
			else
			{
				if(text[i].Equals(' ')) //jump line only if there is a space to avoid cutting words
				{
					//we generate a simple text and jump to the next line, the textArea will take care of the text position, no need to specify bounds.
					textArea.Text.Add(setUntrackedText(new Rect(0, 0, 0, 0), currentText, font, color));
					
					if(lastWord.Length>0)
					{
						if(lastWord[lastWord.Length-1].Equals(' '))
							currentText = lastWord.Replace(" ", string.Empty);
					}
					else
						currentText = string.Empty;
					
					lastWord = string.Empty;
					
					currentWidth = 0;
				}
				else
					currentText += text[i].ToString();
			}
		}
		
		if(currentText.Length>0)
		{
			textArea.Text.Add(setUntrackedText(new Rect(0, 0, 0, 0), currentText, font, color));
		}
		
		textArea.refreshText();
	}
	
	public MGUIElement setImage(string id, Rect boundaries, Vector2 offset, Texture2D image)
	{
		GameObject container = generateQuad(new Vector2(boundaries.width, boundaries.height));
		container.transform.parent = GUICamera.transform;
		container.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 2);
		container.transform.Rotate(-90, 0, 0);
		//container.layer = guiLayers.value;
		
		container.AddComponent<MeshRenderer>();
		container.renderer.material = new Material (Shader.Find("Transparent/VertexLit"));
		container.renderer.material.SetTexture("_MainTex", image);
		//container.renderer.material.mainTextureOffset = offset;
		
		elements.Add(id, new MGUIImage(id, boundaries, container, image));
		return elements[id];
	}
	
	public MGUIElement setButton(string id, Rect boundaries, Vector2 offset, string text, Font font, Color color, Texture2D normal, Texture2D down, Texture2D over)
	{
		GameObject imageContainer = generateQuad(new Vector2(boundaries.width, boundaries.height));
		imageContainer.transform.parent = GUICamera.transform;
		imageContainer.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 2);
		imageContainer.transform.Rotate(-90, 0, 0);
		//imageContainer.layer = guiLayers.value;
		
		imageContainer.AddComponent<MeshRenderer>();
		imageContainer.renderer.material = new Material (Shader.Find("Transparent/VertexLit"));
		imageContainer.renderer.material.SetTexture("_MainTex", normal);
		imageContainer.renderer.material.mainTextureOffset = offset;
		
		GameObject textContainer = new GameObject();
		textContainer.transform.parent = GUICamera.transform;
		
		//to get the centered x position, we get the first vertice's position of the bg image mesh
		MeshFilter imageMeshRenderer = imageContainer.GetComponent<MeshFilter>();
		float centerX = imageMeshRenderer.mesh.vertices[0].x/2;
		
		textContainer.transform.localPosition = new Vector3(boundaries.x-centerX, boundaries.y+1, 1);
		//textContainer.layer = guiLayers.value;
		
		TextMesh textMeshComponent = textContainer.AddComponent<TextMesh>();
		textMeshComponent.text = text;
		textMeshComponent.font = font;
		
		textContainer.AddComponent<MeshRenderer>();
		textMeshComponent.renderer.material = font.material;
		textMeshComponent.renderer.material.color = color;
		
		GameObject container = new GameObject();
		container.transform.parent = GUICamera.transform;
		container.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 0);
		textContainer.transform.parent = container.transform;
		imageContainer.transform.parent = container.transform;
		container.name = id;
		
		elements.Add(id, new MGUIButton(id, boundaries, container, imageContainer, textContainer, text, normal, over, down));
		return elements[id];
	}
	
	public MGUIElement setTextField(string id, Rect boundaries, Vector2 offset, string text, Font font, Color color, Texture2D normal, Texture2D down, Texture2D over)
	{
		GameObject imageContainer = generateQuad(new Vector2(boundaries.width, boundaries.height));
		imageContainer.transform.parent = GUICamera.transform;
		imageContainer.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 2);
		imageContainer.transform.Rotate(-90, 0, 0);
		//imageContainer.layer = guiLayers.value;
		
		imageContainer.AddComponent<MeshRenderer>();
		imageContainer.renderer.material = new Material (Shader.Find("Transparent/VertexLit"));
		imageContainer.renderer.material.SetTexture("_MainTex", normal);
		imageContainer.renderer.material.mainTextureOffset = offset;
		
		//to get the min x position, we get the first vertice's position of the bg image mesh
		MeshFilter imageMeshRenderer = imageContainer.GetComponent<MeshFilter>();
		float minX = imageMeshRenderer.mesh.vertices[0].x;
		
		GameObject textContainer = new GameObject();
		textContainer.transform.parent = GUICamera.transform;
		textContainer.transform.localPosition = new Vector3(boundaries.x-minX+1, boundaries.y+1, 1);
		//textContainer.layer = guiLayers.value;
		
		TextMesh textMeshComponent = textContainer.AddComponent<TextMesh>();
		textMeshComponent.text = text;
		textMeshComponent.font = font;
		
		textContainer.AddComponent<MeshRenderer>();
		textMeshComponent.renderer.material = font.material;
		textMeshComponent.renderer.material.color = color;
		
		GameObject container = new GameObject();
		container.transform.parent = GUICamera.transform;
		container.transform.localPosition = new Vector3(boundaries.x, boundaries.y, 0);
		textContainer.transform.parent = container.transform;
		imageContainer.transform.parent = container.transform;
		container.name = id;
		
		elements.Add(id, new MGUITextfield(id, boundaries, container, imageContainer, textContainer, text, normal, over, down));
		return elements[id];
	}
	
	public MGUIElement getElement(string id)
	{
		return elements[id];
	}
	
	public void removeElement(string id)
	{
		Destroy(elements[id].Container);
		elements.Remove(id);
	}
	
	GameObject generateQuad(Vector2 size)
	{
		GameObject container = new GameObject();
		
		MeshFilter myFilter = container.AddComponent<MeshFilter>();
		
		Vector3[] vertices = new Vector3[]
		{
			new Vector3( size.x, 0, size.y),
			new Vector3( size.x, 0, -size.y),
			new Vector3(-size.x, 0, size.y),
			new Vector3(-size.x, 0, -size.y),
		};
		
		Vector2[] uv = new Vector2[]
		{
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(0, 0),
		};
		
		int[] triangles = new int[]
		{
			0, 1, 2, 2, 1, 3,
		};
		
		myFilter.mesh.vertices = vertices;
		myFilter.mesh.triangles = triangles;
		myFilter.mesh.uv = uv;
		
		myFilter.mesh.RecalculateBounds();
		myFilter.mesh.RecalculateNormals();
		
		return container;
	}
}

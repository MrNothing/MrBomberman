/*
 * The fog is rendered using vertex paint
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogTileHandler : MonoBehaviour 
{
	
	Mesh mesh;
	public Vector3[] vertices;
	
	//we use color32 as it is more performant
	public Color32[] colors;
	
	//used to make fog to its default alpha when it is not cleareds
	byte defaultAlpha = 0;
	
	// Use this for initialization
	void Start () 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		
		vertices = mesh.vertices;
		colors = new Color32[mesh.vertices.Length];
		
		for(int i=0; i<mesh.vertices.Length; i++)
			colors[i] = new Color32(0, 0, 0, defaultAlpha);
		
		mesh.colors32 = colors;
	}
	
	//Update is disabled if there is no transparent vertex
	List<int> modifiedVertexes = new List<int>();
	void Update()
	{
		List<int> modifiedVertexesClone = new List<int>(modifiedVertexes);
		foreach(int i in modifiedVertexesClone)
		{
			if(colors[i].a<defaultAlpha)
			{
				colors[i] = normalizeAlpha(colors[i], defaultAlpha, 1);
			}
			else
			{
				modifiedVertexes.Remove(i);
			}
		}
		
		if(modifiedVertexes.Count==0)
		{
			if(vertices.Length>0)
				enabled = false;
		}
		else
			mesh.colors32 = colors;
	}
	
	Color32 normalizeAlpha(Color32 color, byte normal, byte speed)
	{
		if(color.a>normal)
			color.a-=speed;
		
		if(color.a<normal)
			color.a+=speed;
		
		return color;
	}
	
	public void setDefaultFog(Color32 color)
	{
		for(int i=0; i<mesh.vertices.Length; i++)
			colors[i] = color;
		
		mesh.colors32 = colors;
		defaultAlpha = color.a;
	}
	
	public void clearFog(Vector3 point, float brushSize)
	{
		for(int i=0; i<vertices.Length; i++)
		{
			Vector3 vert = vertices[i];
			Vector3 vertWorldPosition = transform.TransformPoint(new Vector3(vert.x, point.y, vert.z));
			float distance = Vector3.Distance(vertWorldPosition, point);
			if(distance<brushSize) //if i am in brush range
			{
				colors[i] = new Color32(0, 0, 0, 0);
				
				try
				{
					modifiedVertexes.Remove(i);
				}
				catch
				{
					
				}
				modifiedVertexes.Add(i);
				enabled = true;
			}
		}
		
		mesh.colors32 = colors;
	}
	
	//the value of the collider is its radius while the key is the vertice.
	public void clearFog(Vector3 point, float brushSize, Dictionary<int, float> colliders, float smoothRate)
	{
		for(int i=0; i<vertices.Length; i++)
		{
			try
			{
				if(colliders[i]>0)
				{
					//if there is a collider, do not clear the fog depending on the height of that collider
					Vector3 vert = vertices[i];
					Vector3 vertWorldPosition = transform.TransformPoint(new Vector3(vert.x, vert.y, vert.z));
					float distanceY = Mathf.Abs(vertWorldPosition.y-point.y);
					float distance = Vector3.Distance(vertWorldPosition, point);
					
					if(distanceY<smoothRate && distance<brushSize)
					{
						float alpha = 1-(smoothRate-distanceY)/smoothRate;
						
						alpha*=255;
						
						if(alpha<0)
							alpha = 0;
						
						colors[i] = new Color32(0, 0, 0, (byte)alpha);	
						
						try
						{
							modifiedVertexes.Remove(i);
						}
						catch
						{
							
						}
						modifiedVertexes.Add(i);
						enabled = true;
					}
				}
			}
			catch
			{
				Vector3 vert = vertices[i];
				Vector3 vertWorldPosition = transform.TransformPoint(new Vector3(vert.x, point.y, vert.z));
				float distance = Vector3.Distance(vertWorldPosition, point);
				if(distance<brushSize) //if i am in brush range
				{
					colors[i] = new Color32(0, 0, 0, 0);
					
					try
					{
						modifiedVertexes.Remove(i);
					}
					catch
					{
						
					}
					modifiedVertexes.Add(i);
					enabled = true;
				}
			}
		}
		
		mesh.colors32 = colors;
	}
	
	public Dictionary<int, float> findColliders(Vector3 point, float brushSize, float maxStep)
	{
		vertices = mesh.vertices;
		Dictionary<int, float> colliders = new Dictionary<int, float>();
		for(int i=0; i<vertices.Length; i++)
		{
			Vector3 vert = vertices[i];
			Vector3 vertWorldPosition = transform.TransformPoint(new Vector3(vert.x, vert.y, vert.z));
			//float distance = Vector3.Distance(vertWorldPosition, point);
			float distanceY = Mathf.Abs(vertWorldPosition.y-point.y);
			
			if(distanceY>maxStep)
			{
				colliders.Add(i, 0.5f);
			}
			
		}
		return colliders;
	}
}

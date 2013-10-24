using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VerticlesIndexer : MonoBehaviour 
{
	public string Id;
	
	Dictionary<string, int> _index; //since we handle planes, only one vertice is needed.
	public Vector3[] _verts;
	Mesh mesh=null;
	
	public Dictionary<string, int> Index 
	{
		get 
		{
			return this._index;
		}
		set 
		{
			_index = value;
		}
	}
	
	void Start()
	{
		if(mesh==null)
			refreshIndex();
	}

	//refresh the index dictionnary... 
	public void refreshIndex() 
	{
		_index = new Dictionary<string, int>();
		
		mesh = GetComponent<MeshFilter>().mesh;
		
		_verts = mesh.vertices;
		
		/*for(int i=0; i<mesh.vertices.Length; i++)
		{
			Vector3 vert = mesh.vertices[i];
			
			try
			{
				_index.Add(vert.ToString(), i);
			}
			catch
			{
				//this should not happen unless a different shape from the default one has been selected, if it is the case, proceed anyway.
			}
		}*/
	}
	
	public List<int> getNearestVertices(Vector3 point, float range)
	{
		List<int> chosenVertices = new List<int>();
		for(int i=0; i<_verts.Length; i++)
		{
			if(Vector3.Distance(transform.TransformPoint(_verts[i]), point)<range)
				chosenVertices.Add(i);
		}
		return chosenVertices;
	}
	
	public int getNearestVertice(Vector3 point)
	{
		float lastRange = float.MaxValue;
		int chosenVertice=-1;
		for(int i=0; i<_verts.Length; i++)
		{
			float distance = Vector3.Distance(transform.TransformPoint(_verts[i]), point);
			if(distance<lastRange)
			{
				lastRange = distance;
				chosenVertice = i;
			}
		}
		return chosenVertice;
	}
	
	//same principle as getNearestVertice but we convert Vector3 coords to Vector2.
	public int getNearestPlanarVertice(Vector3 worldPoint)
	{
		Vector2 point = new Vector2(worldPoint.x, worldPoint.z);
		float lastRange = float.MaxValue;
		int chosenVertice=-1;
		for(int i=0; i<_verts.Length; i++)
		{
			Vector2 planarWorldVerticePoint = new Vector2(transform.TransformPoint(_verts[i]).x, transform.TransformPoint(_verts[i]).z);
			float distance = Vector2.Distance(planarWorldVerticePoint, point);
			if(distance<lastRange)
			{
				lastRange = distance;
				chosenVertice = i;
			}
		}
		return chosenVertice;
	}
	
	public void applySmoothTranslationToVerticles(Vector3 point, Vector3 translation, float brushSize, float intensity, float MAX_HEIGHT)
	{
		for(int i=0; i<_verts.Length; i++)
		{
			Vector3 vert = _verts[i];
			Vector3 vertWorldPosition = transform.TransformPoint(new Vector3(vert.x, point.y, vert.z));
			float distance = Vector3.Distance(vertWorldPosition, point);
			float relativeIntensity = (1-distance/brushSize)*intensity;
			
			if(relativeIntensity>intensity)
				relativeIntensity = intensity;
			
			if(relativeIntensity>0) //if i am in brush range
			{
				_verts[i]+=translation*relativeIntensity;
				
				if(_verts[i].y>MAX_HEIGHT)
					_verts[i].y = MAX_HEIGHT;
				
				if(_verts[i].y<-MAX_HEIGHT)
					_verts[i].y = -MAX_HEIGHT;
			}
		}
		
		updateVertices();
	}
	
	public void updateVertices()
	{
		mesh.vertices = _verts;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class IOManager
{
	/*
	 * To save the map, we need to save two things, the serializable elements and the Map's tiles textures
	 * */
	public void saveMap(string path, Hashtable _world)
	{
		//we create the new folders if they do not exit.
		createPathIfItDoesNotExist(path);
		
		Hashtable world = new Hashtable(_world);
		//before writing the hashTable, we save the textures and remove the non serializable elements from the hashmap.
		foreach(string s in world.Keys)
		{
			Hashtable elementInfos = (Hashtable) world[s];
			
			if(elementInfos["doodad"]==null) //if this is not a doodad...
			{
				GameObject textureTile = (GameObject) elementInfos["textureTile"];
				GameObject colorTile = (GameObject) elementInfos["colorTile"];
				
				//save the textures...
				saveTexture(path+"/tex_"+s, (Texture2D)textureTile.renderer.material.mainTexture);
				
				//we wont save the color layer for now, if we find a way to save images to jpg format we could save them again.
				//saveTexture(path+"/colo_"+s, (Texture2D)textureTile.renderer.material.mainTexture);
				
				Hashtable serializableVertices = new Hashtable();
				//save the Vertices
				VerticlesIndexer verticeIndexer = (VerticlesIndexer) elementInfos["verticlesIndexer"];
				
				for(int i=0; i<verticeIndexer._verts.Length; i++)
				{
					Hashtable tmpVert = new Hashtable();
					tmpVert.Add("x", verticeIndexer._verts[i].x);
					tmpVert.Add("y", verticeIndexer._verts[i].y);
					tmpVert.Add("z", verticeIndexer._verts[i].z);
					serializableVertices.Add(i, tmpVert);
				}
				
				elementInfos.Add("verts", serializableVertices);
				
				elementInfos.Remove("textureTile");
				elementInfos.Remove("colorTile");
				elementInfos.Remove("fogTile");
				elementInfos.Remove("verticlesIndexer");
			}
			else
			{
				elementInfos.Remove("doodad");
			}
		}
		
		writeHashtable(path+"mainData", world);
	}
	
	public Hashtable loadMapInfos(string path)
	{
		return readHashtable(path);
	}
	
    public string[] getAllFileNamesInFolder(string folder)
    {
        return Directory.GetFiles(folder);
    }
	
	public void saveTexture(string path, Texture2D tex)
	{
		var bytes = tex.EncodeToPNG();
		FileStream file = new FileStream(path, FileMode.OpenOrCreate);
		file.Write(bytes, 0, bytes.Length);
		file.Close();
	}
	
	public Texture2D loadTexture(string path)
	{
		return null;
	}
	
    public void writeHashtable(string path, Hashtable myTable)
    {
        //To write Hashtable on file:
       	
        BinaryFormatter bfw = new BinaryFormatter();
        FileStream file = new FileStream(path, FileMode.OpenOrCreate);
        StreamWriter ws = new StreamWriter(file);
        bfw.Serialize(ws.BaseStream, myTable);
    }

    public Hashtable readHashtable(string path)
    {
        //To read Hashtable from file:

        FileStream file = new FileStream(path, FileMode.Open);
        StreamReader readMap = new StreamReader(file);
        BinaryFormatter bf = new BinaryFormatter();
        return (Hashtable)bf.Deserialize(readMap.BaseStream);
    }

    public void writeTextFile(string text, string path)
    { 
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(text);

        using (StreamWriter outfile = new StreamWriter(path, true))
        {
            outfile.Write(sb.ToString());
        }
    }
	
	void createPathIfItDoesNotExist(string path)
	{
		if(!System.IO.Directory.Exists(path))
		    System.IO.Directory.CreateDirectory(path);
	}
}


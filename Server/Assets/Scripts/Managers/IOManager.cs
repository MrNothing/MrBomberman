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


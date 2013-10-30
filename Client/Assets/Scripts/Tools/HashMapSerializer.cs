/*
 * Author: Boris Musarais. This is the Eternity serialization protocol.
 * */

using UnityEngine;
using System.Collections;
using System;

public class HashMapSerializer {
	
	//converts a string to a HashMap if the string was serialized with the Eternity protocol.
	public static Hashtable dataToHashMap(String data)
    {
        int solveDoublesParams = 0; //in case there is many params with the same name.

        Hashtable params1 = new Hashtable();
        while (data.IndexOf("{") != -1 && data.IndexOf("=") != -1 && data.IndexOf("}") != -1)
        {
            try
            {
                String param_Name = data.Substring(data.IndexOf("{") + 1, data.IndexOf("=") - (data.IndexOf("{") + 1));
                String param_Data = data.Substring(data.IndexOf("=") + 1, data.IndexOf("}") - (data.IndexOf("=") + 1));
				
                if (param_Name.Equals("sub"))
                {
                    Hashtable subHashMap;

                    String firstString = "{sub=" + param_Data + "}";
                    String lastString = "{endsub=" + param_Data + "}";

                    String sub_raw_data = data.Substring(data.IndexOf(firstString) + firstString.Length, data.IndexOf(lastString) - (data.IndexOf(firstString) + firstString.Length));
                    data = ReplaceFirst(data, "{sub=" + param_Data + "}" + sub_raw_data + "{endsub=" + param_Data + "}", "");
                    subHashMap = (Hashtable)dataToHashMap(sub_raw_data);

                    params1.Add(param_Data, subHashMap);
                }
                else
                {
                    data = ReplaceFirst(data, "{" + param_Name + "=" + param_Data + "}", "");

                    String dataType = "";

                    if (param_Name.IndexOf("~") != -1)
                    {
                        dataType = param_Name.Substring(param_Name.IndexOf("~") + 1, param_Name.Length - (param_Name.IndexOf("~") + 1));
                        param_Name = ReplaceFirst(param_Name, "~" + dataType, "");
                    }

                    param_Name = param_Name.Replace("*ti*", "~");
                    param_Data = param_Data.Replace("~eq~", "=");
                    param_Data = param_Data.Replace("~a+~", "{");
                    param_Data = param_Data.Replace("~a-~", "}");

                    System.Object finalData = param_Data;
                    if (dataType.Equals("i"))
                    {
                        finalData = int.Parse(param_Data);
                    }

                    if (dataType.Equals("n"))
                    {
                        finalData = float.Parse(param_Data);
                    }

                    if (dataType.Equals("b"))
                    {
                        finalData = bool.Parse(param_Data);
                    }

                    if (params1[param_Name] == null)
                    {
                        //...
                    }
                    else
                    {
                        solveDoublesParams++;
                        param_Name = param_Name + solveDoublesParams;
                    }

                    params1.Add(param_Name, finalData);
                }
            }
            catch
            {
                //System.out.println("Error while parsing data: "+e);
            }
        }

        return params1;
    }
    /**
     *  Converts the HashMap into data.
     *
     *  HashMap Structure: <String param1,String value>,<String param2,String value>
     *
     *  data Structure: "{param1=value}{param2=value}..."
     */
    public static String hashMapToData(Hashtable params1)
    {

        String data = "";

        foreach (System.Object o in params1.Keys)
        {
            String param = o + "";
            if (params1[param] == null)
            {
                data += "{" + param + "=null}";
            }
            else
            {
                if (params1[param].GetType().Equals(typeof(Hashtable)))
                {
                    data += "{sub=" + param + "}";
                    data += hashMapToData((Hashtable)params1[param]);
                    data += "{endsub=" + param + "}";
                }
                else
                {
                    param = ReplaceFirst(param, "~", "*ti*");

                    //bool known = false;
                    System.Object paramValue = params1[param];
                    if (params1[param].GetType().Equals(typeof(byte)) || params1[param].GetType().Equals(typeof(short)) || params1[param].GetType().Equals(typeof(int)) || params1[param].GetType().Equals(typeof(long)))
                    {
                        //known = true;
                        param = param + "~i";
                    }
                    else
                    {
                        if (params1[param].GetType().Equals(typeof(float)) || params1[param].GetType().Equals(typeof(double)))
                        {
                            //known = true;
                            param = param + "~n";
                        }
                        else
                        {
                            if (params1[param].GetType().Equals(typeof(bool)))
                            {
                                //known = true;
                                param = param + "~b";
                            }
                        }
                    }

                    String controlledValue = paramValue + "";
                    controlledValue = controlledValue.Replace("=", "~eq~");
                    controlledValue = controlledValue.Replace("{", "~a+~");
                    controlledValue = controlledValue.Replace("}", "~a-~");

                    data += "{" + param + "=" + controlledValue + "}";
                }
            }
        }

        //if(Eternity.debug)
        //System.out.println("[SENDING] "+data);

        return data;
    }
	
	public static string ReplaceFirst(string text, string search, string replace)
	{
		int pos = text.IndexOf(search);
		if (pos < 0)
		{
			return text;
		}
		return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
	}
}

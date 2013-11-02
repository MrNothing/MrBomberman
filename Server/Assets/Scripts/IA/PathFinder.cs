/*
* Author: Musarais Boris B4 A*Pathfinder, DO NOT USE THIS WITHOUT PERMISSION
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4
{
    //To use this pathfinder, call start()
	public class PathFinder
	{
        float baseStep = 1;
		
		//open tiles are sorted by distance the closest one will always be picked first
		SortedDictionary<float, Vector3> openTiles;
		
        Hashtable closedTiles;
		
		//walkable tiles...
        Dictionary<String, float> wayPoints;
        
		//path target
        Vector3 target;
		
		//starting point
		Vector3 startPoint;
		
		public delegate void PathFound(List<Vector3> _paths);
		public PathFound OnPathFound;
		
		delegate void StartPathFinder();
		StartPathFinder OnPathFinderStart;
		
		System.Threading.Thread currentThread;
		
        public PathFinder(Dictionary<String, float> _wayPoints, float _baseStep, float _maxCliffHeight)
        {
            openTiles = new SortedDictionary<float, Vector3>();
            closedTiles = new Hashtable();
            wayPoints = _wayPoints;
            baseStep = _baseStep;
			maxCliffHeight = _maxCliffHeight;
			
			OnPathFinderStart = new StartPathFinder(start);
        }
		
		//delegate stuff
		void raisePathFound(List<Vector3> _paths)
		{
			OnPathFound(_paths);
		}
		
		public void initializeSearch(Vector3 _start, Vector3 _target, float _maxCliffHeight)
		{
			maxCliffHeight = _maxCliffHeight;
			target = _target;
            startPoint = _start;
			
			OnPathFinderStart.BeginInvoke(null, null);
		}
		
		public List<Vector3> start(Vector3 _start, Vector3 _target, float _maxCliffHeight)
		{
			maxCliffHeight = _maxCliffHeight;
			return start(_start, _target);
		}
		
        public List<Vector3> start(Vector3 _start, Vector3 _target)
        {
            if (wayPoints.Count > 0)
            {
				startPoint = _start;
                target = _target;
                openTiles = new SortedDictionary<float, Vector3>();
                closedTiles = new Hashtable();

                result = new List<Vector3>();

                bestPath = null;

                search(_start);

                return result;
            }
            else
            {
                List<Vector3> tmpRes = new List<Vector3>();
                tmpRes.Add(_target);
                return tmpRes;
            }
        }
		
		public void start()
        {
            if (wayPoints.Count > 0)
            {
                openTiles = new SortedDictionary<float, Vector3>();
                closedTiles = new Hashtable();

                result = new List<Vector3>();

                bestPath = null;

                search(startPoint);
            }
            else
            {
                List<Vector3> tmpRes = new List<Vector3>();
                tmpRes.Add(target);
            }
        }
		
		//I recommend not to tweak these values, at best this will change nothing, at worst you will lose performance or precision
        int checkRange = 8; 
        float checkAngleStep = Mathf.PI/4;
		
		//in case the path was not found, we use this one.
        private Vector3 bestPath=null;

        private void search(Vector3 lastPoint)
        { 
            //Check around lastPoint...
            
			float angle = Mathf.Atan2(target.x-lastPoint.x, target.z-lastPoint.z);
			
			for(int i=0; i<checkRange; i++)
			{
				float x = Mathf.Sin(angle)*baseStep;
				float y = Mathf.Cos(angle)*baseStep;
			
				Vector3 newPoint = lastPoint.Add(new Vector3(x, 0, y));
                String rangeId = newPoint.toPosRefId(baseStep);
				
				//if there is a walkable tile and I have not already visited this tile
                if (isNotTooHighIfExists(wayPoints, rangeId, lastPoint) && closedTiles[rangeId]==null)
                {
                    closedTiles.Add(rangeId, true);
					
					//if i am closer than the last point to the target
                    if ((lastPoint.Substract(target)).Magnitude() > newPoint.Substract(target).Magnitude())
                    {
						try
						{
                    		openTiles.Add(newPoint.Substract(target).Magnitude(), newPoint);
						}
						catch
						{
							
						}
						
						newPoint.parent = lastPoint;
					
                	}
            	}
				
				angle+=checkAngleStep;
            }
			
			 //if i have ways left...
            if (openTiles.Count > 0)
            {
                float minKey = openTiles.Keys.First();
                
				if(bestPath!=null)
				{
					//if the new chosen path is closer than the last best path, we choose it as the new best path
					if(bestPath.Substract(target).Magnitude()>openTiles[minKey].Substract(target).Magnitude())
						bestPath = openTiles[minKey];
				}
				else
				{
					bestPath = openTiles[minKey];
				}
				
                openTiles.Remove(minKey);
				
				search(bestPath);
            }
            else 
            {
                if (bestPath != null)
                {
                    if (Math.Floor(bestPath.Substract(target).Magnitude()/baseStep) == 0)
                    {
                        //Path found!
                        end();
						
						//Debug.Log("Path found!");
                    }
                    else
                    { 
                        //The destination path was not found, but a closer path was found.
                        end();
						
						//Debug.Log("Approximative Path found!");
                    }
                }
                else
                { 
                    //no path was found!
					//Debug.Log("Path not found!");
                }
            }
        }

        private List<Vector3> result;
        private void end()
        {
            Vector3 lastPath = bestPath;
            do
            {
                result.Add(lastPath);
                lastPath = lastPath.parent;
            }
            while (lastPath.parent != null);
			
			raisePathFound(result);
        }
		
		private bool hasIndexAt(Dictionary<string, float> dictionary, string key)
		{
			try
			{
				if(dictionary[key]!=null)
					return true;
			}
			catch
			{
				return false;
			}
		}
		
		private float getIndexHeight(Dictionary<string, float> dictionary, string key)
		{
			try
			{
				return dictionary[key];
			}
			catch
			{
				return -58380;
			}
		}
		
		float maxCliffHeight = 0.5f;
		
		//this is used to determine if the path is walkable, if the cliff is too high it is not walkable.
		private bool isNotTooHighIfExists(Dictionary<string, float> dictionary, string key, Vector3 currentPosition)
		{
			try
			{
				if(dictionary[key]!=null)
				{
					if(Mathf.Abs(dictionary[key]-currentPosition.y)<maxCliffHeight)
						return true;
					else
						return false;
				}
			}
			catch
			{
				return false;
			}
		}
		
		public string getInfos()
		{
			return "ways: "+wayPoints.Count;
		}
	}
}
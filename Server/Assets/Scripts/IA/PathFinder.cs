/*
* Author: Musarais Boris B4 A*Pathfinder, DO NOT USE THIS WITHOUT PERMISSION
*/

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
		
        Dictionary<String, bool> closedTiles;
		
		//walkable tiles...
        Dictionary<String, Vector3> wayPoints;
        
		//path target
        Vector3 target;

        public PathFinder(Dictionary<String, Vector3> _wayPoints, float _baseStep)
        {
            openTiles = new SortedDictionary<float, Vector3>();
            closedTiles = new Dictionary<String, bool>();
            wayPoints = _wayPoints;
            baseStep = _baseStep;
        }

        public List<Vector3> start(Vector3 start, Vector3 _target)
        {
            if (wayPoints.Count > 0)
            {
                target = _target;
                openTiles = new SortedDictionary<float, Vector3>();
                closedTiles = new Dictionary<String, bool>();

                result = new List<Vector3>();

                bestPath = null;

                search(start);

                return result;
            }
            else
            {
                List<Vector3> tmpRes = new List<Vector3>();
                tmpRes.Add(_target);
                return tmpRes;
            }
        }
		
		//not recommented to increase this, may have some weird (funny?) results...
        int checkRange = 1; 
        /*    
               Checkrange example for 1:
               
               XXX
               X X
               XXX
        */
		
		//in case the path was not found, we use this one.
        private Vector3 bestPath=null;

        private void search(Vector3 lastPoint)
        { 
            //Check around lastPoint...
            for (int i = -checkRange; i < checkRange; i++)
            {
                for (int j = -checkRange; j < checkRange; j++)
                {
                    Vector3 newPoint = lastPoint.Add(new Vector3(i * baseStep, 0, j * baseStep));
                    String rangeId = newPoint.toPosRefId(baseStep);
					
					//if there is a walkable tile and I have not already visited this tile
                    if ((hasIndexAt(wayPoints, rangeId) || wayPoints.Count==0) && !hasIndexAt(closedTiles, rangeId))
                    {
                        closedTiles.Add(rangeId, true);

                        //if i am closer than the last point to the target
                        if ((lastPoint.Substract(target)).Magnitude() > newPoint.Substract(target).Magnitude())
                        {
                            newPoint.parent = lastPoint;
                            openTiles.Add(newPoint.Substract(target).Magnitude(), newPoint);
                        }
                    }
                }
            }

            //if i have ways left...
            if (openTiles.Count > 0)
            {
                float minKey = openTiles.Keys.First();
                openTiles.Remove(minKey);

                bestPath = openTiles[minKey];

                search(bestPath);
            }
            else 
            {
                if (bestPath != null)
                {
                    if (bestPath.Substract(target).Magnitude() == 0)
                    {
                        //Path found!
                        end();
                    }
                    else
                    { 
                        //The destination path was not found, but a closer path was found.
                        end();
                    }
                }
                else
                { 
                    //no path was found!
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
        }
		
		private bool hasIndexAt(Dictionary<string, Vector3> dictionary, string key)
		{
			try
			{
				if(dictionary[key]!=null)
					return true;
				else
					return false;
			}
			catch
			{
				return false;
			}
		}
		
		private bool hasIndexAt(Dictionary<string, bool> dictionary, string key)
		{
			try
			{
				//this is always true
				return dictionary[key]!=null;
			}
			catch
			{
				return false;
			}
		}
	}
}
/*
 * Author: Musarais Boris
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace B4
{
	public class Vector3
	{
		public float x=0;
		public float y=0;
		public float z=0;
		
		//This is usefull for the pathfinder
		public Vector3 parent=null;
		
		public Vector3(float _x, float _y, float _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}
		
		public Vector3 ()
		{
			//zero vector;
            x = 0;
            y = 0;
            z = 0;
		}
		
		public Vector3 (Vector3 vector)
		{
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}
		
		public Vector3 Add(Vector3 tmpVect)
		{
			return new Vector3(x+tmpVect.x, y+tmpVect.y, z+tmpVect.z);
		}
		
		public Vector3 Add(float value)
		{
			return new Vector3(x + value, y + value, z + value);
		}

		public Vector3 Substract(Vector3 tmpVect)
		{
			return new Vector3(x - tmpVect.x, y - tmpVect.y, z - tmpVect.z);
		}
		
		public Vector3 Substract(float value)
		{
			return new Vector3(x - value, y - value, z - value);
		}
		
		public Vector3 Multiply(Vector3 tmpVect)
		{
			return new Vector3(x * tmpVect.x, y * tmpVect.y, z * tmpVect.z);
		}
		
		public Vector3 Multiply(float value)
		{
			return new Vector3(x * value, y * value, z * value);
		}
		
		public Vector3 invert()
		{
			return new Vector3(1f/x, 1f/y, 1f/z);
		}
		
		public Vector3 negative()
		{
			return new Vector3(-x, -y, -z);
		}

		public float Magnitude()
		{
			return Math.Abs(x)+Math.Abs(y)+Math.Abs(z);
		}

		public float SqrMagnitude()
		{
			return (float)Math.Sqrt((double)x * x + (double)y * y + (double)z * z);
		}

		public bool isZero()
		{
			return Magnitude()==0;
		}

		public Vector3 getNewInstance()
		{
			return new Vector3(x, y, z);
		}

		public String toString()
		{
			return x + "_" + y + "_" + z;
		}
		
		public String toPosRefId(float step)
		{
			return Math.Floor(x/step)*step + "_" + Math.Floor(y/step)*step + "_" + Math.Floor(z/step)*step;
		}
		
		private float defaultStep = 1;
		public String toPosRefId()
		{
			return Math.Floor(x/defaultStep)*defaultStep + "_" + Math.Floor(y/defaultStep)*defaultStep + "_" + Math.Floor(z/defaultStep)*defaultStep;
		}

        public Vector3 smash(float factor)
        {
            return new Vector3((float)Math.Floor(x / factor) * factor, (float)Math.Floor(y / factor) * factor, (float)Math.Floor(z / factor) * factor);
        }

        public Vector3 smash(Vector3 factorAsVector)
        {
            return new Vector3((float)Math.Floor(x / factorAsVector.x) * factorAsVector.x, (float)Math.Floor(y / factorAsVector.y) * factorAsVector.y, (float)Math.Floor(z / factorAsVector.z) * factorAsVector.z);
        }
	}
}

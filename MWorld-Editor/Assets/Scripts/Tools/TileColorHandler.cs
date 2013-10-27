using UnityEngine;
using System.Collections;

public class TileColorHandler : MonoBehaviour {
	
	float DEFAULT_TILE_SIZE = 2; //it is alway 2...
	float DEFAULT_TEXTURE_RESOLUTION = 128;
	
	public void paint(Vector3 point, Vector4 color, float brushSize, float intensity, float maxTransparency)
	{
	    Texture2D texture = new Texture2D((int)DEFAULT_TEXTURE_RESOLUTION, (int)DEFAULT_TEXTURE_RESOLUTION);
		
		texture.SetPixels(((Texture2D)renderer.material.mainTexture).GetPixels());
		
        for(int x=0; x<texture.width; x++)
		{
			for(int y=0; y<texture.height; y++)
			{
				//Warning! x and y are inverted in the texture!!!
				float localZcoords = -(((float)x/(float)texture.width)*DEFAULT_TILE_SIZE-DEFAULT_TILE_SIZE/2); //we need to take in account the fact that the mesh is centered
				float localXcoords = -(((float)y/(float)texture.height)*DEFAULT_TILE_SIZE-DEFAULT_TILE_SIZE/2); //we need to take in account the fact that the mesh is centered
				
				Vector3 pixelWorldPosition = transform.TransformPoint(new Vector3(localXcoords, point.y, localZcoords));
				
				float distance = Vector3.Distance(pixelWorldPosition, point);
				float relativeIntensity = (1-distance/brushSize)*intensity;
				
				if(relativeIntensity>intensity)
				relativeIntensity = intensity;
			
				if(relativeIntensity>0) //if i am in brush range
				{
					Color lastColor = texture.GetPixel(x, y);
					Color newColor = new Color(lastColor.r+color.x*relativeIntensity, lastColor.g+color.y*relativeIntensity, lastColor.b+color.z*relativeIntensity, lastColor.a+color.w*relativeIntensity);
					
					if(newColor.a>maxTransparency)
						newColor.a = maxTransparency;
					
					texture.SetPixel(x, y, newColor);
				}
			}
		}
		
        texture.Apply();
		renderer.material.mainTexture = texture;
	}
	
	public void paintTexture(Vector3 point, Texture2D brush, float brushSize, float intensity, float maxTransparency)
	{
		Texture2D texture = new Texture2D((int)DEFAULT_TEXTURE_RESOLUTION, (int)DEFAULT_TEXTURE_RESOLUTION);
		
		texture.SetPixels(((Texture2D)renderer.material.mainTexture).GetPixels());
		
        for(int x=0; x<texture.width; x++)
		{
			for(int y=0; y<texture.height; y++)
			{
				//Warning! x and y are inverted in the texture!!!
				float localZcoords = -(((float)x/(float)texture.width)*DEFAULT_TILE_SIZE-DEFAULT_TILE_SIZE/2); //we need to take in account the fact that the mesh is centered
				float localXcoords = -(((float)y/(float)texture.height)*DEFAULT_TILE_SIZE-DEFAULT_TILE_SIZE/2); //we need to take in account the fact that the mesh is centered
				
				Vector3 pixelWorldPosition = transform.TransformPoint(new Vector3(localXcoords, point.y, localZcoords));
				
				float distance = Vector3.Distance(pixelWorldPosition, point);
				float relativeIntensity = (1-distance/brushSize)*intensity;
				
				if(relativeIntensity>intensity)
				relativeIntensity = intensity;
			
				if(relativeIntensity>0) //if i am in brush range
				{
					Color lastColor = texture.GetPixel(x, y);
					
					Color color = brush.GetPixel(x, y);
					
					Color newColor = new Color(lastColor.r, lastColor.g, lastColor.b, lastColor.a);
					
					//BLEND MODE: NORMAL (merge)
					if(lastColor.r<color.r)
						newColor.r += Mathf.Abs(lastColor.r-color.r)*relativeIntensity;
					
					if(lastColor.r>color.r)
						newColor.r -= Mathf.Abs(lastColor.r-color.r)*relativeIntensity;
						
					if(lastColor.g<color.g)
						newColor.g += Mathf.Abs(lastColor.g-color.g)*relativeIntensity;
					
					if(lastColor.g>color.g)
						newColor.g -= Mathf.Abs(lastColor.g-color.g)*relativeIntensity;
					
					if(lastColor.b<color.b)
						newColor.b += Mathf.Abs(lastColor.b-color.b)*relativeIntensity;
					
					if(lastColor.b>color.b)
						newColor.b -= Mathf.Abs(lastColor.b-color.b)*relativeIntensity;
					
					if(newColor.a>maxTransparency)
						newColor.a = maxTransparency;
					
					//BLEND MODE: ADDITIVE disabled since it has no real use
					//Color newColor = new Color(lastColor.r+color.r*relativeIntensity, lastColor.g+color.g*relativeIntensity, lastColor.b+color.b*relativeIntensity, lastColor.a+color.a*relativeIntensity);
					
					texture.SetPixel(x, y, newColor);
				}
			}
		}
		
        texture.Apply();
		renderer.material.mainTexture = texture;
	}
}

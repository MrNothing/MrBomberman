Shader "Custom/Vertex Colored" 
{
	Properties 
	{
		
	}
	
    SubShader 
    {
	    Tags 
	    {
	    	Queue=Transparent
		}
	    
	    Blend SrcAlpha OneMinusSrcAlpha 
	    
	    Pass 
	    {
	        ColorMaterial AmbientAndDiffuse
	    }
    } 
}
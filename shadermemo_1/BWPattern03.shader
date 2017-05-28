Shader "Custom/BWPattern03" { //BlackAndWhitePattern
	
	

	Properties{
		
		_Debug_Stop("Debug_Stop",Int)=0
		_Debug_Opaque("Debug_Opaque",Int)=0
		
		_Debug_Random("Debug_Random",Int)=1
		
		_MainTex01("Texture01",2D)="white"{}
		_MainTex02("Texture02",2D)="white"{}
		_BGColor("BackGroundColor",Color)=(1,1,1)
		_NoiseColor01("NoiseColor01",Color)=(1,1,1)
		_NoiseColor02("NoiseColor02",Color)=(0,0,0)
		_NoiseColor03("NoiseColor03",Color)=(0.5,0.5,0.5)

		_Speed01("Speed01",Float)=0.1
		_Speed02("Speed02",Float)=100.0
		_Speed03("Speed03",Float)=100.0
		_Speed04("Speed04",Float)=500.0
		_Speed05("Speed05",Float)=500.0
		
		_PScale01("PatternScale01",Float)=10.0
		_PScale02("PatternScale02",Float)=10.0

		_PRow("PatternRowNumber",Int)=5
		_PCol("PatternColumn",Int)=5
		_Margin("MarginWidth",Float)=5.0
		
		_PNoise01("PatternNoise01",Int)=1


		_PAlpha("PAlpha",Float)=1.0
		_Seed01("Seed01",Int)=1

	}


SubShader{

	Tags{"Queue"="Transparent"}
	//Tags{"Queue"="Overlay"}
	//Blend SrcAlpha One
	Blend SrcAlpha OneMinusSrcAlpha
	//Cull BackLighting Off ZWrite Off
	//ZTest Always

	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag 

		int _Debug_Random=1;

		float4 _BGColor;

		float _Speed01;
		float _Speed02;
		float _Speed03;
		float _Speed04;
		float _Speed05;

		float _PScale01;
		float _PScale02;
		int seed01;
		float _PAlpha;
		int _PNoise01;

		int _PRow;
		int _PCol;
		int rwidth;
		int cwidth;
		

		float time=0;
		float timer=1.0;

		float noisecol=1.0;
		int bw=1;
		uint rng_state=131;


		uint rand_lcg()
		{
			// LCG values from Numerical Recipes
			rng_state = 1664525 * rng_state + 1013904223;
			return rng_state;
		}

		uint rand_xorshift()
		{
			// Xorshift algorithm from George Marsaglia's paper
			rng_state ^= (rng_state << 13);
			rng_state ^= (rng_state >> 17);
			rng_state ^= (rng_state << 5);
			return rng_state;
		}

		float GetRandomNumber(float2 texCoord, int Seed)
		{
    		return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
		}

		float4 vert(float4 v:POSITION):SV_POSITION{
			return mul(UNITY_MATRIX_MVP,v);
			
		}

		fixed4 frag(float4 sp:WPOS):COLOR{

			fixed4 ret=(1,1,1,1);
			int rk,ck;
			float rf;
	
		
			rwidth=_ScreenParams.y / _PRow;
			rk=sp.y/rwidth;
		
		
			cwidth=_ScreenParams.x / _PCol;
			ck=sp.x/cwidth;
			
			float t=sin(_Speed01*_Time.z);


			float x=(rk*_Speed04+ck*_Speed05+_Time.z)*_Speed02;
			if(_PNoise01){
					//x+=(rand(seed(rk,ck)))
				}
			if(t<0.0){
				
				ret= fixed4(0.0,0.0,0.0,1.0);
				//float rf=float(rand_xorshift()) * (1.0 / 4294967296.0);
				//ret.x=ret.y=ret.z=rf;
				
				rf=(1.0+(sin(x)))/2;
				ret=fixed4(rf,rf,rf,_PAlpha);
				}
			else{
				ret=fixed4(1.0,1.0,1.0,1.0);
				rf=(1.0+(cos(x)))/2;
				ret=fixed4(rf,rf,rf,_PAlpha);
			}

			
			return ret;

		}

		
		ENDCG

	}

}
	Fallback "Diffuse"

}
Shader "Custom/BWPatternUV03" { //BlackAndWhitePattern
	
	

	Properties{
		
		_Debug_Stop("Debug_Stop",Int)=0
		
		_Debug_Random("Debug_Random",Int)=1
		_Debug_Sound("Debug_Sound",Int)=1
		_SoundSync("SoundSync",Int)=1
		_SyncRow("SoundSync_Row",Int)=1
		_SyncCol("SoundSync_Col",Int)=1

		
		_BGColor("BackGroundColor",Color)=(1,1,1)
		
		_Speed01("Speed01",Float)=0.1
		_Speed02("Speed02",Float)=100.0
		_Speed03("Speed03",Float)=100.0
		_Speed04("Speed04",Float)=500.0
		_Speed05("Speed05",Float)=500.0
		
		_PRowWidth("PatternRowWidth 100/width",Int)=5
		_PColWidth("PatternColumnWidth 100/width ",Int)=5

		_SoundVolume("SoundVolume",Float)=0.0
		_SoundSyncScale("SoundSyncScale",Int)=5000
						
		
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
		#pragma vertex vert_img
		#pragma fragment frag 
		#include "UnityCG.cginc"

		int _Debug_Random=1;
		int _Debug_Sound=1;
		int _SoundSync;
		int _SyncRow;
		int _SyncCol;

		float4 _BGColor;

		float _Speed01;
		float _Speed02;
		float _Speed03;
		float _Speed04;
		float _Speed05;
		
		int seed01;
		float _PAlpha;
		int _PNoise01;

		int _PRowWidth;
		int _PColWidth;

		float _SoundVolume;
		int _SoundSyncScale;

		float time=0;
		float timer=1.0;

		
		float4 vert(float4 v:POSITION):SV_POSITION{
			return mul(UNITY_MATRIX_MVP,v);
			
		}

		fixed4 frag(v2f_img iuv):COLOR{

			fixed4 ret=(1,1,1,1);
			int rk,ck;
			float rf;
	
			if(_SoundSync){
				if(_SyncRow==1)_PRowWidth=_SoundVolume*_SoundSyncScale+1;
				if(_SyncCol==1)_PColWidth=_SoundVolume*_SoundSyncScale+1;
			}
			rk=iuv.uv.y*100;
			ck=iuv.uv.x*100;
			
			rk/=_PRowWidth;
			ck/=_PColWidth;

			float t=sin(_Speed01*_Time.z);
			if(_SoundSync){
				//rk=ck=(int)(_SoundVolume*1000);
				t=sin(_SoundVolume*_Time.z);
								
			}

			float x=(rk*_Speed04+ck*_Speed05+_Time.z)*_Speed02;
			if(_SoundSync)x*=-1*_SoundVolume;
			
			if(t<0.0){
								
				rf=(1.0+(sin(x)))/2;
			}
			else{
				rf=(1.0+(cos(x)))/2;
				
			}
			
			ret=fixed4(rf,rf,rf,_PAlpha);
			return ret;

		}

		
		ENDCG

	}

}
	Fallback "Diffuse"

}
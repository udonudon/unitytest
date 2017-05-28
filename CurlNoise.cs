/*
reference :
 
 https://github.com/ashima/webgl-noise/blob/master/src/noise3D.glsl
 http://blog.livedoor.jp/akinow/archives/52378824.html 
 https://docs.unity3d.com/jp/540/ScriptReference/ParticleSystem.GetParticles.html
 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurlNoise : MonoBehaviour {

	
	public bool debug1=true;

	public Vector3 Amount=new Vector3(1.0f,1.0f,1.0f);
	const int bufferSize=1000;
	
	ParticleSystem psystem=new ParticleSystem();
	private ParticleSystem.Particle[] particles=new ParticleSystem.Particle[bufferSize];
	private float[] dx=new float[bufferSize];
	private float[] dy=new float[bufferSize];
	private float[] dz=new float[bufferSize];

	public float scale=0.1f;
	public float speed=0.05f;

	public GameObject gobj;


	Vector3 mod289(Vector3 x){
		Vector3 ret=new Vector3();
		for(int i=0;i<3;i++){
			ret[i]=x[i]-Mathf.Floor(x[i]*(1.0f/289.0f))*289.0f;
		}
		return ret;
		//return x-Mathf.Floor(x * (1.0f / 289.0f)) * 289.0f;
	}
	Vector4 mod289(Vector4 x){
		Vector4 ret=new Vector4();
		for(int i=0;i<4;i++){
			ret[i]=x[i]-Mathf.Floor(x[i]*(1.0f/289.0f))*289.0f;
		}
		return ret;
	}


	Vector4 permute(Vector4 x){
		Vector4 ret=new Vector4();
		for(int i=0;i<4;i++){
			ret[i]=((x[i]*34.0f)+1.0f)*x[i];
		}
		return mod289(ret);
	}

	Vector4 taylorInvSqrt(Vector4 r)
	{
		Vector4 ret=new Vector4();
		ret=1.79284291400159f*Vector4.one-0.85373472095314f*r;
		return ret;
  		//return 1.79284291400159 - 0.85373472095314 * r;
	}
	
	Vector2 floor(Vector2 v){
		Vector2 ret=new Vector2();
		for(int i=0;i<2;i++){
			ret[i]=Mathf.Floor(v[i]);
		}
		return ret;
	}

	Vector3 floor(Vector3 v){
		Vector3  ret=new Vector3();
		for(int i=0;i<3;i++){
			ret[i]=Mathf.Floor(v[i]);
		}
		return ret;
	}

	Vector4 floor(Vector4 v){
		Vector4 ret=new Vector4();
		for(int i=0;i<4;i++){
			ret[i]=Mathf.Floor(v[i]);
		}
		return ret;
	}

	float dot(Vector3 v1,Vector3 v2){
		return Vector3.Dot(v1,v2);
	}

	int step(float y,float x){
		return (x>=y)?1:0;
	}

	Vector3 step(Vector3 v1,Vector3 v2){
		Vector3 ret=new Vector3();
		for(int i=0;i<3;i++){
			ret[i]=step(v1[i],v2[i]);
		}
		return ret;
	}

	Vector3 min(Vector3 v1,Vector3 v2){
		Vector3 ret=new Vector3();
		for(int i=0;i<3;i++){
			ret[i]=Mathf.Min(v1[i],v2[i]);
		}
		return ret;
	}
	Vector3 max(Vector3 v1,Vector3 v2){
		Vector3 ret=new Vector3();
		for(int i=0;i<3;i++){
			ret[i]=Mathf.Max(v1[i],v2[i]);
		}
		return ret;
	}
	
	Vector4 abs(Vector4 v){
		Vector4 ret=new Vector4();
		for(int i=0;i<4;i++){
			ret[i]=Mathf.Abs(v[i]);
		}
		return ret;
	}

	float snoise(Vector3 v){

		Vector2  C = new Vector2(1.0f/6.0f, 1.0f/3.0f) ;
		Vector4  D = new Vector4(0.0f, 0.5f, 1.0f, 2.0f);

		// First corner
		Vector3 i  = floor(v + dot(v, C.y*Vector3.one)*Vector3.one );
		Vector3 x0 =   v - i + dot(i, C.x*Vector3.one)*Vector3.one ;

		// Other corners
		
		Vector3 g=step(new Vector3(x0.y,x0.z,x0.x),x0);
		//Vector3 g = step(x0.yzx, x0.xyz);
		Vector3 l = 1.0f*Vector3.one - g;
		

		Vector3 i1 = min( g, new Vector3(l.z,l.x,l.y));
		Vector3 i2 = max( g, new Vector3(l.z,l.x,l.y));
		/*
		Vector3 i1 = min( g.xyz, l.zxy );
		Vector3 i2 = max( g.xyz, l.zxy );
		*/
		//   x0 = x0 - 0.0 + 0.0 * C.xxx;
		//   x1 = x0 - i1  + 1.0 * C.xxx;
		//   x2 = x0 - i2  + 2.0 * C.xxx;
		//   x3 = x0 - 1.0 + 3.0 * C.xxx;
		Vector3 x1 = x0 - i1 + C.x*Vector3.one;
		Vector3 x2 = x0 - i2 + C.y*Vector3.one; // 2.0*C.x = 1/3 = C.y
		Vector3 x3 = x0 - D.y*Vector3.one;      // -1.0+3.0*C.x = -0.5 = -D.y

		// Permutations
		i = mod289(i);  //i:Vector3
		Vector4 p = permute( permute( permute( 
					i.z*Vector4.one + new Vector4(0.0f, i1.z, i2.z, 1.0f ))
				+ i.y*Vector4.one + new Vector4(0.0f, i1.y, i2.y, 1.0f )) 
				+ i.x*Vector4.one + new Vector4(0.0f, i1.x, i2.x, 1.0f ));

		// Gradients: 7x7 points over a square, mapped onto an octahedron.
		// The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
		float n_ = 0.142857142857f; // 1.0/7.0
		
		//Vector3  ns = n_ * D.wyz - D.xzx;
		Vector3 ns = n_ * new Vector3(D.x,D.y,D.z) - new Vector3(D.x,D.z,D.x);

		Vector4 j = p - 49.0f * floor(p * ns.z * ns.z);  //  mod(p,7*7)

		Vector4 x_ = floor(j * ns.z);
		Vector4 y_ = floor(j - 7.0f * x_ );    // mod(j,N)

		Vector4 x = x_ *ns.x + ns.y*Vector4.one;
		Vector4 y = y_ *ns.x + ns.y*Vector4.one;
		Vector4 h = 1.0f*Vector4.zero - abs(x) - abs(y);

		/*
		Vector4 b0 = Vector4( x.xy, y.xy );
		Vector4 b1 = Vector4( x.zw, y.zw );
		*/
		Vector4 b0 = new Vector4(x.x,x.y,y.x,y.y);
		Vector4 b1 = new Vector4(x.z,x.w,y.z,y.w);

		//Vector4 s0 = Vector4(lessThan(b0,0.0))*2.0 - 1.0;
		//Vector4 s1 = Vector4(lessThan(b1,0.0))*2.0 - 1.0;
		Vector4 s0 = floor(b0)*2.0f + 1.0f*Vector4.one;
		Vector4 s1 = floor(b1)*2.0f + 1.0f*Vector4.one;
		Vector4 sh = -1*step(h, 0.0f*Vector4.one);

		/*
		Vector4 a0 = b0.xzyw + s0.xzyw*sh.xxyy ;
		Vector4 a1 = b1.xzyw + s1.xzyw*sh.zzww ;
		*/
		Vector4 a0=new Vector4(b0.x,b0.z,b0.y,b0.w) + Vector4.Scale(new Vector4(s0.x,s0.z,s0.y,s0.w),new Vector4(sh.x,sh.x,sh.y,sh.y));
		Vector4 a1=new Vector4(b1.x,b1.z,b1.y,b1.w) + Vector4.Scale(new Vector4(s1.x,s1.z,s1.y,s1.w),new Vector4(sh.z,sh.z,sh.w,sh.w));


		/*
		Vector3 p0 = Vector3(a0.xy,h.x);
		Vector3 p1 = Vector3(a0.zw,h.y);
		Vector3 p2 = Vector3(a1.xy,h.z);
		Vector3 p3 = Vector3(a1.zw,h.w);
		*/
		Vector3 p0=new Vector3(a0.x,a0.y,h.x);
		Vector3 p1=new Vector3(a0.z,a0.w,h.y);
		Vector3 p2=new Vector3(a1.x,a1.y,h.z);
		Vector3 p3=new Vector3(a1.z,a1.w,h.w);

		//Normalise gradients
		Vector4 norm = taylorInvSqrt(new Vector4(dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3)));
		p0 *= norm.x;
		p1 *= norm.y;
		p2 *= norm.z;
		p3 *= norm.w;

		// Mix final noise value
		Vector4 m = max(0.6f*Vector4.one - new Vector4(dot(x0,x0), dot(x1,x1), dot(x2,x2), dot(x3,x3)), 0.0f*Vector4.one);
		//m = m * m;
		m=Vector4.Scale(m,m);
		/*
		return 42.0f * dot( m*m, new Vector4( dot(p0,x0), dot(p1,x1), 
										dot(p2,x2), dot(p3,x3) ) );
		*/
		return 42.0f * dot( Vector4.Scale(m,m), new Vector4( dot(p0,x0), dot(p1,x1), 
										dot(p2,x2), dot(p3,x3) ) );


	}

	void InitializeIfNeeded(){
		if(psystem == null){
			psystem=GetComponent<ParticleSystem>();
		}
		if(particles == null || particles.Length < psystem.maxParticles){
			particles=new ParticleSystem.Particle[psystem.maxParticles];
		}
	}



	void Start () {
		InitializeIfNeeded();
	}
	
	
	void LateUpdate () {

		
		//int length = ParticleSystem.GetParticles(particles);
		int length=psystem.GetParticles(particles);
		int i=0;

		while(i<length){
			
			float  scalex = Time.time  * speed + 0.1365143f; 
			float  scaley = Time.time  * speed +   1.21688f;    
			float  scalez = Time.time  * speed +    2.5564f;   

						
			dx[i] = snoise(new Vector3(particles[i].position.x*scale, particles[i].position.y*scale, particles[i].position.z*scalex)); 
			dy[i] = snoise(new Vector3(particles[i].position.x*scale, particles[i].position.y*scale, particles[i].position.z*scaley)); 
			dz[i] = snoise(new Vector3(particles[i].position.x*scale, particles[i].position.y*scale, particles[i].position.z*scalez));

			particles[i].position += new Vector3(dx[i]*Amount.x, dy[i]*Amount.y, dz[i]*Amount.z) ;            
			i++;     
						
		}
		psystem.SetParticles(particles,length);
	}
}

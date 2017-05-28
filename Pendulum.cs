// reference : http://ja.vcssl.org/code/archive/0001/0200-duplex-pendulum/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pendulum : MonoBehaviour {


	public bool debug1=true;
	public bool debug2=true;
	public bool debug_spherepos=true;

	public double gravity=9.8f;
	public double length1=1.4f;
	public double length2=1.2f;
	public double mass1=1.2f;
	public double mass2=0.1f;
	public float barscale=0.5f;

	private double theta1=0.1f;
	private double theta2=-2.0f;
	private double theta1Dot=0.0f;
	private double theta2Dot=0.0f;


	private float timer;
	public  float deltatime;
	public int calcnum=5000;

	public int centerX;
	public int centerY;

	private Vector3 centerpos;

	public GameObject sphere1;
	public GameObject sphere2;

	public  GameObject bar1;
	public  GameObject bar2;



	
	
	


	// Use this for initialization
	void Start () {

		timer=0.0f;
		centerpos=new Vector3(centerX,centerY,0);
		/*
		sphere1=GameObject.FindGameObjectWithTag("PendulumSphere1");
		sphere2=GameObject.FindGameObjectWithTag("PendulumSphere2");
		bar1=GameObject.FindGameObjectWithTag("PendulumBar1");
		bar2=GameObject.FindGameObjectWithTag("PendulumBar2");
		*/
		bar1.transform.localScale=new Vector3((float)length1,barscale,barscale*2);
		bar2.transform.localScale=new Vector3((float)length2,barscale,barscale);
		//bar1.transform.localPosition.x+=barscale/2;

		

	}

	void calc(){

		

		double theta1DotDot,theta2DotDot;
		double a1,a2,b,d1,d2;

		a1=(mass1+mass2)*length1*length2;
		a2=mass2*length2*length2;

		for(int i=0;i<calcnum;i++){
			b = mass2 * length1 * length2 * Math.Cos( theta1 - theta2 );
			d1 = -mass2 * length1 * length2 * theta2Dot * theta2Dot * Math.Sin( theta1 - theta2 ) - ( mass1 + mass2 ) * gravity * length1 * Math.Sin( theta1 );
			d2 = mass2 * length1 * length2 * theta1Dot * theta1Dot * Math.Sin( theta1 - theta2 ) - mass2 * gravity * length2 * Math.Sin( theta2 );

			// オイラー法で、振り子をdeltaTだけ運動させる
			if(debug1){
				if((a1*a2-b*b)==0)Debug.Log("zero divide");
			}
			theta1DotDot = ( a2*d1 - b*d2 ) / ( a1*a2 - b*b );
			theta2DotDot = ( a1*d2 - b*d1 ) / ( a1*a2 - b*b );
			theta1Dot += theta1DotDot * deltatime;
			theta2Dot += theta2DotDot * deltatime;
			theta1 += theta1Dot * deltatime;
			theta2 += theta2Dot * deltatime;
			
		}

	}


	void move(){

		double point1X=length1*Math.Sin(theta1)+centerX;
		double point1Y=length1*Math.Cos(theta1)+centerY;
		double point2X=length2*Math.Sin(theta2)+point1X;
		double point2Y=length2*Math.Cos(theta2)+point1Y;

		
		Vector3 sphere1pos=sphere1.transform.position;
		Vector3 sphere2pos=sphere2.transform.position;

		Vector3 newpos1=new Vector3((float)point1X,(float)point1Y,sphere1pos.z);
		Vector3 newpos2=new Vector3((float)point2X,(float)point2Y,sphere2pos.z);

		
		
		
		if(debug_spherepos){
			Debug.Log("length1="+length1+"theta1="+theta1+"Sin(theta1)="+Math.Sin(theta1)+"centerX="+centerX);
			Debug.Log("sphere1pos="+sphere1pos);
			Debug.Log("sphere2pos="+sphere2pos);
			Debug.Log("newpos1="+newpos1);
			Debug.Log("newpos2="+newpos2);
		}

		sphere1.transform.position=newpos1;
		sphere2.transform.position=newpos2;

		
		bar1.transform.rotation=Quaternion.Euler(0,0,-1*(float)theta1/Mathf.PI*180+90);
		bar1.transform.position=(centerpos+newpos1)/2;

		bar2.transform.rotation=Quaternion.Euler(0,0,-1*(float)theta2/Mathf.PI*180+90);
		bar2.transform.position=(newpos1+newpos2)/2;
		



	}
	
	// Update is called once per frame
	void Update () {

		timer+=Time.deltaTime;
		if(timer > deltatime){
			timer=0.0f;
			calc();
			move();
		}

				
	}
}

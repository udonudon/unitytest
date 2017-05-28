using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumGenerator : MonoBehaviour {

	// Use this for initialization

	public int generate_num=10;

	public GameObject dpendulum_prefab;
	void Start () {
		dpendulum_prefab=GameObject.FindGameObjectWithTag("dpendulum");
		generate();
	}
	void generate(){

		for(int i=0;i<generate_num;i++){
			GameObject pref=Instantiate(dpendulum_prefab,transform.position,transform.rotation) as GameObject;
			pref.GetComponent<Pendulum>().gravity+=0.1*i;
			
		}

	}
	// Update is called once per frame
	void Update () {
		
	}
}

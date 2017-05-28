using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVisuoEffect : MonoBehaviour {


	public bool debug=true;
	public bool debug_printrms=true;
//	public AudioSource as01;

	private Material mat01;
	private float rms=0.0f;
	private float pitch=0.0f;


	// Use this for initialization
	void Start () {
		
//		as01=GetComponent<AudioSource>();
		mat01=GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		float[] spectrum =new float[256];
		AudioListener.GetSpectrumData(spectrum,0,FFTWindow.Rectangular);

		if(debug){Debug.Log("spectrum[100]="+spectrum[100]);
		for(int i=1;i< spectrum.Length-1;i++){
			Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
		}
		}

		rms=0.0f;
		float sum=0.0f;
		for(int i=0;i<spectrum.Length;i++){
			sum+=spectrum[i]*spectrum[i];
		}
		rms=Mathf.Sqrt(sum/spectrum.Length);
		mat01.SetFloat("_SoundVolume",rms);
		if(debug_printrms)Debug.Log("rms="+rms);
		
	}
}

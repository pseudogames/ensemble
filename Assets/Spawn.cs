using UnityEngine;
using System.Collections;

public class Spawn : Photon.MonoBehaviour {

	public GameObject target = null;
	public float threshold = 0.8f;
	public float persistence = 0.99f;
	
	public float delay = 0.25f;
	private float timeout = 0f;
	
	private float max = 0.00001f;
	private float avg = 0f;
	private float sum = 0f;
	private float count = 0f;
	private float actuation = 0f;
	private bool trigger = false;

	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
	
	
	void OnAudioFilterRead(float[] data, int channels) {
		for (int i = 1; i < data.Length; ++i) {
			float d = data[i] - data[i-1];
			sum += d * d;
		}
		count += data.Length;
	}
	
	void Update () {
		if(count > 0) avg = sum / count;
		sum = 0;
		count = 0;
		max *= persistence;
		if(avg > max) max = avg;
		actuation = avg / max;
		if(target != null) {
			if(actuation >= threshold) trigger = true;
			if(trigger && Time.time > timeout) {
				trigger = false;
				timeout = Time.time + delay;
				
				// create enemy
				GameObject obj = PhotonNetwork.Instantiate("Enemy", gameObject.transform.position, gameObject.transform.rotation, 0);
				Enemy enemy = obj.GetComponent("Enemy") as Enemy;
				enemy.spawn = gameObject;
			}
		}
		gameObject.renderer.material.color = (actuation*0.5f+0.25f) * Color.white;
	}
	
	public float GetActuation() {
		return actuation;
	}

}

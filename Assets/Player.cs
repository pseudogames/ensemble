using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {

	public float damage = 0.05f;
	public LayerMask mask;
	private bool fire = false;

	private Vector3 correctPos = Vector3.zero;
	private Quaternion correctRot = Quaternion.identity;
	private Vector3 correctScale = Vector3.one;
	
    void Update()
    {
        if (!photonView.isMine)
		{
            transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * 5);
            transform.localScale = Vector3.Lerp(transform.localScale, this.correctScale, Time.deltaTime * 5);
        }
    }
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
        }
        else
        {
            this.correctPos = (Vector3)stream.ReceiveNext();
            this.correctRot = (Quaternion)stream.ReceiveNext();
            this.correctScale = (Vector3)stream.ReceiveNext();
        }
    }
	
	

	
	void FixedUpdate () {
	
		if(photonView.isMine) {
			// flamethrower toggle
			if(Input.GetButtonDown("Fire1")) fire = !fire;
			if(Input.GetButton("Fire2")) fire = false;
				
			if(fire || Input.GetButton("Fire1")) {
		
				photonView.RPC("FireStart", PhotonTargets.All);
				
				RaycastHit hit;
				Vector3 jitter = transform.right * (Random.value - 0.5f) + transform.up * (Random.value - 0.5f);
				Vector3 direction = transform.forward + jitter * 0.6f;
				float distance = 6;
		        if (Physics.Raycast(transform.position, direction, out hit, distance, mask)) {
					Debug.DrawRay(transform.position, direction * distance, Color.red, 0.1f); 
					(hit.collider.GetComponent("PhotonView") as PhotonView).RPC("TakeDamage", PhotonTargets.All, damage);
		        }
		        
			} else {
				photonView.RPC("FireStop", PhotonTargets.All);
			}
		
			// healing
			if(gameObject.transform.localScale.y < 1) {
				float health = gameObject.transform.localScale.y * 0.99f + 0.01f;
				float heal = health - gameObject.transform.localScale.y;
				gameObject.transform.position += Vector3.up * heal;
				Vector3 scale = gameObject.transform.localScale;
				scale.y = health;
				gameObject.transform.localScale = scale;
				renderer.material.color = Color.white;
			}
		}
	}

	[RPC]
	void FireStart() {
		particleSystem.Play();
		audio.volume = audio.volume * 0.95f + 0.05f * 0.1f; // max volume
	}
	
	[RPC]
	void FireStop() {
		particleSystem.Stop();
		audio.volume = audio.volume * 0.9f + 0.1f * 0f; // min volume
	}
	
	[RPC]
	void TakeDamage (float damage) {
		renderer.material.color = Color.black;
		
		if(photonView.isMine) {
			Vector3 scale = transform.localScale;
			scale.y = scale.y * (0.98f + 0.02f * (1.0f - damage)); // the smaller the enemy, the less the damage  // FIXME damage hardcoded
			if(scale.y < 0.25f) {
				scale.y = 1.0f;
				transform.position = Vector3.up * 7.0f;
			}
			transform.localScale = scale;
		}
	}

}
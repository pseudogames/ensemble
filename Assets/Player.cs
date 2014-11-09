using UnityEngine;
using System.Collections;

public class Player : Photon.MonoBehaviour {

	public float damage = 0.05f;
	public LayerMask mask;
	private bool want_fire = false;
	private bool firing = false;
	private bool bleeding = false;

	private Vector3 correctPos = Vector3.zero;
	private Quaternion correctRot = Quaternion.identity;
	
	void Start()
	{
		if(photonView.isMine) {
			// TODO enable collision event emission on particleSystem only if photonView.isMine
		}
	}


    void Update()
    {
        if (!photonView.isMine)
		{
            transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * 5);
        }
    }
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.correctPos = (Vector3)stream.ReceiveNext();
            this.correctRot = (Quaternion)stream.ReceiveNext();
        }
    }
	
	

	
	void FixedUpdate () {
	
		if(photonView.isMine) {
			// flamethrower toggle
			if(Input.GetButtonDown("Fire1")) want_fire = !want_fire;
			if(Input.GetButton("Fire2")) want_fire = false;
				
			if(firing != want_fire) {
				firing = want_fire;
				if(want_fire) {
					photonView.RPC("FireStart", PhotonTargets.All);
				} else {
					photonView.RPC("FireStop", PhotonTargets.All);
				}
			}
		}

		// healing
		if(!bleeding && transform.localScale.y < 1) {
			renderer.material.color = Color.white;

			float health = transform.localScale.y * 0.99f + 0.01f;
			float heal = health - transform.localScale.y;
			
			transform.position += Vector3.up * heal;
			
			Vector3 scale = transform.localScale;
			scale.y = health;
			transform.localScale = scale;
		}
		bleeding = false;
	}

	void OnParticleCollision (GameObject other) {
		if(photonView.isMine && other.layer == 8) { // me hitting enemy
			PhotonView pv = other.GetComponent("PhotonView") as PhotonView;
			if(pv) {
				pv.RPC("Burn", PhotonTargets.All, damage);
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
	void Bitten (float damage) {
		bleeding = true;
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

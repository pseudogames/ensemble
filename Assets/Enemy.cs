using UnityEngine;
using System.Collections;

public class Enemy : Photon.MonoBehaviour {

	public GameObject spawn;
	private NavMeshAgent nma;

	
	private Vector3 correctPos = Vector3.zero;
	private Vector3 correctScale = Vector3.one;
	//private Quaternion correctRot = Quaternion.identity;
	
    void Update()
    {
        if (!photonView.isMine)
		{
            transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * 5);
            transform.localScale = Vector3.Lerp(transform.localScale, this.correctScale, Time.deltaTime * 5);
            //transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * 5);
        }
    }
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            //stream.SendNext(transform.rotation);
        }
        else
        {
            this.correctPos = (Vector3)stream.ReceiveNext();
            this.correctScale = (Vector3)stream.ReceiveNext();
            //this.correctRot = (Quaternion)stream.ReceiveNext();
        }
    }
	
		
	
	void Start () {
		nma = GetComponent("NavMeshAgent") as NavMeshAgent;
		InvokeRepeating("Restore", 0, 1);
	}
	
	void Restore () {
		renderer.material.color = Color.white;
		if(photonView.isMine) {
			Vector3 scale = transform.localScale;
			scale.x = scale.z = 1.0f;
			transform.localScale = scale;
		}
	}
	
	void FixedUpdate () {
		if(photonView.isMine) {
			GameObject target = (spawn.GetComponent("Spawn") as Spawn).target;
			float actuation = (spawn.GetComponent("Spawn") as Spawn).GetActuation();
			
			float effective = actuation * transform.localScale.y;
			
			(GetComponent("NavMeshAgent") as NavMeshAgent).speed = 6.0f * effective; // FIXME speed hardcoded
		
			float distance = Vector3.Distance(target.transform.position, transform.position);
			float touch = target.collider.bounds.extents.magnitude + collider.bounds.extents.magnitude;
			if(distance > touch*1.05) {
				nma.destination = target.transform.position;
			} else {
				photonView.RPC("Attack", PhotonTargets.All, effective);
				(target.GetComponent("PhotonView") as PhotonView).RPC("Bitten", PhotonTargets.All, effective);
			}
		}
	}
	
	[RPC]
	void Attack (float damage) {
		renderer.material.color = Color.red;
		
		if(photonView.isMine) {
			float wide = 1f + damage;
	
			Vector3 scale = transform.localScale;
			scale.x = scale.z = scale.z * 0.5f + 0.5f * wide;
			transform.localScale = scale;
		}
	}

	[RPC]
	void Burn (float damage) {
		renderer.material.color = Color.black;

		if(photonView.isMine) {
			Vector3 scale = transform.localScale;
			scale.y = scale.y * (1.0f - damage);
			transform.localScale = scale;
			if(scale.y < 0.25f) {
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}

}

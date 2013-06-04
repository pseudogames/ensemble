using UnityEngine;
using System.Collections;

public class Enemy : Photon.MonoBehaviour {

	public GameObject spawn;
	private NavMeshAgent nma;

	
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
	
		
	
	void Start () {
		nma = GetComponent("NavMeshAgent") as NavMeshAgent;
		InvokeRepeating("Restore", 0, 1);
	}
	
	void Restore () {
		renderer.material.color = Color.white;
	}
	
	void FixedUpdate () {
		if(photonView.isMine) {
			GameObject target = (spawn.GetComponent("Spawn") as Spawn).target;
			float actuation = (spawn.GetComponent("Spawn") as Spawn).GetActuation();
			
			float effective = actuation * transform.localScale.y;
			
			(GetComponent("NavMeshAgent") as NavMeshAgent).speed = 6.0f * effective; // FIXME speed hardcoded
		
			Vector3 scale = target.transform.localScale;
			float distance = Vector3.Distance(target.transform.position, transform.position);
			float touch = target.collider.bounds.extents.magnitude + collider.bounds.extents.magnitude;
			float wide = 1.0f;
			if(distance > touch*1.05) {
				nma.destination = target.transform.position;
			} else {
				wide = 1f + effective;
				renderer.material.color = Color.red;
			
				(target.GetComponent("PhotonView") as PhotonView).RPC("TakeDamage", PhotonTargets.All, effective);
			}
			scale.x = scale.z = scale.z * 0.5f + 0.5f * wide;
			transform.localScale = scale;
		}
	}
	
	[RPC]
	void TakeDamage (float damage) {
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

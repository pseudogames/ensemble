using UnityEngine;

public class Hunter : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private Color correctPlayerColor = Color.white;
    private bool fire = false;
	private int pain = 0;

    void Update()
    {
        if (photonView.isMine)
        {
			fire = Input.GetButton("Fire1");
		} else {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
            renderer.material.color = Color.red * Mathf.Lerp(renderer.material.color.r, this.correctPlayerColor.r, Time.deltaTime * 5);
        }
		renderer.material.color = pain > 0 ? Color.black : fire ? Color.red : Color.white;
		pain--;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(fire); // TODO change to RPC
        }
        else
        {
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            fire = (bool)stream.ReceiveNext();
        }
    }
	
    [RPC]
    void TakeDamage()
    {
        pain = 5;
		if(photonView.isMine) {
			if(Random.value < 0.1) {
				Debug.Log ("die");
				PhotonNetwork.Destroy(photonView);
			}
		}
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
		if(fire && hit.gameObject.tag == "Player") {
			(hit.gameObject.GetComponent("PhotonView") as PhotonView).RPC("TakeDamage", PhotonTargets.All);
		}
    }
	
}

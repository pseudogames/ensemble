using UnityEngine;

public class Hunter : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private Color correctPlayerColor = Color.white;
    private bool fire = false;

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
		renderer.material.color = fire ? Color.red : Color.white;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(fire);
        }
        else
        {
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            fire = (bool)stream.ReceiveNext();
        }
    }
}

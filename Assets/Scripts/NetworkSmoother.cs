using UnityEngine;
using Photon.Pun;

public class NetworkSmoother : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public float smoothSpeed = 15f;

    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                networkPosition,
                Time.deltaTime * smoothSpeed);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                networkRotation,
                Time.deltaTime * smoothSpeed);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
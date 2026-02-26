using UnityEngine;
using Photon.Pun;

public class NetworkSmoother : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public float smoothSpeed = 15f;

    void Update()
    {
        if (photonView.IsMine) return;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            networkPosition,
            Time.deltaTime * smoothSpeed);

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            networkRotation,
            Time.deltaTime * smoothSpeed);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.localPosition);
            stream.SendNext(transform.localRotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
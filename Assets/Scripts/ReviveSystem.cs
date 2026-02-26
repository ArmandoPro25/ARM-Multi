using UnityEngine;
using Photon.Pun;

public class ReviveSystem : MonoBehaviourPun
{
    public float reviveDistance = 3f;

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryRevive();
        }
    }

    void TryRevive()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, reviveDistance);

        foreach (Collider col in hits)
        {
            PlayerHealth health = col.GetComponent<PlayerHealth>();

            if (health != null &&
                health.isDowned &&
                health.photonView != photonView)
            {
                health.photonView.RPC("SyncDownState", RpcTarget.All, false);
                break;
            }
        }
    }
}
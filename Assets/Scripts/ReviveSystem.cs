using UnityEngine;
using Photon.Pun;

public class ReviveSystem : MonoBehaviourPun
{
    public float reviveDistance = 3f;
    public float timeWait = 3f;

    private float total = 0f;

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKey(KeyCode.E))
        {
            PlayerHealth target = GetDownedPlayerNearby();

            if (target != null)
            {
                total += Time.deltaTime;

                if (total >= timeWait)
                {
                    Debug.Log("Reviviendo...");
                    target.photonView.RPC("SyncDownState", RpcTarget.All, false);
                    total = 0f;
                }
            }
            else
            {
                total = 0f;
            }
        }
        else
        {
            total = 0f;
        }
    }

    PlayerHealth GetDownedPlayerNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, reviveDistance);

        foreach (Collider col in hits)
        {
            PlayerHealth health = col.GetComponent<PlayerHealth>();

            if (health != null &&
                health.isDowned &&
                health.photonView != photonView)
            {
                return health;
            }
        }
        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, reviveDistance);
    }
}
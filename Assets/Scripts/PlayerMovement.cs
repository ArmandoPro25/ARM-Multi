using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public float speed = 250f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Solo el jugador local controla su movimiento
        if (!photonView.IsMine)
        {
            rb.isKinematic = true; // Desactiva la física para otros jugadores
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(h, 0, v) * speed;
            rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);
        }
    }

}
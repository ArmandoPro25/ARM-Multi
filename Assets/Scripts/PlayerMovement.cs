using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f;
    public Transform groundCheck;
    public LayerMask groundMask;
    private bool isGrounded;
    private bool jumpRequest;
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

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }

    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            // Movimiento del jugador
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(h, 0, v) * speed;
            rb.linearVelocity = new Vector3(direction.x, rb.linearVelocity.y, direction.z);

            // Salto del jugador
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
            if (jumpRequest)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpRequest = false;
                isGrounded = false; // Evita múltiples saltos
            }
        }
    }
}
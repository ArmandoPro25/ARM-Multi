using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementFP : MonoBehaviourPunCallbacks
{
    public float speed = 6f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.3f;
    public Transform groundCheck;
    public LayerMask groundMask;
    private bool isGrounded;
    private bool jumpRequest;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }

        if (photonView.IsMine && Input.GetKeyDown(KeyCode.K))
        {
            PlayerHealth health = GetComponent<PlayerHealth>();
            health.photonView.RPC("TakeDamage", RpcTarget.All, 100);
            Debug.Log("Estas derrotado");
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        Vector3 newPosition = rb.position + move * speed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        if (jumpRequest)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequest = false;
        }
    }
}
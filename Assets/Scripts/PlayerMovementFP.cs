using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementFP : MonoBehaviourPunCallbacks
{
    [Header("Movimiento")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;

    [Header("Sprint")]
    public float maxSprintTime = 3f;
    public float sprintRegenRate = 1f;

    [Header("Suelo")]
    public float groundCheckDistance = 0.3f;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("UI")]
    public GameObject staminaBarPrefab;  // Arrastra aquí el prefab de la barra
    private GameObject staminaBarInstance;

    public float speedMultiplier = 1f;

    private float currentSprintTime;
    private bool isGrounded;
    private bool jumpRequest;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSprintTime = maxSprintTime;

        if (photonView.IsMine)
        {
            // Solo el jugador local crea la barra de stamina
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null && staminaBarPrefab != null)
            {
                staminaBarInstance = Instantiate(staminaBarPrefab, canvas.transform);
                staminaBarInstance.GetComponent<StaminaBarUI>().playerMovement = this;
            }
            else
            {
                Debug.LogWarning("No se encontró Canvas o falta el prefab de stamina.");
            }
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Verifica si estás en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequest = true;
        }

        // Debug de daño (opcional)
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerHealth health = GetComponent<PlayerHealth>();
            health.photonView.RPC("TakeDamage", RpcTarget.All, 100);
            Debug.Log("Estás derrotado");
        }

        // Control de sprint
        bool wantsToSprint = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        bool isSprinting = wantsToSprint && currentSprintTime > 0f && isGrounded;

        // Gastar sprint
        if (isSprinting)
        {
            currentSprintTime -= Time.deltaTime;
            if (currentSprintTime < 0f) currentSprintTime = 0f;
        }
        else // Regenerar sprint
        {
            if (currentSprintTime < maxSprintTime)
            {
                currentSprintTime += Time.deltaTime * sprintRegenRate;
                if (currentSprintTime > maxSprintTime) currentSprintTime = maxSprintTime;
            }
        }

        // Debug opcional para ver sprint restante
        Debug.Log("Sprint restante: " + currentSprintTime);
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;

        // Determina si puedes sprintar
        bool wantsToSprint = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        bool canSprint = wantsToSprint && currentSprintTime > 0f && isGrounded;

        float baseSpeed = canSprint ? sprintSpeed : walkSpeed;
        float currentSpeed = baseSpeed * speedMultiplier;

        Vector3 targetVelocity = move * currentSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;

        // Salto
        if (jumpRequest)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequest = false;
        }
    }
    public float SprintRatio => currentSprintTime / maxSprintTime;
}
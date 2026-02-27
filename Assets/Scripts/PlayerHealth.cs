using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public int maxHealth = 100;
    private int currentHealth;
    public bool isDowned = false;
    private PlayerMovementFP movement;
    private Rigidbody rb;
    public Transform playerModel;
    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;
    public float downedSpeed = 1f;

    void Start()
    {
        currentHealth = maxHealth;
        movement = GetComponent<PlayerMovementFP>();
        rb = GetComponent<Rigidbody>();
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        if (isDowned) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            DownPlayer();
        }
    }

    void DownPlayer()
    {
        isDowned = true;

        if (photonView.IsMine)
        {
            float targetSpeed = downedSpeed;
            movement.speedMultiplier = targetSpeed / movement.walkSpeed;
        }

        photonView.RPC("SyncDownState", RpcTarget.All, true);
    }

    [PunRPC]
    public void SyncDownState(bool downed)
    {
        isDowned = downed;

        if (downed)
        {
            if (photonView.IsMine)
            {
                // Ajustar multiplicador
                movement.speedMultiplier = downedSpeed / movement.walkSpeed;

                // Cambiar cámaras
                firstPersonCamera.SetActive(false);
                thirdPersonCamera.SetActive(true);
            }

            // Acostar el modelo
            playerModel.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            currentHealth = maxHealth;

            if (photonView.IsMine)
            {
                // Restaurar multiplicador
                movement.speedMultiplier = 1f;

                // Cambiar cámaras
                firstPersonCamera.SetActive(true);
                thirdPersonCamera.SetActive(false);
            }

            // Levantar el modelo
            playerModel.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
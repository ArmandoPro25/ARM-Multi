using UnityEngine;
using Photon.Pun;

public class FirstPersonCamera : MonoBehaviourPunCallbacks
{
    public Transform playerBody;
    public float mouseSensitivity = 300f;

    float xRotation = 0f;
    float yRotation = 0f;

    PhotonView pv;

    void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
    }

    void Start()
    {
        if (!pv.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yRotation = playerBody.eulerAngles.y;
    }

    void Update()
    {
        if (!pv.IsMine) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        // Rotación vertical (cámara)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (cuerpo)
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
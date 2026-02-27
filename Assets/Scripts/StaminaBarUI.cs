using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    public PlayerMovementFP playerMovement;
    private Image staminaImage;

    void Start()
    {
        staminaImage = GetComponent<Image>();
        if (staminaImage == null)
        {
            Debug.LogError("StaminaBarUI: No se encontró componente Image en este GameObject.");
        }
    }

    void Update()
    {
        if (playerMovement == null) return;
        staminaImage.fillAmount = playerMovement.SprintRatio;
    }
}
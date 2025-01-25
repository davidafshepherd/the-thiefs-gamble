using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSpellController : MonoBehaviour
{
    // Public variables for configuration
    public float duration = 5f; // Duration of speed boost in seconds
    public int cost = 10; // Cost of the spell in the shop
    public int cooldown = 20; // Cooldown time in seconds
    public float speedMultiplier = 1.15f; // 15% speed boost

    private bool isActive = false; // Tracks if the spell is currently active
    private bool isOnCooldown = false; // Tracks if the spell is on cooldown

    private ThirdPersonController playerController; // Reference to the player's movement controller
    private float originalMoveSpeed; // Store the player's original MoveSpeed
    private float originalSprintSpeed; // Store the player's original SprintSpeed

    public GameObject speedUI;
    public Image spellDurationImage; // Reference to the image icon for duration feedback

    private Text cooldownText; // Reference to the cooldown text component
    public AudioSource speedSource;
    public AudioClip speedClip;

    void Start()
    {
        // Find the player's controller
        playerController = GetComponent<ThirdPersonController>();
        if (playerController != null)
        {
            originalMoveSpeed = playerController.MoveSpeed;
            originalSprintSpeed = playerController.SprintSpeed;
        }
        else
        {
            Debug.LogWarning("Player does not have a ThirdPersonController component!");
        }

        // Get the cooldown text component from the speed UI
        if (speedUI != null)
        {
            Transform cooldownTransform = speedUI.transform.Find("Cooldown");
            if (cooldownTransform != null)
            {
                cooldownText = cooldownTransform.GetComponent<Text>();
                if (cooldownText != null)
                {
                    cooldownText.text = "Ready"; // Initial state
                }
            }

            // Ensure the spell duration image is fully filled at the start
            if (spellDurationImage != null)
            {
                spellDurationImage.fillAmount = 1; // Start fully filled
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ActivateSpell();
        }
        if (GameManager.Instance.getIsSpeedPurchased())
        {
            speedUI.SetActive(true);
        }
    }

    // Method called by the shop to purchase the spell
    public void PurchaseSpell()
    {
        if (GameManager.Instance.getIsSpeedPurchased())
        {
            Debug.Log("Spell already purchased.");
        }
        else if (GameManager.Instance.GetPlayerGold() >= cost)
        {
            GameManager.Instance.setSpeedPurchased(true);
            GameManager.Instance.AddPlayerGold(-cost);
            Debug.Log("Speed spell purchased!");
            speedUI.SetActive(true);
        }
        else
        {
            Debug.Log("Not enough gold to purchase the speed spell.");
        }
    }

    // Method to activate the spell (e.g., when the player uses it)
    public void ActivateSpell()
    {
        if (!GameManager.Instance.getIsSpeedPurchased())
        {
            Debug.Log("Spell not purchased. Cannot activate.");
            return;
        }

        if (!isActive && !isOnCooldown)
        {
            StartCoroutine(ActivateSpeedBoost());
        }
        else if (isOnCooldown)
        {
            Debug.Log("Spell is on cooldown.");
        }
        else
        {
            Debug.Log("Spell is already active.");
        }
    }

    // Coroutine to handle speed boost duration
    private System.Collections.IEnumerator ActivateSpeedBoost()
    {
        Debug.Log("Speed spell activated!");
        speedSource.PlayOneShot(speedClip);
        isActive = true;
        playerController.MoveSpeed = originalMoveSpeed * speedMultiplier;
        playerController.SprintSpeed = originalSprintSpeed * speedMultiplier;

        // Update the cooldown text to "Active"
        if (cooldownText != null)
        {
            cooldownText.text = "Active";
        }

        // Animate the spell duration UI
        float elapsedTime = 0f;
        if (spellDurationImage != null)
        {
            spellDurationImage.fillAmount = 1; // Fully visible initially
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (spellDurationImage != null)
            {
                spellDurationImage.fillAmount = 1 - (elapsedTime / duration);
            }
            yield return null;
        }

        if (spellDurationImage != null)
        {
            spellDurationImage.fillAmount = 1; // Reset to full after use
        }

        playerController.MoveSpeed = originalMoveSpeed;
        playerController.SprintSpeed = originalSprintSpeed;
        isActive = false;

        Debug.Log("Speed spell ended.");

        // Start cooldown
        StartCoroutine(HandleCooldown());
    }

    // Coroutine to handle cooldown
    private System.Collections.IEnumerator HandleCooldown()
    {
        isOnCooldown = true;
        float cooldownRemaining = cooldown;

        while (cooldownRemaining > 0)
        {
            if (cooldownText != null)
            {
                cooldownText.text = Mathf.CeilToInt(cooldownRemaining).ToString() + "s";
            }

            cooldownRemaining -= Time.deltaTime;
            yield return null;
        }

        if (cooldownText != null)
        {
            cooldownText.text = "Ready";
        }

        isOnCooldown = false;
    }

    // Method to check if the spell has been purchased (for shop UI)
    public bool IsPurchased()
    {
        return GameManager.Instance.getIsSpeedPurchased();
    }

    // Method to check if the spell is currently active
    public bool IsActive()
    {
        return isActive;
    }
}

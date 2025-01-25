using UnityEngine;
using UnityEngine.UI;

public class InvisibilitySpellController : MonoBehaviour
{
    // Public variables for configuration
    public float duration = 5f; // Duration of invisibility in seconds
    public int cost = 10; // Cost of the spell in the shop
    public int cooldown = 20; // Cooldown time in seconds

    private bool isActive = false; // Tracks if the spell is currently active
    private bool isOnCooldown = false; // Tracks if the spell is on cooldown

    private SkinnedMeshRenderer playerRenderer; // Reference to the player's Skinned Mesh Renderer
    private Color originalColor; // Store the player's original color/material

    public GameObject invisibilityUI;
    public Image spellDurationImage; // Reference to the image icon for duration feedback

    private Text cooldownText; // Reference to the cooldown text component
    public AudioSource spellAudioSource;
    public AudioClip invisibilitySound;

    void Start()
    {
        // Find the Skinned Mesh Renderer component from the children
        playerRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (playerRenderer != null)
        {
            // Clone the material to make it unique
            playerRenderer.material = new Material(playerRenderer.material);
            originalColor = playerRenderer.material.color;
        }
        else
        {
            Debug.LogWarning("Player does not have a Skinned Mesh Renderer component!");
        }

        // Get the cooldown text component from the invisibility UI
        if (invisibilityUI != null)
        {
            Transform cooldownTransform = invisibilityUI.transform.Find("Cooldown");
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateSpell();
        }
        if (GameManager.Instance.getIsInvisibilityPurchased())
        {
            invisibilityUI.SetActive(true);
        }
    }

    // Method called by the shop to purchase the spell
    public void PurchaseSpell()
    {
        if (GameManager.Instance.getIsInvisibilityPurchased())
        {
            Debug.Log("Spell already purchased.");
        }
        else if (GameManager.Instance.GetPlayerGold() >= cost)
        {
            GameManager.Instance.setInvisibilityPurchased(true);
            GameManager.Instance.AddPlayerGold(-cost);
            Debug.Log("Invisibility spell purchased!");
            invisibilityUI.SetActive(true);
        }
        else
        {
            Debug.Log("Not enough gold to purchase the invisibility spell.");
        }
    }

    // Method to activate the spell (e.g., when the player uses it)
    public void ActivateSpell()
    {
        if (!GameManager.Instance.getIsInvisibilityPurchased())
        {
            Debug.Log("Spell not purchased. Cannot activate.");
            return;
        }

        if (!isActive && !isOnCooldown)
        {
            StartCoroutine(ActivateInvisibility());
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

    // Coroutine to handle invisibility duration
    private System.Collections.IEnumerator ActivateInvisibility()
    {
        Debug.Log("Invisibility spell activated!");
        spellAudioSource.PlayOneShot(invisibilitySound);

        isActive = true;
        SetSurfaceType(true);
        SetInvisibility(true);

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

        SetInvisibility(false);
        SetSurfaceType(false);
        isActive = false;

        Debug.Log("Invisibility spell ended.");

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

    // Helper method to toggle invisibility
    private void SetInvisibility(bool invisible)
    {
        if (playerRenderer != null)
        {
            if (invisible)
            {
                Color transparentColor = originalColor;
                transparentColor.a = 0.2f; // Make player semi-transparent
                playerRenderer.material.color = transparentColor;
            }
            else
            {
                playerRenderer.material.color = originalColor; // Restore original color
            }
        }
    }

    // Helper method to dynamically change surface type
    private void SetSurfaceType(bool transparent)
    {
        if (playerRenderer != null)
        {
            Material material = playerRenderer.material;

            if (transparent)
            {
                material.SetFloat("_Surface", 1); // Transparent
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.renderQueue = 3000; // Transparent render queue
            }
            else
            {
                material.SetFloat("_Surface", 0); // Opaque
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.renderQueue = 2000; // Opaque render queue
            }
        }
    }

    // Method to check if the spell has been purchased (for shop UI)
    public bool IsPurchased()
    {
        return GameManager.Instance.getIsInvisibilityPurchased();
    }

    // Method to check if the spell is currently active
    public bool IsActive()
    {
        return isActive;
    }
}

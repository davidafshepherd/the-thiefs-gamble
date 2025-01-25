using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RevealSpellController : MonoBehaviour
{
    // Public variables for configuration
    public float duration = 5f; // Duration of the reveal effect in seconds
    public int cost = 10; // Cost of the spell in the shop
    public int cooldown = 20; // Cooldown time in seconds
    public float revealRadius = 15f; // Radius to detect enemies

    private bool isActive = false; // Tracks if the spell is currently active
    private bool isOnCooldown = false; // Tracks if the spell is on cooldown

    public GameObject revealUI;
    public Image spellDurationImage; // Reference to the image icon for duration feedback

    private Text cooldownText; // Reference to the cooldown text component
    private List<GameObject> outlinedObjects = new List<GameObject>(); // List to track enabled outlines

    public AudioSource revealSource;
    public AudioClip revealClip;

    void Start()
    {
        // Get the cooldown text component from the reveal UI
        if (revealUI != null)
        {
            Transform cooldownTransform = revealUI.transform.Find("Cooldown");
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            ActivateSpell();
        }
        if (GameManager.Instance.getIsRevealPurchased())
        {
            revealUI.SetActive(true);
        }
    }

    // Method called by the shop to purchase the spell
    public void PurchaseSpell()
    {
        if (GameManager.Instance.getIsRevealPurchased())
        {
            Debug.Log("Spell already purchased.");
        }
        else if (GameManager.Instance.GetPlayerGold() >= cost)
        {
            GameManager.Instance.setRevealPurchased(true);
            GameManager.Instance.AddPlayerGold(-cost);
            Debug.Log("Reveal spell purchased!");
            revealUI.SetActive(true);
        }
        else
        {
            Debug.Log("Not enough gold to purchase the reveal spell.");
        }
    }

    // Method to activate the spell (e.g., when the player uses it)
    public void ActivateSpell()
    {
        if (!GameManager.Instance.getIsRevealPurchased())
        {
            Debug.Log("Spell not purchased. Cannot activate.");
            return;
        }

        if (!isActive && !isOnCooldown)
        {
            StartCoroutine(ActivateReveal());
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

    // Coroutine to handle reveal effect duration
    private IEnumerator ActivateReveal()
    {
        Debug.Log("Reveal spell activated!");
        revealSource.PlayOneShot(revealClip);
        isActive = true;
        outlinedObjects.Clear(); // Clear the list of previously outlined objects

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

            // Find all enemies in the reveal radius
            Collider[] colliders = Physics.OverlapSphere(transform.position, revealRadius);
            foreach (Collider collider in colliders)
            {
                ThiefController thief = collider.GetComponent<ThiefController>();
                if (thief != null)
                {
                    Outline outline = thief.GetComponent<Outline>();
                    if (outline != null && !outline.enabled)
                    {
                        outline.enabled = true; // Enable the outline effect
                        if (!outlinedObjects.Contains(thief.gameObject))
                        {
                            outlinedObjects.Add(thief.gameObject); // Add to the list
                        }
                    }
                }
            }

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

        // Disable all outlines at the end of the spell
        foreach (GameObject outlinedObject in outlinedObjects)
        {
            Outline outline = outlinedObject.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }

        outlinedObjects.Clear(); // Clear the list
        isActive = false;

        Debug.Log("Reveal spell ended.");

        // Start cooldown
        StartCoroutine(HandleCooldown());
    }

    // Coroutine to handle cooldown
    private IEnumerator HandleCooldown()
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
        return GameManager.Instance.getIsRevealPurchased();
    }

    // Method to check if the spell is currently active
    public bool IsActive()
    {
        return isActive;
    }
}

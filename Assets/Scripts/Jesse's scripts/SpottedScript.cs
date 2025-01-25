using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpottedScript : MonoBehaviour
{
    public float spottedIntensity = 0.4f; // Maximum intensity when spotted
    public float fadeOutSpeed = 0.5f;    // Speed of fade-out effect
    public float holdTime = 2.0f;
    public TextMeshProUGUI spottedText;
    private Vignette vignette;           // Reference to the Vignette effect
    private Volume volume;               // Reference to the URP Volume

    private int activeThiefCount = 0;    // Number of thieves currently spotting the player
    private Coroutine fadeOutCoroutine;  // Reference to the fade-out coroutine
    private bool isPlayerCaught = false;

    void Start()
    {
        // Find the Volume on this object or in the scene
        volume = GetComponent<Volume>();

        // Ensure the Volume has the Vignette override
        if (volume != null && volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f; // Start with no vignette
        }
        else
        {
            Debug.LogError("Vignette not found on the Volume! Make sure it's added.");
        }

        if (spottedText != null)
        {
            Color textColor = spottedText.color;
            textColor.a = 0f;
            spottedText.color = textColor;
        }
    }

    public void AddThiefDetection()
    {
        if (!isPlayerCaught)
        {
            activeThiefCount++;

            // Start the spotted effect if it's not already active
            if (activeThiefCount == 1)
            {
                if (fadeOutCoroutine != null)
                {
                    StopCoroutine(fadeOutCoroutine);
                }
                StartSpottedEffect();
            }
        }
    }
    public void LockAddThiefDetection()
    {
        isPlayerCaught = true;
    }
    public void UnlockAddThiefDetection()
    {
        isPlayerCaught=false;
    }

    public void RemoveThiefDetection()
    {
        activeThiefCount = Mathf.Max(0, activeThiefCount - 1);

        // Stop the spotted effect if no thieves are detecting the player
        if (activeThiefCount == 0)
        {
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
            }
            fadeOutCoroutine = StartCoroutine(FadeOutEffect());
        }
    }

    private void StartSpottedEffect()
    {
        if (vignette == null || spottedText == null)
            return;

        // Set the vignette to the maximum intensity
        vignette.intensity.value = spottedIntensity;

        // Set the text alpha to fully visible
        Color textColor = spottedText.color;
        textColor.a = 1f;
        spottedText.color = textColor;
    }

    private IEnumerator FadeOutEffect()
    {
        if (vignette == null || spottedText == null)
            yield break;

        // Gradually fade out the vignette and text
        while (vignette.intensity.value > 0f || spottedText.color.a > 0f)
        {
            if (vignette.intensity.value > 0f)
            {
                vignette.intensity.value -= fadeOutSpeed * Time.deltaTime;
            }

            if (spottedText.color.a > 0f)
            {
                Color textColor = spottedText.color;
                textColor.a -= fadeOutSpeed * Time.deltaTime;
                spottedText.color = textColor;
            }

            yield return null;
        }

        // Ensure the vignette intensity and text alpha are exactly 0
        vignette.intensity.value = 0f;
        Color finalTextColor = spottedText.color;
        finalTextColor.a = 0f;
        spottedText.color = finalTextColor;
    }

    // New method to stop the effect immediately
    public void StopEffect()
    {
        // Reset the vignette intensity and text alpha
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0f;
        }

        if (spottedText != null)
        {
            Color textColor = spottedText.color;
            textColor.a = 0f;
            spottedText.color = textColor;
        }

        activeThiefCount = 0; // Reset active thief count
    }
}

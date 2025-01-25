using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CaughtScreenEffect : MonoBehaviour
{
    public float caughtIntensity = 1f; // Maximum intensity when caught
    public float effectDuration = 5.0f; // Duration for how long the effect stays
    public TextMeshProUGUI caughtText;  // Text displayed when caught
    private Vignette vignette;          // Reference to the Vignette effect
    private DepthOfField depthOfField;
    private Volume volume;              // Reference to the URP Volume

    private bool isEffectActive = false; // Tracks if the effect is already active
    private string customMessage = "Caught!"; // Default message for the caught screen

    public GameObject player; // Reference to the player GameObject
    private ThirdPersonController thirdPersonController; // Reference to the ThirdPersonController component

    void Start()
    {
        // Find the Volume on this object or in the scene
        volume = GetComponent<Volume>();

        if (volume != null)
        {
            if (volume.profile.TryGet(out vignette) &&
                volume.profile.TryGet(out depthOfField))
            {
                volume.weight = 0f;
                vignette.intensity.value = 0f; // No vignette by default
                depthOfField.focusDistance.value = 10f; // Start with no blur
            }
        }

        if (caughtText != null)
        {
            Color textColor = caughtText.color;
            textColor.a = 0f; // Start with text hidden
            caughtText.color = textColor;
        }

        // Find and store the ThirdPersonController component
        if (player != null)
        {
            thirdPersonController = player.GetComponent<ThirdPersonController>();
        }
    }

    public void SetCustomMessage(string message)
    {
        customMessage = message; // Update the message displayed on the caught screen
    }

    public void TriggerCaughtEffect()
    {
        if (!isEffectActive) // Ensure the effect is triggered only once
        {
            isEffectActive = true;

            // Disable the ThirdPersonController to lock player movement
            if (thirdPersonController != null)
            {
                thirdPersonController.enabled = false;
            }

            StartCoroutine(CaughtEffectCoroutine());
        }
    }

    private IEnumerator CaughtEffectCoroutine()
    {
        if (vignette == null || caughtText == null || volume == null)
            yield break;

        // Set vignette intensity and initialize blur
        volume.weight = 1f;
        vignette.intensity.value = caughtIntensity;

        // Set the text alpha to fully visible and customize text appearance
        Color textColor = caughtText.color;
        textColor.a = 1f; // Fully visible
        caughtText.color = textColor;

        // Optionally set custom text for the caught message
        caughtText.text = customMessage; // Make sure `customMessage` is defined and set elsewhere

        if (volume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            depthOfField.active = true; // Enable Depth of Field
            float initialBlur = 0f; // No blur at start
            float maxBlur = 10f; // Adjust based on desired intensity

            // Gradually increase the blur over effectDuration
            float elapsedTime = 0f;
            while (elapsedTime < effectDuration)
            {
                float t = elapsedTime / effectDuration;

                // Increase blur intensity
                if (depthOfField.gaussianStart.overrideState) // If using Gaussian
                {
                    depthOfField.gaussianStart.value = Mathf.Lerp(initialBlur, maxBlur, t);
                    depthOfField.gaussianEnd.value = Mathf.Lerp(initialBlur, maxBlur + 5f, t); // End range slightly larger
                }

                if (depthOfField.focusDistance.overrideState) // If using Bokeh
                {
                    depthOfField.focusDistance.value = Mathf.Lerp(10f, 1f, t); // Bring the blur closer over time
                    depthOfField.aperture.value = Mathf.Lerp(16f, 4f, t); // Increase aperture for stronger blur
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        // Wait for the effect duration
        yield return new WaitForSeconds(effectDuration);

        // Fade out vignette, blur, and caughtText
        while (vignette.intensity.value > 0f || caughtText.color.a > 0f)
        {
            if (vignette.intensity.value > 0f)
            {
                vignette.intensity.value = Mathf.Max(0f, vignette.intensity.value - Time.deltaTime); // Ensure it doesn't go below 0
            }

            if (caughtText.color.a > 0f)
            {
                textColor.a = Mathf.Max(0f, textColor.a - Time.deltaTime); // Ensure it doesn't go below 0
                caughtText.color = textColor;
            }

            yield return null;
        }

        // Reset Depth of Field and vignette
        if (volume.profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.active = false; // Disable Depth of Field
        }
        vignette.intensity.value = 0f;
        caughtText.color = new Color(caughtText.color.r, caughtText.color.g, caughtText.color.b, 0f);
        volume.weight = 0f;

        // Re-enable the ThirdPersonController to restore player movement
        if (thirdPersonController != null)
        {
            thirdPersonController.enabled = true;
        }

        isEffectActive = false; // Reset effect state
    }
}

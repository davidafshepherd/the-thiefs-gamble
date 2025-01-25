using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DayNightManager : MonoBehaviour
{
    public Light directionalLight; // Assign the sun/moon Directional Light
    public Material daySkyboxMaterial; // Assign your Skybox/Procedural material
    public Material nightSkyboxMaterial;

    [Header("Lighting Settings")]
    public Color dayLightColor = Color.white; // Light color for day
    public Color nightLightColor = new Color(0.1f, 0.1f, 0.5f); // Light color for night
    public float dayLightIntensity = 1f; // Intensity for day
    public float nightLightIntensity = 0.3f; // Intensity for night

    [Header("Skybox Settings")]
    public Color daySkyTint = new Color(0.5f, 0.7f, 1f); // Sky tint for day
    public Color nightSkyTint = new Color(0f, 0f, 0.1f); // Sky tint for night
    public Color dayGroundColor = new Color(0.8f, 0.8f, 0.8f); // Ground color for day
    public Color nightGroundColor = new Color(0.05f, 0.05f, 0.1f); // Ground color for night

    [Header("Fog Settings")]
    public bool enableFog = true; // Enable/Disable fog
    public Color dayFogColor = new Color(0.8f, 0.8f, 0.8f, 0.5f); // Fog color for day
    public Color nightFogColor = new Color(0f, 0f, 0.1f, 0.5f); // Fog color for night
    public float dayFogDensity = 0.01f; // Fog density for day
    public float nightFogDensity = 0.05f; // Fog density for night

    [Header("Post-Processing")]
    public Volume postProcessingVolume; // Reference to the URP Post-Processing Volume
    private ColorAdjustments colorAdjustments; // Color grading settings
    private Vignette vignette; // Vignette effect for night

    private void Start()
    {
        // Initialize post-processing effects
        if (postProcessingVolume != null)
        {
            if (postProcessingVolume.profile.TryGet(out colorAdjustments) &&
                postProcessingVolume.profile.TryGet(out vignette))
            {
                vignette.intensity.value = 0f; // No vignette by default
            }
        }
    }

    public void SetDay(float transitionSpeed)
    {
        if (transitionSpeed <= 0f)
        {
            ApplyDaySettings();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(TransitionToDay(transitionSpeed));
        }
    }

    public void SetNight(float transitionSpeed)
    {
        if (transitionSpeed <= 0f)
        {
            ApplyNightSettings();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(TransitionToNight(transitionSpeed));
        }
    }

    private IEnumerator TransitionToDay(float duration)
    {
        GameManager.Instance.isDay = true;
        float timeElapsed = 0f;
        Material currentSkybox = RenderSettings.skybox;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            // Interpolate lighting
            directionalLight.color = Color.Lerp(nightLightColor, dayLightColor, t);
            directionalLight.intensity = Mathf.Lerp(nightLightIntensity, dayLightIntensity, t);

            // Interpolate fog
            if (enableFog)
            {
                RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, t);
                RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, t);
            }

            // Interpolate post-processing
            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(-50f, 0f, t);
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(0.4f, 0f, t);
            }

            // Fade the skybox color to black before switching
            if (currentSkybox != daySkyboxMaterial)
            {
                currentSkybox.SetColor("_SkyTint", Color.Lerp(nightSkyTint, Color.black, t));
                currentSkybox.SetColor("_GroundColor", Color.Lerp(nightGroundColor, Color.black, t));
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        ApplyDaySettings();
    }

    private IEnumerator TransitionToNight(float duration)
    {
        GameManager.Instance.isDay = false;
        float timeElapsed = 0f;
        Material currentSkybox = RenderSettings.skybox;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            // Interpolate lighting
            directionalLight.color = Color.Lerp(dayLightColor, nightLightColor, t);
            directionalLight.intensity = Mathf.Lerp(dayLightIntensity, nightLightIntensity, t);

            // Interpolate fog
            if (enableFog)
            {
                RenderSettings.fogColor = Color.Lerp(dayFogColor, nightFogColor, t);
                RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity, nightFogDensity, t);
            }

            // Interpolate post-processing
            if (colorAdjustments != null)
            {
                colorAdjustments.saturation.value = Mathf.Lerp(0f, -50f, t);
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(0f, 0.4f, t);
            }

            // Fade the skybox color to black before switching
            if (currentSkybox != nightSkyboxMaterial)
            {
                currentSkybox.SetColor("_SkyTint", Color.Lerp(daySkyTint, Color.black, t));
                currentSkybox.SetColor("_GroundColor", Color.Lerp(dayGroundColor, Color.black, t));
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        ApplyNightSettings();
    }

    private void ApplyDaySettings()
    {
        // Apply instant day settings
        directionalLight.color = dayLightColor;
        directionalLight.intensity = dayLightIntensity;

        RenderSettings.skybox = daySkyboxMaterial;
        daySkyboxMaterial.SetColor("_SkyTint", daySkyTint);
        daySkyboxMaterial.SetColor("_GroundColor", dayGroundColor);

        if (enableFog)
        {
            RenderSettings.fogColor = dayFogColor;
            RenderSettings.fogDensity = dayFogDensity;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = 0f;
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0f;
        }
    }

    private void ApplyNightSettings()
    {
        // Apply instant night settings
        directionalLight.color = nightLightColor;
        directionalLight.intensity = nightLightIntensity;

        RenderSettings.skybox = nightSkyboxMaterial;
        nightSkyboxMaterial.SetColor("_SkyTint", nightSkyTint);
        nightSkyboxMaterial.SetColor("_GroundColor", nightGroundColor);

        if (enableFog)
        {
            RenderSettings.fogColor = nightFogColor;
            RenderSettings.fogDensity = nightFogDensity;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = -50f;
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0.4f;
        }
    }
}

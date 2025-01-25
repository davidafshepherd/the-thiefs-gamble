using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("Image Settings")]
    public Image fadeImage;                                                 // Reference to the black screen image
    public float fadeDuration = 3f;                                         // Fade duration

    [Header("Text Settings")]
    public Text moralText;                                                  // Reference to the text for storing the moral
    public string firstMoralHalf = "Enemies are made by injury \n";         // First half of the moral of the fable
    public string secondMoralHalf = "not kindness.";                        // Second half of the moral of the fable
    public string moral = "Enemies are made by injury not kindness.";       // Moral of the fable 

    [Header("Audio Settings")]
    private AudioSource audioSource;                                        // Audio source to play the background music

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Handle ending scene
        StartCoroutine(HandleEnding());
    }

    private IEnumerator HandleEnding()
    {
        // Fade in
        StartCoroutine(FadeMusic(fadeDuration, true));
        yield return StartCoroutine(Fade(fadeDuration, 0));
        yield return new WaitForSeconds(5f);

        // Fade out
        StartCoroutine(FadeMusic(fadeDuration, false));
        yield return StartCoroutine(Fade(fadeDuration, 1));
        yield return new WaitForSeconds(2f);

        // Display first half of the moral
        yield return StartCoroutine(TypeMoral(false));
        yield return new WaitForSeconds(1.5f);

        // Display rest of the moral
        yield return StartCoroutine(AppendMoral());
        yield return new WaitForSeconds(2f);

        // Undisplay text
        yield return StartCoroutine(TypeMoral(true));
        yield return new WaitForSeconds(3f);

        // Load menu
        SceneManager.LoadScene(0);
    }

    private IEnumerator Fade(float fadeDuration, float targetAlpha)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;
        
        float time = 0;
        while (time < fadeDuration)
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = color;

            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeMusic(float fadeDuration, bool fadeIn)
    {
        float startVolume = fadeIn ? 0f : audioSource.volume;
        float targetVolume = fadeIn ? audioSource.volume : 0f;

        if (fadeIn) audioSource.volume = 0f;                        // Ensure starting volume is 0 for fade-in
        if (fadeIn && !audioSource.isPlaying) audioSource.Play();   // Start playing if not already

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (!fadeIn) audioSource.Stop();                            // Stop the audio if fading out
    }

    private IEnumerator TypeMoral(bool reverse)
    {
        moralText.text = "";
        if (reverse)
        {
            for (int i = moral.Length; i >= 0; i--)
            {
                moralText.text = moral.Substring(0, i);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            for (int i = 0; i <= firstMoralHalf.Length; i++)
            {
                moralText.text = firstMoralHalf.Substring(0, i);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator AppendMoral()
    {
        string currentText = moralText.text;

        for (int i = 1; i <= secondMoralHalf.Length; i++)
        {
            moralText.text = currentText + secondMoralHalf.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

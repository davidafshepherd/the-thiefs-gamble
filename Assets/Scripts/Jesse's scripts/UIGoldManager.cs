using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGoldManager : MonoBehaviour
{
    public GameObject goldChangeUI;
    private Text goldChangeText;            // Reference to the UI Text element
    private int playerGold;
    
    public float animationDuration = 1.5f;  // Duration of the animation
    public float fadeOutSpeed = 2f;         // Speed of fade-out effect

    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        goldChangeText = goldChangeUI.GetComponent<Text>();

        // Store the original color of the text
        if (goldChangeText != null) originalColor = goldChangeText.color;

        // Display current gold amount
        Text goldText = GetComponent<Text>();
        goldText.text = GameManager.Instance.GetPlayerGold().ToString();
    }

    // Update is called once per frame
    public void Update()
    {
        // Cache the Text component
        Text goldText = GetComponent<Text>();

        // Update only when the value changes
        if (int.Parse(goldText.text) != GameManager.Instance.GetPlayerGold())
        {
            int goldDifference = GameManager.Instance.GetPlayerGold() - int.Parse(goldText.text);   // Difference in gold amount
            goldText.text = GameManager.Instance.GetPlayerGold().ToString();                        // Display new gold amount
            DisplayGoldChange(goldDifference);                                                      // Display difference in gold amount
        }
    }

    public void DisplayGoldChange(int goldAmount)
    {
        if (goldChangeText == null) return;

        // Set the text to show the gold change
        goldChangeText.text = (goldAmount > 0 ? "+" : "") + goldAmount.ToString();

        // Set the initial position and reset alpha
        goldChangeText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        goldChangeText.transform.localScale = Vector3.one;

        // Start the animation
        StartCoroutine(AnimateGoldChange());
    }

    private System.Collections.IEnumerator AnimateGoldChange()
    {
        float elapsedTime = 0f;

        // Animation loop
        while (elapsedTime < animationDuration)
        {
            // Move text upwards
            goldChangeText.transform.localPosition += new Vector3(0, 1f * Time.deltaTime * 50f, 0);

            // Gradually fade out
            Color color = goldChangeText.color;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / animationDuration);
            goldChangeText.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure fully transparent at the end
        Color finalColor = goldChangeText.color;
        finalColor.a = 0f;
        goldChangeText.color = finalColor;
    }
}

using UnityEngine;
using TMPro;

public class UpdateMoralityUI : MonoBehaviour
{
    private TextMeshProUGUI moralityText; // Reference to the TextMeshPro UI component

    private void Start()
    {
        // Get the TextMeshPro UI component
        moralityText = GetComponent<TextMeshProUGUI>();
        if (moralityText == null)
        {
            Debug.LogError("No TextMeshProUGUI component found on this GameObject!");
            return;
        }

        // Update the UI initially
        UpdateMoralityText();
    }

    private void Update()
    {
        // Continuously update the morality text
        UpdateMoralityText();
    }

    private void UpdateMoralityText()
    {
        // Get morality alignment from the GameManager
        string moralityAlignment = GameManager.Instance.GetMoralityAlignment();

        // Update the text
        moralityText.text = moralityAlignment;

        // Change text color based on alignment
        switch (moralityAlignment)
        {
            case "Virtuous":
                moralityText.color = Color.green; // Green for Virtuous
                break;

            case "Neutral":
                moralityText.color = Color.grey; // Grey for Neutral
                break;

            case "Deceptive":
                moralityText.color = Color.red; // Red for Deceptive
                break;

            default:
                moralityText.color = Color.white; // Fallback color
                Debug.LogWarning("Unknown morality alignment: " + moralityAlignment);
                break;
        }
    }
}

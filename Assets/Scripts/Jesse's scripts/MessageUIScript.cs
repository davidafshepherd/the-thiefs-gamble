using UnityEngine;
using UnityEngine.UI;

public class MessageUIScript : MonoBehaviour
{
    // Static instance for global access
    public static MessageUIScript Instance { get; private set; }

    private Text messageText; // Cached Text component

    private void Awake()
    {
        // Singleton pattern to ensure a single instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Cache the Text component
        messageText = GetComponent<Text>();

        SetMessage(string.Empty);

    }

    /// <summary>
    /// Set the message text to display.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }
}

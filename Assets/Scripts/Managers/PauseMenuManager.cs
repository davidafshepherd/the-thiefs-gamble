using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuUI; // Reference to the pause screen UI GameObject

    private bool isPaused = false; // Tracks if the game is currently paused
    public ThirdPersonController playerController;

    private void Update()
    {
        // Toggle pause menu when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    /// <summary>
    /// Pauses the game and shows the pause menu.
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze game time
        playerController.enabled = false;
        pauseMenuUI.SetActive(true); // Show the pause menu UI
        Cursor.visible = true; // Show the cursor
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
    }

    /// <summary>
    /// Resumes the game and hides the pause menu.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        playerController.enabled=true;
        Time.timeScale = 1f; // Resume game time
        pauseMenuUI.SetActive(false); // Hide the pause menu UI
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
    }

    /// <summary>
    /// Quits the application. Works only in a built version of the game.
    /// </summary>
    public void QuitGame()
    {

        Debug.Log("Quitting to main menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Resets the game to the last checkpoint.
    /// </summary>
    public void ResetToCheckpoint()
    {
        Debug.Log("Resetting to last checkpoint...");
        // Implement logic for resetting to the last checkpoint
        // Example: Call a method from your checkpoint manager
        GameManager.Instance.ResetToLastSave();
        ResumeGame(); // Resume the game after resetting
    }
}

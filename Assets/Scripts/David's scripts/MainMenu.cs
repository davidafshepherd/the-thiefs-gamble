using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        // Enable cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Destroy game manager, if there is one
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
            GameManager.Instance = null;
        }
    }

    public void Play()
    {
        // Load game
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        // Quit game
        Application.Quit();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;        // Reference to the player
    public Vector3 lastLocation;    // Location of the player in the current scene
    public Vector3 lastRotation;    // Rotation of the player in the current scene

    [Header("Index Settings")]
    public int doorIndex;           // Index of the door that this script is attached to
    public int sceneIndex;          // Index of the scene that the door takes the player to

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.IsDoorOpen(doorIndex) && other.transform == player)
        {
            // Save the player's position and rotation
            GameManager.Instance.SetLocation(lastLocation);
            GameManager.Instance.SetRotation(lastRotation);

            // Change room
            GameManager.Instance.ChangeRoom(sceneIndex-1);
            SceneManager.LoadScene(sceneIndex);
        }
    }
}

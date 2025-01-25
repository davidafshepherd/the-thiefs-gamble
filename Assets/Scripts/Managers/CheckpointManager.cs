using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject[] checkpoints;        // Array of checkpoints
    public AudioClip checkpoint;            // Audio clip of a checkpoint being reached
    public AudioClip playerSpeech;          // Audio clip of 
    private AudioSource audioSource;        // AudioSource to play a checkpoint reached

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Store the current room
        int currentRoom = GameManager.Instance.GetCurrentRoom();

        // Activate checkpoint i, if the checkpoint has been activated but not reached
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (currentRoom == 1 && GameManager.Instance.IsCheckpointActivated(i) && !GameManager.Instance.IsCheckpointReached(i))
            {
                ActivateCheckpoint(i);
            }
        }
    }

    public void ActivateCheckpoint(int checkpointIndex)
    {
        checkpoints[checkpointIndex].SetActive(true);
        GameManager.Instance.ActivateCheckpoint(checkpointIndex);
    }

    public void DeactivateCheckpoint(int checkpointIndex)
    {
        checkpoints[checkpointIndex].SetActive(false);
        GameManager.Instance.ReachCheckpoint(checkpointIndex);
        audioSource.PlayOneShot(checkpoint);
    }

    public void PlayPlayerSpeech()
    {
        audioSource.PlayOneShot(playerSpeech);
    }
}

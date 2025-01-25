using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    public Transform player;        // Reference to the player
    public int saveIndex;           // Save index

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GetNextSave() == saveIndex && other.transform == player) {
            // Store player's save location and rotation
            GameManager.Instance.SetSaveLocation(transform.position);
            GameManager.Instance.SetSaveRotation(transform.eulerAngles);

            // Save game and increment next save
            GameManager.Instance.SaveGame(); 
            GameManager.Instance.IncrementNextSave();
        }
    }
}

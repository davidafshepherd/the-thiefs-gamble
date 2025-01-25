using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;

public class CrystalController : MonoBehaviour
{
    [Header("Managers")]
    public Transform instructionManager;                                    // Reference to the instruction manager
    public Transform doorManager;                                           // Reference to the door manager

    [Header("Interaction Settings")]
    public Transform player;                                                // Reference to the player
    private bool isPlayerNearby = false;                                    // Tracks if the player is near the crystal

    [Header("InteractUI Settings")]
    public GameObject interactUI;                                           // Reference to the interact UI
    public string interactMessage = "Press E to Steal Crystal";             // Interact message to display in the interact UI

    [Header("Steal Settings")]
    public GameObject crossHair;                                            // Reference to the player's crosshair
    public Transform guard1;                                                // Reference to the queen's 1st guard
    public Transform guard2;                                                // Reference to the queen's 2nd guard
    public Transform queen;                                                 // Reference to the queen

    void Update()
    {
        // If the player is nearby
        if (isPlayerNearby)
        {
            // Handle stealing
            if (Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.IsTaskComplete(1)) Steal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            // Set the player to nearby
            isPlayerNearby = true;

            // Store game states
            int currentRoom = GameManager.Instance.GetCurrentRoom();
            bool secondTask = GameManager.Instance.IsTaskComplete(1);

            // Set the interact message
            if (currentRoom == 2 && !secondTask)
            { interactUI.GetComponent<TextMeshProUGUI>().text = interactMessage; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            // Set the player to not nearby
            isPlayerNearby = false;

            // Reset the interact message
            interactUI.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void Steal()
    {
        // Complete the 2nd task
        GameManager.Instance.CompleteTask(1);

        // Hide the steal crystal instruction and reset the interact message
        instructionManager.GetComponent<InstructionManager>().HideCrystalInstruction();
        interactUI.GetComponent<TextMeshProUGUI>().text = "";

        // Disable the player controller and disable the crosshair
        player.GetComponent<ThirdPersonController>().DisableController();
        crossHair.SetActive(false);

        // Activate guards
        guard1.gameObject.SetActive(true);
        guard2.gameObject.SetActive(true);

        // Make the guards face the player
        StartCoroutine(FacePlayer(guard1, 0.75f));
        StartCoroutine(FacePlayer(guard2, 0.75f));

        // Activate queen and make the queen face the player
        queen.gameObject.SetActive(true);
        StartCoroutine(FacePlayer(queen, 0.75f));

        // Begin the 4th interaction
        queen.GetComponent<QueenController>().SecondInteraction();
    }

    private IEnumerator FacePlayer(Transform transform, float faceDuration)
    {
        Quaternion startRotation = transform.rotation;                                                      // Start rotation
        Vector3 direction = (player.position - transform.position).normalized;                              // Directon to player
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));      // Target rotation

        // Time elapsed
        float timeElapsed = 0f;

        while (timeElapsed < faceDuration)
        {
            // Interpolate rotation towards target rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / faceDuration);

            timeElapsed += Time.deltaTime;      // Increment elapsed time
            yield return null;                  // Wait for the next frame
        }
    }
}

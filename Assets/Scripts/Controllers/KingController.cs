using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;
using TMPro;

public class KingController : MonoBehaviour
{
    [Header("Managers")]
    public Transform doorManager;                                   // Reference to the door manager
    public Transform instructionManager;                            // Reference to the instruction manager

    [Header("Interaction Settings")]
    public Transform player;                                        // Reference to the player
    public float rotationSpeed = 3f;                                // King's rotation speed towards the player
    private bool isPlayerNearby = false;                            // Tracks if the player is near the king
    private bool isInteracting = false;                             // Tracks if the king is currently interacting with the player

    [Header("InteractUI Settings")]
    public GameObject interactUI;                                   // Reference to the interact UI
    public string interactMessage = "Press E to Talk to King";      // Interact message to display in the interact UI

    [Header("Conversation Settings")]
    public NPCConversation firstKingConversation;                   // King's first conversation with the player
    public NPCConversation secondKingConversation;                  // King's second conversation with the player
    public NPCConversation alternateSecondKingConversation;         // King's second conversation with the player if the player was caught

    [Header("Cursor settings")]
    public GameObject crossHair;                                    // Reference to the player's cross hair

    private void Update()
    {
        // If the player is nearby
        if (isPlayerNearby) 
        {
            // Make the queen face the player
            FacePlayer();

            // Handle interaction
            if (Input.GetKeyDown(KeyCode.E) && !isInteracting) HandleInteraction();
        }
    }

    private void FacePlayer()
    {
        // Calculate the direction to the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Look at the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            // Set the player to nearby
            isPlayerNearby = true;

            // Store game states
            int currentRoom = GameManager.Instance.GetCurrentRoom();
            bool firstInteraction = GameManager.Instance.IsInteractionComplete(0);
            bool firstTask = GameManager.Instance.IsTaskComplete(0);
            bool secondInteraction = GameManager.Instance.IsInteractionComplete(1);

            // Set the interact message
            if (currentRoom == 0 && !firstInteraction)
            { interactUI.GetComponent<TextMeshProUGUI>().text = interactMessage; }
            else if (currentRoom == 0 && firstInteraction && firstTask && !secondInteraction)
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

    private void HandleInteraction()
    {
        // Store game states
        int currentRoom = GameManager.Instance.GetCurrentRoom();
        bool firstInteraction = GameManager.Instance.IsInteractionComplete(0);
        bool firstTask = GameManager.Instance.IsTaskComplete(0);
        bool secondInteraction = GameManager.Instance.IsInteractionComplete(1);

        // Handle interactions
        if (currentRoom == 0 && !firstInteraction)
        {
            isInteracting = true;
            FirstInteraction();
        }
        else if (currentRoom == 0 && firstInteraction && firstTask && !secondInteraction)
        {
            isInteracting = true;
            SecondInteraction();
        }
    }

    private void FirstInteraction()
    {
        // Begin the first interaction and reset the interact message
        GameManager.Instance.BeginInteraction(0);
        interactUI.GetComponent<TextMeshProUGUI>().text = "";

        // Disable the player controller and unlock the cursor
        player.GetComponent<ThirdPersonController>().DisableController();
        Cursor.lockState = CursorLockMode.None;

        // Disable the player's cross hair and enable their cursor
        crossHair.SetActive(false);
        Cursor.visible = true;

        // Hide the approach instruction and start the king's first conversation with the player
        instructionManager.GetComponent<InstructionManager>().HideApproachInstruction();
        ConversationManager.Instance.StartConversation(firstKingConversation);

        // Make the player face the king
        StartCoroutine(player.GetComponent<ThirdPersonController>().FaceNPC(transform, 1));
        StartCoroutine(player.GetComponent<ThirdPersonController>().PanToNPC(transform, 1));
    }

    public void EndFirstInteraction()
    {
        // Enable the player controller and lock the cursor
        player.GetComponent<ThirdPersonController>().EnableController();
        Cursor.lockState = CursorLockMode.Locked;

        // Disable the player's cursor and enable their cross hair
        Cursor.visible = false;
        crossHair.SetActive(true);

        // Open the exit, display the exit instruction and begin the first task 
        doorManager.GetComponent<DoorManager>().OpenMeetingRoomDoor();
        instructionManager.GetComponent<InstructionManager>().ShowExitInstruction();
        GameManager.Instance.BeginTask(0);

        // Complete the first interaction and turn off interaction mode
        GameManager.Instance.CompleteInteraction(0);
        isInteracting = false;
    }

    private void SecondInteraction()
    {
        // Begin the second interaction and reset the interact message
        GameManager.Instance.BeginInteraction(1);
        interactUI.GetComponent<TextMeshProUGUI>().text = "";

        // Disable the player controller and unlock the cursor
        player.GetComponent<ThirdPersonController>().DisableController();
        Cursor.lockState = CursorLockMode.None;

        // Disable the player's cross hair and enable their cursor
        crossHair.SetActive(false);
        Cursor.visible = true;

        // Hide the approach instruction
        instructionManager.GetComponent<InstructionManager>().HideApproachInstruction();

        //Start the king's second conversation with the player
        if (GameManager.Instance.getAct1CaughtCounter() < 1)
        { ConversationManager.Instance.StartConversation(secondKingConversation); }
        else { ConversationManager.Instance.StartConversation(alternateSecondKingConversation); }

        // Make the player face the king
        StartCoroutine(player.GetComponent<ThirdPersonController>().FaceNPC(transform, 1));
        StartCoroutine(player.GetComponent<ThirdPersonController>().PanToNPC(transform, 1));
    }

    public void MakeGoldChoice(bool choice) 
    {
        GameManager.Instance.MakeChoice(0, choice);
    }

    public void ReceiveGold(int gold)
    {
        GameManager.Instance.RemovePlayerGold(gold);
    }

    public void GiveGold(int gold)
    {
        GameManager.Instance.AddPlayerGold(gold);
    }

    public void AdjustMoralityScore(int value)
    {
        GameManager.Instance.AdjustMoralityScore(value);
    }

    public void EndSecondInteraction()
    {
        // Enable the player controller and lock the cursor
        player.GetComponent<ThirdPersonController>().EnableController();
        Cursor.lockState = CursorLockMode.Locked;

        // Disable the player's cursor and enable their cross hair
        Cursor.visible = false;
        crossHair.SetActive(true);

        // Display the exit instruction and begin next tasks
        instructionManager.GetComponent<InstructionManager>().ShowExitInstruction();
        GameManager.Instance.BeginTask(1);

        // Complete the second interaction and turn off interaction mode
        GameManager.Instance.CompleteInteraction(1);
        isInteracting = false;
    }
}
using DialogueEditor;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QueenController : MonoBehaviour
{
    [Header("Managers")]
    public Transform instructionManager;                                    // Reference to the instruction manager
    public Transform doorManager;                                           // Reference to the door manager
    public Transform checkpointManager;                                     // Reference to the checkpoint manager

    [Header("Interaction Settings")]
    public Transform player;                                                // Reference to the player
    public float rotationSpeed = 3f;                                        // Queen's rotation speed towards the player
    private bool isPlayerNearby = false;                                    // Tracks if the player is near the queen
    private bool isInteracting = false;                                     // Tracks if the queen is currently interacting with the player

    [Header("InteractUI Settings")]
    public GameObject interactUI;                                           // Reference to the interact UI
    public string interactMessage = "Press E to Talk to Unknown Woman";     // Interact message to display in the interact UI

    [Header("Conversation Settings")]
    public NPCConversation firstQueenConversation;                          // Queen's first conversation with the player
    public NPCConversation alternateFirstQueenConversation;                 // Queen's first conversation with the player if the player doesn't have enough gold
    public NPCConversation returnFirstQueenConversation;                    // Queen's conversation with the player after returning  

    public NPCConversation secondQueenConversation;                         // Queen's second conversation with the player
    public NPCConversation caughtSecondQueenConversation;                   // Queen's second conversation with the player if the player was caught
    public NPCConversation alternateSecondQueenConversation;                // Queen's alternate second conversation with the player
    public NPCConversation caughtAlternateSecondQueenConversation;          // Queen's alternate second conversation with the player if the player was caught
    public GameObject crossHair;                                            // Player's cross hair

    [Header("Wagon Settings")]
    public Transform wagon;                                                 // Reference to the wagon
    public Vector3 liftAmount = new Vector3(0, 0, -90);                     // Wagon's lift amount
    public float liftDuration = 2f;                                         // Wagon's lift duration
    public AudioClip wagonLifting;                                          // Audio clip of the wagon being lifted
    private AudioSource audioSource;                                        // AudioSource to play the door opening and closing

    [Header("Crystal Setings")]
    public Transform crystal;                                               // Reference to the crystal
    public AudioClip crystalCollecting;                                     // Audio clip of the crystal being collected

    [Header("Ending Settings")]
    public Image fadeImage;                                                 // Reference to fade at the end
    public float fadeDuration = 3f;                                         // Ending's fading duration

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
            bool checkpoint0 = GameManager.Instance.IsCheckpointReached(0);
            bool firstInteraction = GameManager.Instance.IsInteractionComplete(2);
            bool returnToQueen = GameManager.Instance.GetReturnToQueen();
            int playerGold = GameManager.Instance.GetPlayerGold();

            // Set the interact message
            if (currentRoom == 1 && checkpoint0 && !returnToQueen && !firstInteraction) 
            { interactUI.GetComponent<TextMeshProUGUI>().text = interactMessage; }

            else if (currentRoom == 1 && checkpoint0 && returnToQueen && 2 < playerGold && !firstInteraction)
            { interactUI.GetComponent<TextMeshProUGUI>().text = interactMessage; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player) {
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
        bool checkpoint0 = GameManager.Instance.IsCheckpointReached(0);
        bool firstInteraction = GameManager.Instance.IsInteractionComplete(2);
        bool returnToQueen = GameManager.Instance.GetReturnToQueen();
        int playerGold = GameManager.Instance.GetPlayerGold();

        // Handle interactions
        if (currentRoom == 1 && checkpoint0 && !returnToQueen && !firstInteraction)
        {
            isInteracting = true;
            FirstInteraction();
        }
        else if (currentRoom == 1 && checkpoint0 && returnToQueen && 2 < playerGold && !firstInteraction)
        {
            isInteracting = true;
            AlternateFirstInteraction();
        }
    }

    private void FirstInteraction()
    {
        // Begin the first interaction and reset the interact message
        GameManager.Instance.BeginInteraction(2);
        interactUI.GetComponent<TextMeshProUGUI>().text = "";

        // Disable the player controller and unlock the cursor
        player.GetComponent<ThirdPersonController>().DisableController();
        Cursor.lockState = CursorLockMode.None;

        // Disable the player's cross hair and enable their cursor
        crossHair.SetActive(false);
        Cursor.visible = true;

        // Hide the approach wagon instruction and sets conversation scroll speed to 1
        instructionManager.GetComponent<InstructionManager>().HideKingdomInstruction();
        ConversationManager.Instance.ScrollSpeed = 1;

        // Start the queen's first conversation with the player
        if (2 < GameManager.Instance.GetPlayerGold())
        { ConversationManager.Instance.StartConversation(firstQueenConversation); }
        else { ConversationManager.Instance.StartConversation(alternateFirstQueenConversation); }

        // Make the player face the queen
        StartCoroutine(player.GetComponent<ThirdPersonController>().FaceNPC(transform, 1));
        StartCoroutine(player.GetComponent<ThirdPersonController>().PanToNPC(transform, 1));
    }

    private void AlternateFirstInteraction()
    {
        // Begin the first interaction and reset the interact message
        GameManager.Instance.BeginInteraction(2);
        interactUI.GetComponent<TextMeshProUGUI>().text = "";

        // Disable the player controller and unlock the cursor
        player.GetComponent<ThirdPersonController>().DisableController();
        Cursor.lockState = CursorLockMode.None;

        // Disable the player's cross hair and enable their cursor
        crossHair.SetActive(false);
        Cursor.visible = true;

        // Hide the approach wagon instruction and start the queen's first conversation with the player
        instructionManager.GetComponent<InstructionManager>().HideGoldInstruction();
        ConversationManager.Instance.ScrollSpeed = 1;
        ConversationManager.Instance.StartConversation(returnFirstQueenConversation);

        // Make the player face the queen
        StartCoroutine(player.GetComponent<ThirdPersonController>().FaceNPC(transform, 1));
        StartCoroutine(player.GetComponent<ThirdPersonController>().PanToNPC(transform, 1));
    }

    public void MakeWagonChoice(bool choice) 
    {
        GameManager.Instance.MakeChoice(1, choice);
    }

    public void SetReturnToQueen()
    {
        GameManager.Instance.SetReturnToQueen(true);
    }

    public void LiftWagon()
    {
        StartCoroutine(RotateWagon());
        audioSource.PlayOneShot(wagonLifting);
    }

    private IEnumerator RotateWagon()
    {
        Quaternion startRotation = wagon.rotation;                                          // Initial rotation
        Quaternion targetRotation = Quaternion.Euler(wagon.eulerAngles + liftAmount);       // Target rotation

        // Time elapsed
        float timeElapsed = 0f;

        while (timeElapsed < liftDuration)
        {
            // Interpolate rotation over time
            wagon.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / liftDuration);

            timeElapsed += Time.deltaTime;      // Increment elapsed time
            yield return null;                  // Wait for the next frame
        }
    }

    public void ReceiveGold(int gold)
    {
        GameManager.Instance.RemovePlayerGold(gold);
    }

    public void EndFirstInteraction()
    {
        // Enable the player controller and lock the cursor
        player.GetComponent<ThirdPersonController>().EnableController();
        Cursor.lockState = CursorLockMode.Locked;

        // Disable the player's cursor and enable their cross hair
        Cursor.visible = false;
        crossHair.SetActive(true);

        // Complete the first interaction and turn off interaction mode
        GameManager.Instance.CompleteInteraction(2);
        isInteracting = false;

        // Show the infiltrate kingdom instruction and activate checkpoint 2
        instructionManager.GetComponent<InstructionManager>().ShowDirectionInstruction(1, 1);
        checkpointManager.GetComponent<CheckpointManager>().ActivateCheckpoint(1);
    }

    public void EndAlternateFirstInteraction()
    {
        // Enable the player controller and lock the cursor
        player.GetComponent<ThirdPersonController>().EnableController();
        Cursor.lockState = CursorLockMode.Locked;

        // Disable the player's cursor and enable their cross hair
        Cursor.visible = false;
        crossHair.SetActive(true);

        // Reset the first interaction and turn off interaction mode
        GameManager.Instance.ResetInteraction(2);
        isInteracting = false;

        // Show the return to queen instruction
        instructionManager.GetComponent<InstructionManager>().ShowGoldInstruction();
    }

    public void SecondInteraction()
    {
        // Begin the second interaction
        GameManager.Instance.BeginInteraction(3);
        StartCoroutine(HandleSecondInteraction());
    }

    private IEnumerator HandleSecondInteraction()
    {
        // Make the player face the crystal
        StartCoroutine(player.GetComponent<ThirdPersonController>().FaceNPC(crystal, 1));
        yield return StartCoroutine(player.GetComponent<ThirdPersonController>().PanToNPC(crystal, 1));

        // Hide crystal
        CollectCrystal();
        yield return new WaitForSeconds(0.5f);

        // Close door
        doorManager.GetComponent<DoorManager>().CloseThroneRoomDoor();
        yield return new WaitForSeconds(2.5f);

        // Unlock the cursor and enable it
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start the queen's second conversation with the player
        if (GameManager.Instance.getAct3CaughtCounter() < 1)
        {
            if (GameManager.Instance.GetChoice(1)) { ConversationManager.Instance.StartConversation(secondQueenConversation); }
            else { ConversationManager.Instance.StartConversation(alternateSecondQueenConversation); }
        }
        else
        {
            if (GameManager.Instance.GetChoice(1)) { ConversationManager.Instance.StartConversation(caughtSecondQueenConversation); }
            else { ConversationManager.Instance.StartConversation(caughtAlternateSecondQueenConversation); }
        }

    }

    public void CollectCrystal()
    {
        audioSource.PlayOneShot(crystalCollecting);
        crystal.gameObject.SetActive(false);
    }

    public void FaceQueen()
    {
        // Make the player face the queen
        StartCoroutine(player.GetComponent<ThirdPersonController>().FaceNPC(transform, 0.75f));
        StartCoroutine(player.GetComponent<ThirdPersonController>().PanToNPC(transform, 0.75f));
    }

    public void FirstEnding()
    {
        // Lock the cursor and disable it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // End the second interaction
        GameManager.Instance.CompleteInteraction(3);

        // End game
        StartCoroutine(HandleEnding(4));
    }

    public void SecondEnding()
    {
        // Lock the cursor and disable it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // End the second interaction
        GameManager.Instance.CompleteInteraction(3);

        // End game
        StartCoroutine(HandleEnding(5));
    }

    private IEnumerator HandleEnding(int sceneIndex)
    {
        yield return StartCoroutine(Fade(fadeDuration, 1));
        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator Fade(float fadeDuration, float targetAlpha)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;

        float time = 0;
        while (time < fadeDuration)
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = color;

            time += Time.deltaTime;
            
            yield return null;
        }
    }

    public void AdjustMoralityScore(int score)
    {
        GameManager.Instance.AdjustMoralityScore(score);
    }
}

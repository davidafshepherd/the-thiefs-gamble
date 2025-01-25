using System.Collections;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject crosshair;                        // Reference to the player's cross hair
    public GameObject conversationDialogue;             // Reference to the conversation manager's dialogue
    public GameObject conversationOptions;              // Reference to the conversation manager's options
    public GameObject interactUI;                       // Reference to the interact UI

    [Header("Meeting Room Instructions")]
    public GameObject approachInstruction;              // Reference to the approach instruction
    public GameObject exitInstruction;                  // Reference to the exit instruction

    [Header("Meeting Room Stars")]
    public GameObject approachStar;                     // Reference to the approach instruction's star
    public GameObject exitStar;                         // Reference to the exit instruction's star

    [Header("Main Scene Instructions")]
    public GameObject pickpocketInstruction;            // Reference to the pickpocket thief instruction
    public GameObject returnInstruction;                // Reference to the return to king instruction
    public GameObject wizardInstruction;                // Reference to the visit wizard instruction
    public GameObject kingdomInstruction;               // Reference to the head for kingdom instruction
    public GameObject goldInstruction;                  // Reference to the return to woman instruction
    public GameObject castleInstruction;                // Reference to the enter castle instruction
    public GameObject[] directionInstructions;          // Array of direction instructions

    [Header("Main Scene Stars")]
    public GameObject pickpocketStar;                   // Reference to the pickpocket thief instruction's star
    public GameObject returnStar;                       // Reference to the return to king instruction's star
    public GameObject wizardStar;                       // Reference to the visit wizard instruction's star
    public GameObject kingdomStar;                      // Reference to the head for kingdom instruction's star
    public GameObject goldStar;                         // Reference to the return to queen instruction's star
    public GameObject castleStar;                       // Reference to the enter castle instruction's star
    public GameObject[] checkpointStars;                // Array of checkpoint stars

    [Header("Throne Room Instructions")]
    public GameObject crystalInstruction;               // Reference to the steal crystal instruction

    [Header("Throne Room Stars")]
    public GameObject crystalStar;                      // Reference to the steal crystal instruction's star

    private void Start()
    {
        // Store the current room
        int currentRoom = GameManager.Instance.GetCurrentRoom();

        // Display the approach instruction, if the 1st interaction hasn't began
        if (currentRoom == 0 && !GameManager.Instance.IsInteractionOnGoing(0)) 
        { ShowApproachInstruction(); }

        // Display the exit instruction, if the 1st interaction has finished and the player hasn't finished the 1st task
        if (currentRoom == 0 && GameManager.Instance.IsInteractionComplete(0) && !GameManager.Instance.IsTaskComplete(0)) 
        { ShowExitInstruction(); }

        // Display the pick pocket thief instruction, if the 1st interaction has finished and the player hasn't finished the 1st task
        if (currentRoom == 1 && GameManager.Instance.IsInteractionComplete(0) && !GameManager.Instance.IsTaskComplete(0)) 
        { ShowPickpocketInstrucion(); }

        // Display the return to king instruction, if the player has finished the 1st task and the 2nd interaction hasn't began
        if (currentRoom == 1 && GameManager.Instance.IsTaskComplete(0) && !GameManager.Instance.IsInteractionOnGoing(1)) 
        { ShowReturnInstruction(); }

        // Display the approach instruction, if the 1st interaction has finished, the player has finished the 1st task and the second interaction hasn't began
        if (currentRoom == 0 && GameManager.Instance.IsInteractionComplete(0) && GameManager.Instance.IsTaskComplete(0) && !GameManager.Instance.IsInteractionComplete(1)) 
        { ShowApproachInstruction(); }

        // Display the exit instrution, if the 2nd interaction has finished and the player hasn't finished the 2nd task
        if (currentRoom == 0 && GameManager.Instance.IsInteractionComplete(1) && !GameManager.Instance.IsTaskComplete(1)) 
        { ShowExitInstruction(); }

        // Display the visit wizard instruction, if the 2nd interaction has finished and the player hasn't began the 3rd task
        if (currentRoom == 1 && GameManager.Instance.IsInteractionComplete(1) && !GameManager.Instance.IsTaskOnGoing(2)) 
        { ShowWizardInstruction(); }
        


        // Display the leave kingdom instruction, if the player has finished the 3rd task and hasn't reached checkpoint 0
        if (currentRoom == 1 && GameManager.Instance.IsTaskComplete(2) && !GameManager.Instance.IsCheckpointReached(0)) 
        { ShowDirectionInstruction(0, 0); }

        // Display the head for kingdom instruction, if the player has reached checkpoint 0 and the 3rd interaction hasn't began
        if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(0) && !GameManager.Instance.GetReturnToQueen() && !GameManager.Instance.IsInteractionOnGoing(2)) 
        { ShowKingdomInstruction(); }

        // Display the return to queen instruction, if the player has reached checkpoint 0 and has promised to return to the queen with the gold
        if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(0) && GameManager.Instance.GetReturnToQueen() && !GameManager.Instance.IsInteractionOnGoing(2))
        { ShowGoldInstruction(); }

        // Display the infiltrate kingdom instruction, if the 3rd interaction has finished and the player hasn't reached checkpoint 1
        if (currentRoom == 1 && GameManager.Instance.IsInteractionOnGoing(2) && !GameManager.Instance.IsCheckpointReached(1))
        { ShowDirectionInstruction(1, 1); }

        // Display the sneak through cemetary instruction, if the player's next checkpoint is 2
        if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(1) && !GameManager.Instance.IsCheckpointReached(2))
        { ShowDirectionInstruction(2, 2); }

        // Display the head for gate instruction, if the player's next checkpoint is 3
        if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(2) && !GameManager.Instance.IsCheckpointReached(3))
        { ShowDirectionInstruction(3, 3); }

        // Display the get across laval instruction, if the player's next checkpoint is 4
        if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(3) && !GameManager.Instance.IsCheckpointReached(4))
        { ShowDirectionInstruction(4, 4); }

        // Display the climb wall instruction, if the player's next checkpoint is 5
        if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(4) && !GameManager.Instance.IsCheckpointReached(5))
        { ShowDirectionInstruction(5, 5); }

        // Display the enter castle instruction, if the player has reached checkpoint 5 and the 4th interaction hasn't began
        if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(5) && !GameManager.Instance.IsInteractionOnGoing(3))
        { ShowCastleInstruction(); }



        // Display the steal crystal instruction, if the 4th interaction hasn't began
        if (currentRoom == 2 && !GameManager.Instance.IsInteractionOnGoing(3))
        { ShowCrystalInstruction(); }
    }

    private void HideUI()
    {
        crosshair.SetActive(false);
        conversationDialogue.SetActive(false);
        conversationOptions.SetActive(false);
        if (interactUI != null) interactUI.SetActive(false);
    }

    private IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(1.5f);
        crosshair.SetActive(true);
        if (interactUI != null) interactUI.SetActive(true);
    }

    public void ShowApproachInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        approachInstruction.SetActive(true);
        approachStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideApproachInstruction()
    {
        // Hide instruction and star
        approachInstruction.SetActive(false);
        approachStar.SetActive(false);
    }

    public void ShowExitInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        exitInstruction.SetActive(true);
        exitStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideExitInstruction()
    {
        // Hide instruction and star
        exitInstruction.SetActive(false);
        exitStar.SetActive(false);
    }

    public void ShowPickpocketInstrucion()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        pickpocketInstruction.SetActive(true);
        pickpocketStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HidePickpocketInstruction()
    {
        // Hide instruction and star
        pickpocketInstruction.SetActive(false);
        pickpocketStar.SetActive(false);
    }

    public void ShowReturnInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        returnInstruction.SetActive(true);
        returnStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideReturnInstruction()
    {
        // Hide instruction and star
        returnInstruction.SetActive(false);
        returnStar.SetActive(false);
    }

    public void ShowWizardInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        wizardInstruction.SetActive(true);
        wizardStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideWizardInstruction()
    {
        // Hide instruction and star
        wizardInstruction.SetActive(false);
        wizardStar.SetActive(false);
    }

    public void ShowDirectionInstruction(int directionIndex, int starIndex)
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        directionInstructions[directionIndex].SetActive(true);
        checkpointStars[starIndex].SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideDirectionInstruction(int directionIndex, int starIndex)
    {
        // Hide instruction and star
        directionInstructions[directionIndex].SetActive(false);
        checkpointStars[starIndex].SetActive(false);
    }

    public void ShowKingdomInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        kingdomInstruction.SetActive(true);
        kingdomStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideKingdomInstruction()
    {
        // Hide instruction and star
        kingdomInstruction.SetActive(false);
        kingdomStar.SetActive(false);
    }

    public void ShowGoldInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        goldInstruction.SetActive(true);
        goldStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideGoldInstruction()
    {
        // Hide instruction and star
        goldInstruction.SetActive(false);
        goldStar.SetActive(false);
    }

    public void ShowCastleInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        castleInstruction.SetActive(true);
        castleStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideCastleInstruction()
    {
        // Hide instruction and star
        castleInstruction.SetActive(false);
        castleStar.SetActive(false);
    }

    public void ShowCrystalInstruction()
    {
        // Hide UI
        HideUI();

        // Display instruction and star
        crystalInstruction.SetActive(true);
        crystalStar.SetActive(true);

        // Display UI
        StartCoroutine(ShowUI());
    }

    public void HideCrystalInstruction()
    {
        // Hide instruction and star
        crystalInstruction.SetActive(false);
        crystalStar.SetActive(false);
    }
}
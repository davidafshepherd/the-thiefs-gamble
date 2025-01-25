using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;
using UnityEngine.InputSystem;
using TMPro;
using UnityEditor;

public class WizardController : MonoBehaviour
{
    [Header("Managers")]
    public Transform instructionManager;
    public Transform checkpointManager;
    public Transform doorManager;

    [Header("Interaction Settings")]
    public Transform player;
    public GameObject playerController;
    private PlayerInput playerInput;
    public float rotationSpeed = 5f;
    private bool isPlayerNearby = false;

    private NPCConversation myConversation;
    [SerializeField] private NPCConversation virtuous;
    [SerializeField] private NPCConversation neutral;
    [SerializeField] private NPCConversation deceptive;
    [SerializeField] float textSpeed = 1f;

    [SerializeField] private GameObject shopUI;

    [SerializeField] private GameObject interactUI;
    private bool hasSetMessageUI = false;
    private string interactMessage = "Press E to Talk to Wizard";
    private string skipMessage = "";

    // Store base and previous prices
    private int baseInvisibilityCost;
    private int baseRevealCost;
    private int baseSpeedCost;
    private int previousInvisibilityCost;
    private int previousRevealCost;
    private int previousSpeedCost;

    private void Start()
    {
        playerInput = playerController.GetComponent<PlayerInput>();

        // Cache the base prices
        var invisibilityController = playerController.GetComponent<InvisibilitySpellController>();
        var revealSpellController = playerController.GetComponent<RevealSpellController>();
        var speedSpellController = playerController.GetComponent<SpeedSpellController>();

        if (invisibilityController != null) baseInvisibilityCost = invisibilityController.cost;
        if (revealSpellController != null) baseRevealCost = revealSpellController.cost;
        if (speedSpellController != null) baseSpeedCost = speedSpellController.cost;

        // Initialize previous costs with base costs
        previousInvisibilityCost = baseInvisibilityCost;
        previousRevealCost = baseRevealCost;
        previousSpeedCost = baseSpeedCost;
    }

    private void Update()
    {
        if (isPlayerNearby)
        {
            FacePlayer();
            SetMessageUI(interactMessage);

            if (Input.GetKeyDown(KeyCode.E) && !ConversationManager.Instance.IsConversationActive)
            {
                ResetMessageUI();
                HandleInteraction();
            }
        }
        else { ResetMessageUI(); }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            isPlayerNearby = false;
        }
    }

    private void HandleInteraction()
    {
        SetMessageUI(skipMessage);
        StartInteraction();
        ConversationManager.Instance.ScrollSpeed = textSpeed;

        switch (GameManager.Instance.GetMoralityAlignment())
        {
            case "Virtuous":
                myConversation = virtuous;
                break;
            case "Neutral":
                myConversation = neutral;
                break;
            case "Deceptive":
                myConversation = deceptive;
                break;
        }

        ConversationManager.Instance.StartConversation(myConversation);
    }

    public void StartInteraction()
    {
        if (!GameManager.Instance.IsTaskOnGoing(2))
        {
            instructionManager.GetComponent<InstructionManager>().HideWizardInstruction();
            GameManager.Instance.BeginTask(2);
        }

        playerInput.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void EndInteraction()
    {
        playerInput.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ResetMessageUI();

        if (!GameManager.Instance.IsTaskComplete(2))
        {
            GameManager.Instance.CompleteTask(2);
            doorManager.GetComponent<DoorManager>().OpenHumanCastleGate();

            instructionManager.GetComponent<InstructionManager>().ShowDirectionInstruction(0, 0);
            checkpointManager.GetComponent<CheckpointManager>().ActivateCheckpoint(0);
        }
    }

    public void openShop()
    {
        handleShopPrices();
        shopUI.SetActive(true);
    }

    private void handleShopPrices()
    {
        var invisibilityController = playerController.GetComponent<InvisibilitySpellController>();
        var revealSpellController = playerController.GetComponent<RevealSpellController>();
        var speedSpellController = playerController.GetComponent<SpeedSpellController>();

        if (invisibilityController != null)
        {
            int adjustedCost = GameManager.Instance.CalculateAdjustedPrice(baseInvisibilityCost);
            UpdatePreviousCostUI("Invisibility_Cost_Base", previousInvisibilityCost);
            UpdateCostUI("Invisibility_Cost", adjustedCost);
            previousInvisibilityCost = adjustedCost; // Update previous cost
            invisibilityController.cost = adjustedCost;
        }

        if (revealSpellController != null)
        {
            int adjustedCost = GameManager.Instance.CalculateAdjustedPrice(baseRevealCost);
            UpdatePreviousCostUI("Reveal_Cost_Base", previousRevealCost);
            UpdateCostUI("Reveal_Cost", adjustedCost);
            previousRevealCost = adjustedCost; // Update previous cost
            revealSpellController.cost = adjustedCost;
        }

        if (speedSpellController != null)
        {
            int adjustedCost = GameManager.Instance.CalculateAdjustedPrice(baseSpeedCost);
            UpdatePreviousCostUI("Speed_Cost_Base", previousSpeedCost);
            UpdateCostUI("Speed_Cost", adjustedCost);
            previousSpeedCost = adjustedCost; // Update previous cost
            speedSpellController.cost = adjustedCost;
        }
    }

    private void UpdateCostUI(string itemName, int cost)
    {
        Transform itemCostText = FindChildByName(shopUI.transform, itemName);
        if (itemCostText != null && itemCostText.GetComponent<Text>() != null)
        {
            itemCostText.GetComponent<Text>().text = $"{cost}";
        }
        else
        {
            Debug.LogWarning($"Cost UI for {itemName} not found or missing Text component.");
        }
    }

    private void UpdatePreviousCostUI(string previousItemName, int previousCost)
    {
        Transform previousCostText = FindChildByName(shopUI.transform, previousItemName);
        if (previousCostText != null)
        {
            if (previousCostText.GetComponent<Text>() != null)
            {
                previousCostText.GetComponent<Text>().text = $"{previousCost}";
                previousCostText.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Previous cost UI for {previousItemName} is missing a Text component.");
            }
        }
        else
        {
            Debug.LogWarning($"Previous cost GameObject for {previousItemName} not found.");
        }
    }

    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform found = FindChildByName(child, name);
            if (found != null) return found;
        }
        return null;
    }

    public void closeShop()
    {
        shopUI.SetActive(false);
        EndInteraction();
    }

    public void SetMessageUI(string message)
    {
        if (!hasSetMessageUI)
        {
            interactUI.GetComponent<TextMeshProUGUI>().text = message;
            hasSetMessageUI = true;
        }
    }

    public void ResetMessageUI()
    {
        if (hasSetMessageUI)
        {
            interactUI.GetComponent<TextMeshProUGUI>().text = string.Empty;
            hasSetMessageUI = false;
        }
    }

    public void AdjustMoralityScore(int score)
    {
        GameManager.Instance.AdjustMoralityScore(score);
    }
}

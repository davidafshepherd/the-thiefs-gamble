using System.Collections;
using UnityEngine;
using TMPro;

interface MyInteractable
{
    void Interact();
    float GetInteractDistance();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public GameObject interactUIObj;
    private TextMeshProUGUI interactUI; // Reference to the UI TextMeshPro object
    private bool isUIReset = true; // Tracks if the UI has already been reset

    public float interactionHoldTime = 2f; // Time required to hold E to interact
    private float interactionProgress = 0f; // Tracks progress of the interaction
    private MyInteractable currentInteractable; // The interactable object being targeted
    private Coroutine interactionCoroutine; // For handling hold interaction logic

    private RaycastHit? cachedRaycastHit; // Cache the last valid raycast hit

    private void Start()
    {
        interactUI = interactUIObj.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        PerformRaycast(); // Perform raycast once per frame
        UpdateUI();

        // Handle interaction progress
        if (currentInteractable != null && Input.GetKey(KeyCode.E))
        {
            if (interactionCoroutine == null)
            {
                interactionCoroutine = StartCoroutine(HoldToInteract());
            }
        }
        else
        {
            if (interactionCoroutine != null)
            {
                StopCoroutine(interactionCoroutine);
                interactionCoroutine = null;
                ResetInteractionProgress(); // Reset the progress if the player stops looking or releasing E
            }
        }
    }

    private void PerformRaycast()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo))
        {
            cachedRaycastHit = hitInfo; // Cache the result
        }
        else
        {
            cachedRaycastHit = null; // Clear the cache if nothing is hit
        }
    }

    private void UpdateUI()
    {
        if (cachedRaycastHit.HasValue)
        {
            RaycastHit hitInfo = cachedRaycastHit.Value;

            // Check if the hit object implements MyInteractable
            if (hitInfo.collider.gameObject.TryGetComponent(out MyInteractable interactObj))
            {
                float interactDistance = interactObj.GetInteractDistance();

                // Check if the hit object is within interaction distance
                if (hitInfo.distance <= interactDistance)
                {
                    currentInteractable = interactObj; // Store the current interactable

                    // Check for PickpocketableNPC and display "Cannot Pickpocket" or interaction progress
                    if (hitInfo.collider.gameObject.TryGetComponent(out ThiefController thiefController))
                    {
                        if (thiefController.HasPickpocketed())
                        {
                            interactUI.text = "Cannot Pickpocket";
                        }
                        else
                        {
                            interactUI.text = $"Hold E to Pickpocket ({Mathf.FloorToInt(interactionProgress / interactionHoldTime * 100)}%)";
                        }
                        isUIReset = false;
                        return;
                    }
                }
            }
        }

        // Reset UI if no valid interactable
        if (!isUIReset)
        {
            ResetInteractionProgress();
            interactUI.text = string.Empty;
            isUIReset = true;
            currentInteractable = null; // Clear current interactable
        }
    }

    private IEnumerator HoldToInteract()
    {
        while (interactionProgress < interactionHoldTime)
        {
            // Ensure the player is still looking at the same interactable
            if (!cachedRaycastHit.HasValue ||
                !cachedRaycastHit.Value.collider.gameObject.TryGetComponent(out MyInteractable interactObj) ||
                interactObj != currentInteractable)
            {
                ResetInteractionProgress();
                yield break; // Stop the coroutine if the player looks away
            }

            interactionProgress += Time.deltaTime;
            UpdateUI(); // Update the progress UI
            yield return null;
        }

        // Interaction is complete
        currentInteractable.Interact();
        ResetInteractionProgress();
    }

    private void ResetInteractionProgress()
    {
        interactionProgress = 0f;
        interactUI.text = string.Empty; // Clear the UI text
        isUIReset = true; // Mark UI as reset
        currentInteractable = null; // Clear the current interactable
    }
}

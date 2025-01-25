using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//interface MyInteractable
//{
//    void Interact();
//    float GetInteractDistance();
//}

public class OldInteractor : MonoBehaviour
{
    public Transform InteractorSource;
    public GameObject interactUIObj;
    private TextMeshProUGUI interactUI; // Reference to the UI TextMeshPro object
    private bool isUIReset = true; // Tracks if the UI has already been reset

    private void Start()
    {
        interactUI = interactUIObj.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // Cast a ray when E is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo))
            {
                // Check if the hit object implements MyInteractable
                if (hitInfo.collider.gameObject.TryGetComponent(out MyInteractable interactObj))
                {
                    float interactDistance = interactObj.GetInteractDistance();

                    // Check if the hit object is within interaction distance
                    if (hitInfo.distance <= interactDistance)
                    {
                        interactObj.Interact();
                    }
                }
            }
        }

        // Update UI text based on raycast hit
        UpdateUI();
    }

    private void UpdateUI()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo))
        {
            // Check if the hit object implements MyInteractable
            if (hitInfo.collider.gameObject.TryGetComponent(out MyInteractable interactObj))
            {
                float interactDistance = interactObj.GetInteractDistance();

                // Check if the hit object is within interaction distance
                if (hitInfo.distance <= interactDistance)
                {
                    // If the object has a PickpocketableNPC, show "Press E to Pickpocket"
                    if (hitInfo.collider.gameObject.TryGetComponent(out PickpocketableNPC pickPocketableNPC))
                    {
                        if (pickPocketableNPC.HasPickpocketed()) { interactUI.text = "Cannot Pickpocket"; }
                        else { interactUI.text = "Press E to Pickpocket"; }
                        isUIReset = false; // UI has been updated
                    }
                    // If the object has a ThiefController, show "Press E to Pickpocket"
                    else if (hitInfo.collider.gameObject.TryGetComponent(out ThiefController thiefController))
                    {
                        if (thiefController.HasPickpocketed()) { interactUI.text = "Cannot Pickpocket"; }
                        else { interactUI.text = "Press E to Pickpocket"; }
                        isUIReset = false; // UI has been updated
                    }
                    else
                    {
                        // Generic interaction text
                        interactUI.text = "Press E to Interact";
                        isUIReset = false; // UI has been updated
                    }
                    return; // Stop further checks if an interactable is detected
                }
            }
        }

        // If no interactable is detected and the UI has not been reset yet
        if (!isUIReset)
        {
            interactUI.text = string.Empty; // Clear the text
            isUIReset = true; // Mark UI as reset
        }
    }
}

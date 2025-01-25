using DialogueEditor;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointHandler : MonoBehaviour
{
    [Header("Managers")]
    public Transform instructionManager;            // Reference to the instruction manager
    public Transform doorManager;                   // Reference to the door manager
    public Transform checkpointManager;             // Reference to the checkpoint manager

    [Header("Checkpoint Settings")]
    public int currentDirection;                    // Index of the curent direction being displayed
    public int currentCheckpoint;                   // Index of the current checkpoint

    public int nextDirection;                       // Index of the next direction to be displayed
    public int nextCheckpoint;                      // Index of the next checkpoint

    [Header("End Checkpoint Settings (Optional)")]
    public bool endCheckpoint = false;              // Tracks if this checkpoint is an end checkpoint
    public bool kingdomCheckpoint = false;          // Tracks if this is the end checkpoint before the wagon interaction
    public bool castleCheckpoint = false;           // Tracks if this is the end checkpoint before entering the undead kingdom's castle

    public void OnTriggerEnter(Collider other)
    {
        // Hide the current direction instruction and deactivate the current checkpoint
        instructionManager.GetComponent<InstructionManager>().HideDirectionInstruction(currentDirection, currentCheckpoint);
        checkpointManager.GetComponent<CheckpointManager>().DeactivateCheckpoint(currentCheckpoint);

        // If checkpoint is an end checkpoint
        if (endCheckpoint)
        {
            // If end checkpoint before wagon interaction
            if (kingdomCheckpoint) { HandleKingdomCheckpoint(); }

            // If end checkpoint before entering the undead kingdom's castle
            if (castleCheckpoint) { HandleCastleCheckpoint(); }
        } 
        else
        { HandleCheckpoint();}
    }

    public void HandleCheckpoint()
    {
        // Show the next direction instruction and activate the next checkpoint
        instructionManager.GetComponent<InstructionManager>().ShowDirectionInstruction(nextDirection, nextCheckpoint);
        checkpointManager.GetComponent<CheckpointManager>().ActivateCheckpoint(nextCheckpoint);
    }

    public void HandleKingdomCheckpoint()
    {
        checkpointManager.GetComponent<CheckpointManager>().PlayPlayerSpeech();
        instructionManager.GetComponent<InstructionManager>().ShowKingdomInstruction();
    }

    public void HandleCastleCheckpoint()
    {
        instructionManager.GetComponent<InstructionManager>().ShowCastleInstruction();
        doorManager.GetComponent<DoorManager>().OpenUndeadCastleDoor();
    }
}

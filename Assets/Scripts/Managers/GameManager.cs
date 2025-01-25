using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Room Index:          0.Meeting Room Scene,       1.Main Scene;               2.Throne Room Scene.              
    // Door Index:          0.Meeting Room Door,        1.Human Castle Door,        2.Human Castle Gate,        3.Undead Castle Door,       4.Throne Room Door.

    // Interaction Index:   0.King sets test task,      1.Give gold to King / King sets main task,      2.Undead Queen asks for help;       3.Undead Queen confronts Player.
    // Task Index:          0.Rob Thief,                1.Retrieve crystal,                             2.Vist Wizard.

    // Choice Index:        0.Give all gold (T) / Keep half of gold (F),
    //                      1.Help Undead Queen (T) / Leave Undead Queen (F).

    // Checkpoint Index:    0.0,      1.01,      2.011,      3.0110,      4.01100,      5.012,      6.0120,      7.01200,      8.012000,
    //                      9.02,     10.020,    11.0200,    12.1,        13.2,         14.3,       15.4,        16.5.

    public static GameManager Instance;

    // Tracks the current room
    public int currentRoom = 0;
    public int savedCurrentRoom = 0;

    // Tracks the doors open
    private bool[] doorsOpen = new bool[5];
    private bool[] savedDoorsOpen = new bool[5];

    // Tracks the interactions started
    private bool[] interactionsStarted =new bool[4];
    private bool[] savedInteractionsStarted = new bool[4];

    // Tracks the interactions completed
    private bool[] interactionsCompleted = new bool[4];
    private bool[] savedInteractionsCompleted = new bool[4];

    // Tracks the tasks started
    private bool[] tasksStarted = new bool[3];
    private bool[] savedTasksStarted = new bool[3];

    // Tracks the tasks completed
    private bool[] tasksCompleted = new bool[3];
    private bool[] savedTasksCompleted = new bool[3];

    // Tracks the choices made
    private bool[] choicesMade = new bool[2];
    private bool[] savedChoicesMade = new bool[2];

    // Tracks the checkpoints activated
    private bool[] checkpointsActivated = new bool[18];
    private bool[] savedCheckpointsActivated = new bool[18];

    // Tracks the checkpoints reached
    private bool[] checkpointsReached = new bool[18];
    private bool[] savedCheckpointsReached = new bool[18];

    // Tracks the player's meeting room location
    private Vector3 MRLocation = new Vector3(18.9400005f, 0, 36.4000015f);
    private Vector3 savedMRLocation = new Vector3(18.9400005f, 0, 36.4000015f);

    // Tracks the player's meeting room rotation
    private Vector3 MRRotation = new Vector3(0, 90, 0);
    private Vector3 savedMRRotation = new Vector3(0, 90, 0);

    // Tracks the player's main scene location
    private Vector3 MSLocation = new Vector3(181.479996f, 0.270000011f, 191.559998f);
    private Vector3 savedMSLocation = new Vector3(181.479996f, 0.270000011f, 191.559998f);

    // Tracks the player's main scene rotation
    private Vector3 MSRotation = new Vector3(0, -90, 0);
    private Vector3 savedMSRotation = new Vector3(0, -90, 0);

    // Tracks the player's throne room location
    private Vector3 TRLocation = new Vector3(4.76000023f, -2.37800002f, 75.1299973f);
    private Vector3 savedTRLocation = new Vector3(4.76000023f, -2.37800002f, 75.1299973f);

    // Tracks the player's throne room rotation
    private Vector3 TRRotation = new Vector3(0, -90, 0);
    private Vector3 savedTRRotation = new Vector3(0, -90, 0);

    // Tracks the player's gold
    private int playerGold = 0;
    private int savedPlayerGold = 0;

    // Tracks if the player has left the queen and intends to return
    private bool returnToQueen = false;
    private bool savedReturnToQueen = false;

    // Tracks if the invisibility spell has been purchased
    public bool isInvisibilityPurchased = false;
    public bool savedIsInvisibilityPurchased = false;

    // Tracks if the speed spell has been purchased
    public bool isSpeedPurchased = false;
    public bool savedIsSpeedPurchased = false;

    // Tracks if the reveal spell has been purchased
    public bool isRevealPurchased = false;
    public bool savedIsRevealPurchased = false;

    // Tracks player's morality behavior (-100 to 100 scale)
    public int moralityScore = 0;
    public int savedMoralityScore = 0;

    //Track Day or night time
    public bool isDay = true;
    public bool savedIsDay = true;

    // Tracks the last save's position and rotation
    private Vector3 saveLocation = new Vector3(177.639999f, 1.18773568f, 191.199997f);
    private Vector3 saveRotation = new Vector3(0, -90, 0);

    // Tracks the last save
    private int save = 0;

    private int act1CaughtCounter = 0;
    private int act2CaughtCounter = 0;
    private int act3CaughtCounter = 0;

    private void Awake()
    {
        if (Instance == null)
        {  
            // Persist across scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            // Prevent duplicates
            Destroy(gameObject); 
        }
    }
    
    public void OpenDoor(int doorIndex)
    {
        doorsOpen[doorIndex] = true;
    }

    public void CloseDoor(int doorIndex)
    {
        doorsOpen[doorIndex] = false;
    }

    public bool IsDoorOpen(int doorIndex)
    {
        return doorsOpen[doorIndex];
    }

    public int GetCurrentRoom()
    {
        return currentRoom;
    }

    public void ChangeRoom(int roomIndex)
    {
        currentRoom = roomIndex;
    }

    public void BeginInteraction(int interactionIndex)
    {
        interactionsStarted[interactionIndex] = true;
    }

    public bool IsInteractionOnGoing(int interactionIndex)
    {
        return interactionsStarted[interactionIndex];
    }

    public void CompleteInteraction(int interactionIndex)
    {
        interactionsCompleted[interactionIndex] = true;
    }

    public void ResetInteraction(int interactionIndex)
    {
        interactionsStarted[interactionIndex] = false;
        interactionsCompleted[interactionIndex] = false;
    }

    public bool IsInteractionComplete(int interactionIndex)
    {
        return interactionsCompleted[interactionIndex];
    }

    public void BeginTask(int taskIndex)
    {
        tasksStarted[taskIndex] = true;
    }

    public bool IsTaskOnGoing(int taskIndex)
    {
        return tasksStarted[taskIndex];
    }

    public void CompleteTask(int taskIndex)
    {
        tasksCompleted[taskIndex] = true;

    }

    public bool IsTaskComplete(int taskIndex)
    {
        return tasksCompleted[taskIndex];
    }

    public void MakeChoice(int choiceIndex, bool choice)
    {
        choicesMade[choiceIndex] = choice;
    }

    public bool GetChoice(int choiceIndex)
    {
        return choicesMade[choiceIndex];
    }

    public void ActivateCheckpoint(int checkpointIndex)
    {
        checkpointsActivated[checkpointIndex] = true;
    }

    public bool IsCheckpointActivated(int checkPointIndex)
    {
        return checkpointsActivated[checkPointIndex];
    }

    public void ReachCheckpoint(int checkpointIndex)
    {
        checkpointsReached[checkpointIndex] = true;
    }

    public bool IsCheckpointReached(int checkPointIndex)
    {
        return checkpointsReached[checkPointIndex];
    }

    public void SetLocation(Vector3 location)
    {
        if (currentRoom == 0) MRLocation = location;
        if (currentRoom == 1) MSLocation = location;
        if (currentRoom == 2) TRLocation = location;
    }

    public Vector3 GetLocation()
    {
        if (currentRoom == 0) return MRLocation;
        else if (currentRoom == 1) return MSLocation;
        else if (currentRoom == 2) return TRLocation;
        else return Vector3.zero;
    }

    public void SetRotation(Vector3 rotation)
    {
        if (currentRoom == 0) MRRotation = rotation;
        if (currentRoom == 1) MSRotation = rotation;
        if (currentRoom == 2) TRRotation = rotation;
    }

    public Vector3 GetRotation()
    {
        if (currentRoom == 0) return MRRotation;
        else if (currentRoom == 1) return MSRotation;
        else if (currentRoom == 2) return TRRotation;
        else return Vector3.zero;
    }

    public int GetPlayerGold()
    {
        return playerGold;
    }

    public void AddPlayerGold(int gold)
    {
        playerGold += gold;
    }

    public void RemovePlayerGold(int gold)
    {
        playerGold -= gold;
    }

    public void SetReturnToQueen(bool choice)
    {
        returnToQueen = choice;
    }

    public bool GetReturnToQueen()
    {
        return returnToQueen;
    }

    public bool getIsInvisibilityPurchased()
    {
        return isInvisibilityPurchased;
    }

    public void setInvisibilityPurchased(bool purchased)
    {
        isInvisibilityPurchased = purchased;
    }

    public bool getIsSpeedPurchased()
    {
        return isSpeedPurchased;
    }

    public void setSpeedPurchased(bool purchased)
    {
        isSpeedPurchased = purchased;
    }

    public bool getIsRevealPurchased()
    {
        return isRevealPurchased;
    }

    public void setRevealPurchased(bool purchased)
    {
        isRevealPurchased = purchased;
    }

    // Method to modify the morality score
    public void AdjustMoralityScore(int value)
    {
        moralityScore += value;
        Debug.Log("Morality Score Updated: " + moralityScore);
    }

    // Example method to determine the player's morality alignment
    public string GetMoralityAlignment()
    {
        if (moralityScore > 0)
            return "Virtuous";
        else if (moralityScore < 0)
            return "Deceptive";
        else
            return "Neutral";
    }

    // Adjust prices based on morality score
    public int CalculateAdjustedPrice(int basePrice)
    {
        // Calculate the adjustment percentage
        // Positive morality score gives a discount, negative morality score adds a penalty
        float adjustmentPercentage = -moralityScore * 1.5f; // Positive scores reduce price, negative increase it

        // Convert percentage to a multiplier
        float adjustmentMultiplier = 1 + (adjustmentPercentage / 100f);

        // Calculate the adjusted price
        int adjustedPrice = Mathf.CeilToInt(basePrice * adjustmentMultiplier);

        // Ensure the price is at least 1
        return Mathf.Max(adjustedPrice, 1);
    }

    public void SetSaveLocation(Vector3 location)
    {
        saveLocation = location;
    }

    public Vector3 GetSaveLocation()
    {
        return saveLocation;
    }

    public void SetSaveRotation(Vector3 rotation)
    {
        saveRotation = rotation;
    }

    public Vector3 GetSaveRotation()
    {
        return saveRotation;
    }

    public void SaveGame()
    {
        Debug.Log("Game saved...");
        // Copy values from game state arrays to backup arrays
        savedCurrentRoom = currentRoom;
        for (int i = 0; i < doorsOpen.Length; i++) savedDoorsOpen[i] = doorsOpen[i];

        for (int i = 0; i < interactionsStarted.Length; i++) savedInteractionsStarted[i] = interactionsStarted[i];
        for (int i = 0; i < interactionsCompleted.Length; i++) savedInteractionsCompleted[i] = interactionsCompleted[i];

        for (int i = 0; i < tasksStarted.Length; i++) savedTasksStarted[i] = tasksStarted[i];
        for (int i = 0; i < tasksCompleted.Length; i++) savedTasksCompleted[i] = tasksCompleted[i];

        for (int i = 0; i < choicesMade.Length; i++) savedChoicesMade[i] = choicesMade[i];

        for (int i = 0; i < checkpointsActivated.Length; i++) savedCheckpointsActivated[i] = checkpointsActivated[i];
        for (int i = 0; i < checkpointsReached.Length; i++) savedCheckpointsReached[i] = checkpointsReached[i];

        savedMRLocation = MRLocation;
        savedMRRotation = MRRotation;

        savedMSLocation = MSLocation;
        savedMSRotation = MSRotation;
        
        savedTRLocation = TRLocation;
        savedTRRotation = TRRotation;

        savedPlayerGold = playerGold;
        savedReturnToQueen = returnToQueen;

        savedIsInvisibilityPurchased = isInvisibilityPurchased;
        savedIsSpeedPurchased = isSpeedPurchased;
        savedIsRevealPurchased = isRevealPurchased;

        savedMoralityScore = moralityScore;

        savedIsDay = isDay;
    }

    public void ResetToLastSave()
    {
        Debug.Log("Resetting to last save...");
        // Copy values from backup arrays to game state arrays
        currentRoom = savedCurrentRoom;
        for (int i = 0; i < savedDoorsOpen.Length; i++) doorsOpen[i] = savedDoorsOpen[i];

        for (int i = 0; i < savedInteractionsStarted.Length; i++) interactionsStarted[i] = savedInteractionsStarted[i];
        for (int i = 0; i < savedInteractionsCompleted.Length; i++) interactionsCompleted[i] = savedInteractionsCompleted[i];

        for (int i = 0; i < savedTasksStarted.Length; i++) tasksStarted[i] = savedTasksStarted[i];
        for (int i = 0; i < savedTasksCompleted.Length; i++) tasksCompleted[i] = savedTasksCompleted[i];

        for (int i = 0; i < savedChoicesMade.Length; i++) choicesMade[i] = savedChoicesMade[i];

        for (int i = 0; i < savedCheckpointsActivated.Length; i++) checkpointsActivated[i] = savedCheckpointsActivated[i];
        for (int i = 0; i < savedCheckpointsReached.Length; i++) checkpointsReached[i] = savedCheckpointsReached[i];

        MRLocation = savedMRLocation;
        MRRotation = savedMRRotation;

        MSLocation = savedMSLocation;
        MSRotation = savedMSRotation;

        TRLocation = savedTRLocation;
        TRRotation = savedTRRotation;

        playerGold = savedPlayerGold;
        returnToQueen = savedReturnToQueen;

        isInvisibilityPurchased = savedIsInvisibilityPurchased;
        isSpeedPurchased = savedIsSpeedPurchased;
        isRevealPurchased = savedIsRevealPurchased;

        moralityScore = savedMoralityScore;

        isDay = savedIsDay;

        // Set save position and rotation
        SetLocation(saveLocation);
        SetRotation(saveRotation);

        // Reset game
        SceneManager.LoadScene(currentRoom+1);
    }

    public void IncrementNextSave()
    {
        save++;
    }

    public int GetNextSave()
    {
        return save;
    }

    public int getAct1CaughtCounter()
    {
        return act1CaughtCounter;
    }

    public int getAct2CaughtCounter()
    {
        return act2CaughtCounter;
    }

    public int getAct3CaughtCounter()
    {
        return act3CaughtCounter;
    }

    public void incrementAct1CaughtCounter()
    {
        act1CaughtCounter++;
        Debug.Log($"Act1 caught counter is now {act1CaughtCounter}");
    }

    public void incrementAct2CaughtCounter()
    {
        act2CaughtCounter++;
        Debug.Log($"Act2 caught counter is now {act2CaughtCounter}");
    }

    public void incrementAct3CaughtCounter()
    {
        act3CaughtCounter++;
        Debug.Log($"Act3 caught counter is now {act3CaughtCounter}");
    }
}
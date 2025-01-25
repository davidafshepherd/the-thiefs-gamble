using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class ThiefController : MonoBehaviour, MyInteractable
{
    private enum ThiefState { Patrolling, Chasing, Searching }
    private ThiefState currentState = ThiefState.Patrolling;

    [Header("References")]
    private NavMeshAgent agent;                                         // Reference to the NavMeshAgent
    private Animator animator;                                          // Reference to the Animator
    public Transform player;                                            // Reference to the player
    private InvisibilitySpellController invisibilitySpellController;    // Reference to the invisibility controller
    public Transform instructionManager;                                // Reference to the instruction manager
    public SpottedScript spottedScript;
    public CaughtScreenEffect caughtScript;
    private bool isPlayerCaught = false; // Tracks if the player has been caught

    [Header("Patrolling Settings")]
    public Transform patrolWaypointsParent; // Reference to the parent object containing all waypoints
    private Transform[] patrolPoints;       // Array to store the dynamically added waypoints
    private int nextPatrolPoint;                // Index of the next patrol point
    public float walkingSpeed = 4f;             // Thief's walking speed
    public float maxSpeed = 20f;
    public float pauseDuration = 5f; // Duration to pause at each patrol point
    private bool isPaused = false; // Tracks if the thief is paused

    [Header("Field of Vision Settings")]
    public float fieldOfViewAngle = 45f;        // Field of vision angle in degrees
    public float visionRange = 10f;             // Maximum distance the thief can "see"

    [Header("Detection Settings")]
    public float detectionTime = 3f;            // Time required to detect the player
    private float currentDetectionTime = 0f;    // Timer for detecting the player
    private bool isPlayerInSight = false;       // Whether the player is currently in sight

    [Header("Searching Settings")]
    public float searchDuration = 5f;           // Duration to search for the player
    private bool isSearching = false;           // Whether the agent is currently searching
    private Vector3 lastKnownPosition;          // Last known position of the player
    public float searchRadius = 5f;             // Radius around the last known position to search

    [Header("Chase Settings")]
    public float chaseDuration = 3f;            // Duration to chase the player after detection
    private float chaseTimeRemaining;           // Timer for how long the thief will chase the player
    public float chaseSpeed = 8f;               // Speed during chasing

    [Header("Interaction Settings")]
    public float interactDistance = 15f;        // Distance from which the thief can be interacted with
    private bool hasPickpocketed = false;       // Tracks if the thief has been pick pocketed or not
    public int goldAmount = 20;                 // Amount of gold on the thief

    [Header("UI Elements")]
    public Canvas canvas;                       // Reference to the canvas for the detection bar
    public Slider slider;                       // Reference to the progress bar UI

    [Header("Sound Settings")]
    public AudioClip[] alertSounds; // Array of NPC alerted sound effects
    private AudioSource audioSource; // Reference to the AudioSource
    private bool hasPlayedAlertSound = false; // Tracks if the alert sound has been played
    public AudioClip pickpocketSound;


    private void Awake()
    {
        // Initialize NavMeshAgent and Animator
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from this GameObject!");
        }

    }

    private void Start()
    {
        if (patrolWaypointsParent != null)
        {
            // Get all child transforms of the parent and populate the patrolPoints array
            patrolPoints = new Transform[patrolWaypointsParent.childCount];
            for (int i = 0; i < patrolWaypointsParent.childCount; i++)
            {
                patrolPoints[i] = patrolWaypointsParent.GetChild(i);
            }
        }
        else
        {
            // No PatrolWaypointsParent assigned; assume this is a static character
            Debug.LogWarning("No PatrolWaypointsParent assigned. Setting Thief to static patrol at current position.");
            patrolPoints = new Transform[1];

            // Create a new GameObject at the Thief's current position to serve as the static patrol point
            GameObject staticPoint = new GameObject("StaticPatrolPoint");
            staticPoint.transform.position = transform.position;
            patrolPoints[0] = staticPoint.transform;
        }

        // Start patrolling
        nextPatrolPoint = 0;
        isPaused = false;
        SetNextPatrolPoint();

        // Find the player's invisibility controller
        invisibilitySpellController = player.GetComponent<InvisibilitySpellController>();

        // Set the thief's pick pocketed state
        if (GameManager.Instance.IsTaskComplete(0) && gameObject.name == "Thief") { hasPickpocketed = true; }

        chaseTimeRemaining = chaseDuration;
    }

    private void Update()
    {
        DetectPlayer();

        // Update Animator Parameters
        UpdateAnimatorParameters();

        switch (currentState)
        {
            case ThiefState.Patrolling:
                agent.speed = walkingSpeed;
                Patrol();
                hasPlayedAlertSound = false;
                DepleteDetectionBar();
                break;
            case ThiefState.Chasing:
                agent.speed = chaseSpeed;
                ChasePlayer();
                break;
            case ThiefState.Searching:
                agent.speed = walkingSpeed;
                hasPlayedAlertSound = false;
                SearchForPlayer();
                DepleteDetectionBar();
                break;
        }

        // Update detection progress bar
        UpdateDetectionBar();
    }

    

    private void Patrol()
    {
        if (isPaused) return; // Skip moving while paused

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(PauseAtPatrolPoint());
        }
    }

    private IEnumerator PauseAtPatrolPoint()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        SetNextPatrolPoint();
        isPaused = false;
    }

    private void SetNextPatrolPoint()
    {
        // Set the next patrol point
        agent.SetDestination(patrolPoints[nextPatrolPoint].position);

        // Update the next patrol point
        nextPatrolPoint = (nextPatrolPoint + 1) % patrolPoints.Length;
    }


    private void ChasePlayer()
    {

        if (isPlayerInSight)
        {
            chaseTimeRemaining = chaseDuration; // Reset the chase timer when the player is in sight
            agent.SetDestination(player.position);
            lastKnownPosition = player.position;
        }
        else
        {
            chaseTimeRemaining -= Time.deltaTime;
            agent.SetDestination(player.position);
            lastKnownPosition = player.position;

            if (chaseTimeRemaining <= 0f)
            {
                currentState = ThiefState.Searching;
                StartSearching();
            }
        }
    }

    private void SearchForPlayer()
    {

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            SearchAroundLastKnownPosition();
        }
    }


    

    private void UpdateAnimatorParameters()
    {
        // Normalize movement speed for animations
        float normalizedSpeed = agent.velocity.magnitude / maxSpeed;

        // Update Speed parameter in the Animator
        animator.SetFloat("Speed_f", normalizedSpeed);
    }

    private void DetectPlayer()
    {
        // Check if the player is invisible
        if (invisibilitySpellController != null && invisibilitySpellController.IsActive())
        {
            // Reset detection if the player is invisible
            if (isPlayerInSight)
            {
                isPlayerInSight = false;
                currentDetectionTime = 0f;

                // Notify the SpottedScript
                FindObjectOfType<SpottedScript>()?.RemoveThiefDetection();
            }

            // Exit the method to ensure no further detection occurs
            return;
        }

        // Calculate the direction to the player
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check if the player is within vision range
        if (distanceToPlayer <= visionRange)
        {
            // Normalize the direction and check the angle
            directionToPlayer.Normalize();
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= fieldOfViewAngle / 2f)
            {
                // Perform a raycast to ensure the player is not blocked by obstacles
                Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer);
                if (Physics.Raycast(ray, out RaycastHit hit, visionRange))
                {
                    if (hit.transform == player)
                    {
                        if (isPlayerCaught)
                        {
                            FindObjectOfType<SpottedScript>()?.LockAddThiefDetection();
                        }
                        if (!isPlayerInSight && !isPlayerCaught)
                        {
                            isPlayerInSight = true;
                            FindObjectOfType<SpottedScript>()?.AddThiefDetection();
                        }
                        
                        currentDetectionTime += Time.deltaTime;

                        if (currentDetectionTime >= detectionTime && !isPlayerCaught)
                        {
                            // Trigger the caught effect and set the player as caught
                            isPlayerCaught = true;
                            HandlePlayerCaught();
                        }

                        if (currentState != ThiefState.Chasing)
                        {
                            StartChasing();
                        }

                        return;
                    }
                }
            }
        }

        // Reset detection if the player is not in sight
        if (isPlayerInSight)
        {
            isPlayerInSight = false;
        }
    }

    private void HandlePlayerCaught()
    {
        if (caughtScript != null)
        {
            if (gameObject.name == "Thief")
            {
                GameManager.Instance.incrementAct1CaughtCounter();
            } 
            else if (gameObject.name.Contains("Random_Thief"))
            {
                GameManager.Instance.incrementAct2CaughtCounter();
            } 
            else if (gameObject.name.Contains("Random_Undead"))
            {
                GameManager.Instance.incrementAct3CaughtCounter();
            }
            spottedScript.StopEffect();
            caughtScript.SetCustomMessage("Caught!");
            caughtScript.TriggerCaughtEffect();
            hasPickpocketed = true;
            chaseSpeed = 0f;
            walkingSpeed = 0f;
            StartCoroutine(ResetAfterCaught());
        }
    }

    private IEnumerator ResetAfterCaught()
    {
        yield return new WaitForSeconds(caughtScript.effectDuration); // Wait for the caught effect to finish
        GameManager.Instance.ResetToLastSave(); // Reset the player
        isPlayerCaught = false; // Allow the caught effect to be triggered again
        FindObjectOfType<SpottedScript>()?.UnlockAddThiefDetection();
        currentDetectionTime = 0f; // Reset detection time
    }


    private void StartChasing()
    {
        currentState = ThiefState.Chasing;
        if (!hasPlayedAlertSound)
        {
            PlayRandomAlertSound(); // Play sound when spotting player
            hasPlayedAlertSound = true;
        }
        
        chaseTimeRemaining = chaseDuration; // Reset the chase timer
    }

    private void StartSearching()
    {
        isSearching = true;
        currentState = ThiefState.Searching;
        StartCoroutine(SearchTimer());
        SearchAroundLastKnownPosition();
    }

    private void SearchAroundLastKnownPosition()
    {
        Vector3 randomPoint;
        NavMeshHit hit;
        for (int i = 0; i < 3; i++) // Try multiple random points within the search radius
        {
            randomPoint = lastKnownPosition + Random.insideUnitSphere * searchRadius;
            if (NavMesh.SamplePosition(randomPoint, out hit, searchRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    private IEnumerator SearchTimer()
    {
        yield return new WaitForSeconds(searchDuration);
        isSearching = false;
        currentState = ThiefState.Patrolling;
        SetNextPatrolPoint();
    }

    private void UpdateDetectionBar()
    {
        // Update the detection bar fill amount
        slider.value = currentDetectionTime / detectionTime;

        // Show the bar only when currentDetectionTime is greater than 0
        slider.transform.parent.gameObject.SetActive(currentDetectionTime > 0);

        // Ensure the bar is always above the NPC
        canvas.transform.position = transform.position + Vector3.up * 3.05f;
        canvas.transform.rotation = Camera.main.transform.rotation;
    }

    // New method to handle gradual depletion of detection bar
    private void DepleteDetectionBar()
    {
        if (!isPlayerInSight && currentDetectionTime > 0f)
        {
            currentDetectionTime -= Time.deltaTime * 0.5f; // Slow depletion rate
            currentDetectionTime = Mathf.Max(currentDetectionTime, 0f); // Prevent negative values

            // Check if the detection bar is fully depleted
            if (currentDetectionTime <= 0f || currentState != ThiefState.Chasing)
            {
                FindObjectOfType<SpottedScript>()?.RemoveThiefDetection();
            }
        }
    }

    public void Interact()
    {
        if (!hasPickpocketed)
        {
            GameManager.Instance.AddPlayerGold(goldAmount);
            hasPickpocketed = true;
            Debug.Log("Has pickpocketed is " + hasPickpocketed);
            audioSource.PlayOneShot(pickpocketSound);

            if (gameObject.name == "Thief")
            {
                instructionManager.GetComponent<InstructionManager>().HidePickpocketInstruction();
                instructionManager.GetComponent<InstructionManager>().ShowReturnInstruction();

                GameManager.Instance.CompleteTask(0);
            }
            
        }
    }

    public float GetInteractDistance()
    {
        return interactDistance;
    }

    public bool HasPickpocketed()
    {
        return hasPickpocketed;
    }

    private void PlayRandomAlertSound()
    {
        if (alertSounds.Length > 0 && audioSource != null)
        {
            // Pick a random sound effect
            AudioClip randomClip = alertSounds[Random.Range(0, alertSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }
}

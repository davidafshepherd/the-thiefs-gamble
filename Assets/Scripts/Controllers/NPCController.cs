using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Transform wanderArea;        // Reference to the wandering area
    public float wanderDelay = 7f;      // Delay between picking new destinations

    private Animator animator;          // Reference to the Animator
    public float maxSpeed = 2.1f;       // NPC's max speed

    private UnityEngine.AI.NavMeshAgent agent;
    private Vector3 wanderAreaMin;
    private Vector3 wanderAreaMax;

    private void Awake()
    {
        // Initialize NavMeshAgent and Animator
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Get the bounds of the wander area
        var bounds = wanderArea.GetComponent<BoxCollider>().bounds;
        wanderAreaMin = bounds.min;
        wanderAreaMax = bounds.max;

        StartWandering();
    }

    private void Update()
    {
        // Update Animator Parameters
        UpdateAnimatorParameters();
    }

    private void StartWandering()
    {
        // Start the wandering coroutine
        StartCoroutine(Wander());
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            // Choose a random position within the wandering area
            Vector3 newPosition = GetRandomPosition();

            // Set the NPC's destination
            agent.SetDestination(newPosition);

            // Wait until the NPC reaches the destination or the delay time passes
            yield return new WaitForSeconds(wanderDelay);
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Generate random x, y, z within the bounds
        float x = Random.Range(wanderAreaMin.x, wanderAreaMax.x);
        float z = Random.Range(wanderAreaMin.z, wanderAreaMax.z);
        float y = wanderAreaMin.y;

        return new Vector3(x, y, z);
    }

    private void UpdateAnimatorParameters()
    {
        // Normalize movement speed for animations
        float normalizedSpeed = agent.velocity.magnitude / maxSpeed;

        // Update Speed parameter in the Animator
        animator.SetFloat("Speed_f", normalizedSpeed);
    }
}

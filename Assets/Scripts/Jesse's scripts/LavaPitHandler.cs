using System.Collections;
using UnityEngine;

public class LavaPitHandler : MonoBehaviour
{
    public CaughtScreenEffect caughtScript; // Reference to the caught effect script
    public SpottedScript spottedScript;
    public string deathMessage = "You Died";     // Custom message for the death screen

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the lava pit is the player
        if (other.CompareTag("Player"))
        {
            HandlePlayerDeath();
        }
    }
    private void HandlePlayerDeath()
    {
        if (caughtScript != null)
        {
            spottedScript.StopEffect();
            caughtScript.SetCustomMessage(deathMessage);
            caughtScript.TriggerCaughtEffect();
            StartCoroutine(ResetAfterCaught());
        }
    }

    private IEnumerator ResetAfterCaught()
    {
        yield return new WaitForSeconds(caughtScript.effectDuration); // Wait for the caught effect to finish
        GameManager.Instance.ResetToLastSave(); // Reset the player
    }
}

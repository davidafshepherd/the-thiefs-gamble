using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Characters")]
    public Transform player;
    public Transform thief;
    public Transform wizard;
    public Transform NPCs;

    [Header("Atmosphere Control")]
    public DayNightManager dayNightManager;
    public AudioSource marketAmbience;
    public AudioSource nightAmbience;
    public AudioSource undeadAmbience;

    void Start()
    {
        //StartCoroutine(DelayPlayerPositionUpdate(5f));
        StartCoroutine(ForcePlayerPosition(0.1f)); // Apply correct position for 5 seconds.

        // Store the current room
        int currentRoom = GameManager.Instance.GetCurrentRoom();

        // Deactivate NPCs, if the player has began the 1st task and hasn't finished the 1st task
        if (currentRoom == 1 && GameManager.Instance.IsTaskOnGoing(0) && !GameManager.Instance.IsTaskComplete(0))
        {
            NPCs.gameObject.SetActive(false);
        }

        // Deactivate the thief, if the player hasn't began the 1st task or has finished the 1st task
        if (currentRoom == 1 && !GameManager.Instance.IsTaskOnGoing(0) || GameManager.Instance.IsTaskComplete(0))
        {
            thief.gameObject.SetActive(false);
        }

        // Deactivate the wizard, if the player hasn't began the 2nd task
        if (currentRoom == 1 && !GameManager.Instance.IsTaskOnGoing(1))
        {
            wizard.gameObject.SetActive(false);
        }

        // Set night time, if the player has began the 1st task and hasn't finished the 1st task
        if (currentRoom == 1 && GameManager.Instance.IsTaskOnGoing(0) && !GameManager.Instance.IsTaskComplete(0))
        {
            dayNightManager.SetNight(0f);
            nightAmbience.Play();
        }
        // Set night time, if the player has reached the cemetery checkpoint
        else if (currentRoom == 1 && GameManager.Instance.IsCheckpointReached(1))
        {
            dayNightManager.SetNight(0f);
            undeadAmbience.Play();
            marketAmbience.Play();
        }
        // Otherwise, set daytime
        else
        {
            dayNightManager.SetDay(0f);
            marketAmbience.Play();
        }
    }

    private IEnumerator DelayPlayerPositionUpdate(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.GetComponent<ThirdPersonController>().enabled = false;
        // Reset the player's position and rotation after the delay
        player.position = GameManager.Instance.GetLocation();
        player.rotation = Quaternion.Euler(GameManager.Instance.GetRotation());
        player.GetComponent<ThirdPersonController>().enabled = true;
    }

    private IEnumerator ForcePlayerPosition(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            player.position = GameManager.Instance.GetLocation();
            player.rotation = Quaternion.Euler(GameManager.Instance.GetRotation());
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame.
        }
    }

}

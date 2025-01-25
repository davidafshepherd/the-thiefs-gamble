using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Meeting Room Door Settings")]
    public Transform meetingRoomDoor;                               // Reference to the meeting room door
    public Vector3 MRDrotationAmount = new Vector3(0, -135, 0);     // Meeting room door's rotation amount
    public float MRDrotationDuration = 1f;                          // Meeting room door's rotation duration

    [Header("Human Castle Door Settings")]
    public Transform humanCastleDoor;                               // Reference to the human castle door
    public Vector3 HCDrotationAmount = new Vector3(0, 135, 0);      // Human castle door's rotation amount
    public float HCDrotationDuration = 1f;                          // Human castle door's rotation duration

    [Header("Human Castle Gate Settings")]
    public Transform humanCastleGate;                               // Reference to the human castle gate
    public Vector3 HCGliftAmount = new Vector3(0, 4.7f, 0);         // Human castle gate's lift amout                               
    public float HCGliftDuration = 3f;                              // Human castle gate's lift duration

    [Header("Undead Castle Door Settings")]
    public Transform undeadCastleDoor;                              // Reference to the undead castle door

    [Header("Throne Room Door Settings")]
    public Transform throneRoomDoor;                                // Reference to the throne room door
    public Vector3 TRDrotationAmount = new Vector3(0, -135, 0);     // Throne room door's rotation amount
    public float TRDrotationDuration = 1f;                          // Throne room door's rotation duration

    [Header("Audio Settings")]
    public AudioClip doorOpening;                                   // Audio clip of the door opening
    public AudioClip doorClosing;                                   // Audio clip of the door closing
    public AudioClip gateOpening;                                   // Audio clip of the gate opening
    public AudioClip gateClosing;                                   // Audio clip of the gate closing
    private AudioSource audioSource;                                // AudioSource to play the door opening and closing

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Store the current room
        int currentRoom = GameManager.Instance.GetCurrentRoom();

        // Open the meeting room door, if the door is meant to already be open
        if (currentRoom == 0 && GameManager.Instance.IsDoorOpen(0)) RotateDoorInstantly(meetingRoomDoor, MRDrotationAmount);

        // Open the human castle door, if the door is meant to already be open
        if (currentRoom == 1 && GameManager.Instance.IsDoorOpen(1)) RotateDoorInstantly(humanCastleDoor, HCDrotationAmount);

        // Open the human castle gate, if the gate is meant to already be open
        if (currentRoom == 1 && GameManager.Instance.IsDoorOpen(2)) LiftGateImmediately(humanCastleGate, HCGliftAmount);

        // Open the undead castle door, if the door is meant to already be open
        if (currentRoom == 1 && GameManager.Instance.IsDoorOpen(3)) undeadCastleDoor.gameObject.SetActive(true);

        // Open the throne room door, if the door is meant to already be open
        if (currentRoom == 2 && GameManager.Instance.IsDoorOpen(4)) RotateDoorInstantly(throneRoomDoor, TRDrotationAmount);
    }

    public void OpenMeetingRoomDoor()
    {
        if (!GameManager.Instance.IsDoorOpen(0) && !GameManager.Instance.IsDoorOpen(1))
        {
            StartCoroutine(RotateDoor(meetingRoomDoor, MRDrotationAmount, MRDrotationDuration));
            audioSource.PlayOneShot(doorOpening);

            GameManager.Instance.OpenDoor(0);
            GameManager.Instance.OpenDoor(1);
        }
    }

    public void CloseMeetingRoomDoor()
    {
        if (GameManager.Instance.IsDoorOpen(0) && GameManager.Instance.IsDoorOpen(1))
        {
            StartCoroutine(RotateDoor(meetingRoomDoor, -MRDrotationAmount, MRDrotationDuration));
            audioSource.PlayOneShot(doorClosing);

            GameManager.Instance.CloseDoor(0);
            GameManager.Instance.CloseDoor(1);
        }
    }

    public void OpenHumanCastleDoor()
    {
        if (!GameManager.Instance.IsDoorOpen(1) && !GameManager.Instance.IsDoorOpen(0))
        {
            StartCoroutine(RotateDoor(humanCastleDoor, HCDrotationAmount, HCDrotationDuration));
            audioSource.PlayOneShot(doorOpening);

            GameManager.Instance.OpenDoor(1);
            GameManager.Instance.OpenDoor(0);
        }
    }

    public void CloseHumanCastleDoor()
    {
        if (GameManager.Instance.IsDoorOpen(1) && GameManager.Instance.IsDoorOpen(0))
        {
            StartCoroutine(RotateDoor(humanCastleDoor, -HCDrotationAmount, HCDrotationDuration));
            audioSource.PlayOneShot(doorClosing);

            GameManager.Instance.CloseDoor(1);
            GameManager.Instance.CloseDoor(0);
        }
    }

    public void OpenHumanCastleGate()
    {
        StartCoroutine(LiftGate(humanCastleGate, HCGliftAmount, HCGliftDuration));
        audioSource.PlayOneShot(gateOpening);
        GameManager.Instance.OpenDoor(2);
    }

    public void CloseHumanCastleGate()
    {
        StartCoroutine(LiftGate(humanCastleGate, -HCGliftAmount, HCGliftDuration));
        audioSource.PlayOneShot(gateClosing);
        GameManager.Instance.CloseDoor(2);
    }

    public void OpenUndeadCastleDoor()
    {
        if (!GameManager.Instance.IsDoorOpen(3) && !GameManager.Instance.IsDoorOpen(4))
        {
            undeadCastleDoor.gameObject.SetActive(true);

            GameManager.Instance.OpenDoor(3);
            GameManager.Instance.OpenDoor(4);
        }
    }

    public void CloseUndeadCastleDoor()
    {
        if (GameManager.Instance.IsDoorOpen(3) && GameManager.Instance.IsDoorOpen(4))
        {
            undeadCastleDoor.gameObject.SetActive(false);

            GameManager.Instance.CloseDoor(3);
            GameManager.Instance.CloseDoor(4);
        }
    }

    public void OpenThroneRoomDoor()
    {
        if (!GameManager.Instance.IsDoorOpen(4) && !GameManager.Instance.IsDoorOpen(3))
        {
            StartCoroutine(RotateDoor(throneRoomDoor, TRDrotationAmount, TRDrotationDuration));
            audioSource.PlayOneShot(doorOpening);

            GameManager.Instance.OpenDoor(4);
            GameManager.Instance.OpenDoor(3);
        }
    }

    public void CloseThroneRoomDoor()
    {
        if (GameManager.Instance.IsDoorOpen(4) && GameManager.Instance.IsDoorOpen(3))
        {
            StartCoroutine(RotateDoor(throneRoomDoor, -TRDrotationAmount, TRDrotationDuration));
            audioSource.PlayOneShot(doorClosing);

            GameManager.Instance.CloseDoor(4);
            GameManager.Instance.CloseDoor(3);
        }
    }

    private IEnumerator RotateDoor(Transform door, Vector3 rotationAmount, float rotationDuration)
    {
        Quaternion startRotation = door.rotation;                                          // Initial rotation
        Quaternion targetRotation = Quaternion.Euler(door.eulerAngles + rotationAmount);   // Target rotation
        
        // Time elapsed
        float timeElapsed = 0f;                                                            
        
        while (timeElapsed < rotationDuration)
        {
            // Interpolate rotation over time
            door.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / rotationDuration);

            timeElapsed += Time.deltaTime;      // Increment elapsed time
            yield return null;                  // Wait for the next frame
        }
    }

    private void RotateDoorInstantly(Transform door, Vector3 rotationAmount)
    {
        door.rotation = Quaternion.Euler(door.eulerAngles + rotationAmount);
    }

    private IEnumerator LiftGate(Transform gate, Vector3 liftAmount, float liftDuration)
    {
        Vector3 startPosition = gate.position;                  // Initial position    
        Vector3 targetPosition = gate.position + liftAmount;    // Target position

        // Time elapsed
        float timeElapsed = 0f;

        while (timeElapsed < liftDuration)
        {
            // Interpolate position over time
            gate.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / liftDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void LiftGateImmediately(Transform gate, Vector3 liftAmount)
    {
        gate.position = gate.position + liftAmount;
    }
}

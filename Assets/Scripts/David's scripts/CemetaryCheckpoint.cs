using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemetaryCheckpoint : MonoBehaviour
{
    public Transform dayNightManager;
    public Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            dayNightManager.GetComponent<DayNightManager>().SetNight(3f);
        }
    }
}

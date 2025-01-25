using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickpocketableNPC : MonoBehaviour, MyInteractable
{
    public float interactDistance = 15f;
    public int goldAmount = 30;
    private bool hasPickpocketed;

    // Start is called before the first frame update
    void Start()
    {
        hasPickpocketed = false;
    }

    public void Interact()
    {
        if (!hasPickpocketed)
        {
            GameManager.Instance.AddPlayerGold(goldAmount);
            hasPickpocketed = true;
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
    
}

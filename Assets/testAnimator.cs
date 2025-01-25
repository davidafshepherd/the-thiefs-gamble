using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnimator : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = rb.velocity.magnitude;
        Debug.Log("Player Speed: " + speed);
        animator.SetFloat("Speed", speed);
    }


}

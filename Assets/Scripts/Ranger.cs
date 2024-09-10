using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ranger : Interactable
{
    private PlayerCamera playerCamera;
    private Animator animator;
    private NavMeshAgent navmeshAgent;


    void Start()
    {
        playerCamera = FindObjectOfType<PlayerCamera>();
        animator = GetComponent<Animator>();
        navmeshAgent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        animator.SetFloat("MoveSpeed", navmeshAgent.velocity.magnitude);
        Debug.Log(navmeshAgent.velocity.magnitude);
    }

    public void SelectObject()
    {
        playerCamera.SelectObject(this);
    }
}

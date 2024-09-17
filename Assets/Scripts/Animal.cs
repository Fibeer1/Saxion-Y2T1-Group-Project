using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : Wanderer
{
    public bool isDead = false;
    private GameObject trapTriggered;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shouldKeepTrackOfTrails = true;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
        HandleWandering();
        HandleTrailLeaving();
    }

    public IEnumerator Die()
    {
        if (isDead)
        {
            yield break;
        }

        isDead = true;
        navMeshAgent.enabled = false;
        for (int i = 0; i < 20; i++)
        {
            transform.Rotate(0, 0, 5);
            yield return new WaitForSeconds(0.01f);
        }
        GameManager.animals.Remove(this);
        yield return new WaitForSeconds(1.5f);
        if (trapTriggered != null)
        {
            Destroy(trapTriggered);
        }
        FOVDebug.FindFOVEntities();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trap")
        {
            trapTriggered = other.gameObject;
            StartCoroutine(Die());
        }
    }
}

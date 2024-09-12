using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : Wanderer
{
    public bool isDead = false;

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
        HandleWandering();
        HandleTrailLeaving();
    }     

    public IEnumerator Die()
    {
        navMeshAgent.enabled = false;
        for (int i = 0; i < 20; i++)
        {
            transform.Rotate(0, 0, 5);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}

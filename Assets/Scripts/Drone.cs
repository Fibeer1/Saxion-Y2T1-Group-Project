using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Drone : Interactable
{
    private NavMeshAgent navMeshAgent;
    public Transform target;
    [SerializeField] private float wanderTimer;
    private float wanderTime = 10;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        wanderTimer = wanderTime;
    }

    private void Update()
    {
        HandleWandering();
    }

    private void HandleWandering()
    {
        if (navMeshAgent.velocity.magnitude > 1)
        {
            return;
        }

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            Vector3 randDirection = Random.insideUnitSphere * 35;

            randDirection += transform.position;

            PickTarget(randDirection);
        }
    }

    public void PickTarget(Vector3 targetPosition)
    {
        wanderTimer = wanderTime;

        NavMeshHit navHit;

        NavMesh.SamplePosition(targetPosition, out navHit, 35, -1);

        navMeshAgent.SetDestination(navHit.position);
    }
}

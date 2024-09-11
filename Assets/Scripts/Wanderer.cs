using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wanderer : Lifeform
{
    [SerializeField] private GameObject trailNode;
    public List<TrailNode> trail;
    public float trailTimer = 1;
    public float trailTimerDuration = 1;
    private bool shouldRotateFootprint = false;
    [SerializeField] private protected float wanderTimer = 10;
    private protected NavMeshAgent navMeshAgent;
    private protected Animator animator;
    private bool shouldWander = true;
    private protected bool shouldKeepTrackOfTrails = false;

    private protected void HandleWandering()
    {
        if (!shouldWander)
        {
            return;
        }
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            PickRandomPosition(25);
            wanderTimer = 10;
            if (GetComponent<Animal>() != null)
            {
                GetComponent<Animal>().trailTimer = 0;
            }            
        }
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
    }

    private protected void HandleTrailLeaving()
    {
        if (navMeshAgent.velocity.magnitude < 0.5f)
        {
            return;
        }

        trailTimer -= Time.deltaTime;
        if (trailTimer <= 0)
        {
            trailTimer = trailTimerDuration;

            TrailNode currentTrailNode = Instantiate(trailNode, transform.position, transform.rotation).GetComponent<TrailNode>();
            if (shouldRotateFootprint)
            {
                currentTrailNode.transform.Rotate(0, 0, 180);
            }
            shouldRotateFootprint = !shouldRotateFootprint;
            if (!shouldKeepTrackOfTrails)
            {
                return;
            }
            currentTrailNode.animal = GetComponent<Animal>();
            trail.Add(currentTrailNode.GetComponent<TrailNode>());
            if (trail.IndexOf(currentTrailNode) > 0)
            {
                //If there are other nodes before this one, add it to the last spawned node as its next node
                trail[trail.IndexOf(currentTrailNode) - 1].nextTrailNode = currentTrailNode;
            }
        }
    }

    private protected void PickRandomPosition(float distance)
    {
        Vector3 randDirection = Random.insideUnitSphere * distance;

        randDirection += transform.position;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, distance, -1);

        navMeshAgent.SetDestination(navHit.position);
    }
}

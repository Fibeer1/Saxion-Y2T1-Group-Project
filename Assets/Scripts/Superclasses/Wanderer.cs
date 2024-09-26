using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wanderer : Entity
{
    [SerializeField] private protected GameObject trailNodePrefab;
    public List<TrailNode> trail;
    [SerializeField] private Transform[] feet;
    [SerializeField] private protected float wanderTimer = 10;
    [SerializeField] private protected float wanderTimerDuration;
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
        if (navMeshAgent.velocity.magnitude <= 1)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                PickRandomPosition(25);
                wanderTimer = wanderTimerDuration;
            }
        }
    }

    protected void Footstep(int footstepIndex)
    {
        float reverseFootstepValue = 180;
        if (GetComponent<Animal>() != null)
        {
            reverseFootstepValue = 0;
        }
        TrailNode currentTrailNode = Instantiate(trailNodePrefab, feet[footstepIndex].position, transform.rotation).GetComponent<TrailNode>();
        currentTrailNode.transform.position = new Vector3(currentTrailNode.transform.position.x, transform.position.y + 0.05f, currentTrailNode.transform.position.z);
        if (footstepIndex == 0)
        {
            currentTrailNode.transform.Rotate(0, 0, reverseFootstepValue);
            Destroy(currentTrailNode);
        }
        FOVDebug.FindFOVEntities();
        if (!shouldKeepTrackOfTrails || footstepIndex == 0)
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

    private protected void PickRandomPosition(float distance)
    {
        Vector3 randDirection = Random.insideUnitSphere * distance;

        randDirection += transform.position;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, distance, -1);

        navMeshAgent.SetDestination(navHit.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Poacher : Wanderer
{
    private bool followingTrail = false;
    private Animal targetAnimal;
    private bool duringAnimation = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shouldKeepTrackOfTrails = false;
    }
    
    private void Update()
    {
        if (duringAnimation)
        {
            return;
        }

        if (!followingTrail)
        {
            HandleWandering();
        }
        else
        {
            HandleTrailFollowing();
        }
    }

    void Footstep(int footstepIndex)
    {

        TrailNode currentTrailNode = Instantiate(trailNode, transform.position, transform.rotation).GetComponent<TrailNode>();
        if (footstepIndex == 0)
        {
            currentTrailNode.transform.Rotate(0, 0, 180);
        }
    }

    private void HandleTrailFollowing()
    {
        float distance = Vector3.Distance(transform.position, targetAnimal.transform.position);
        Debug.Log(distance);
        if (distance < 5)
        {
            StartCoroutine(ShootAnimal());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TrailNode animalFootprint = other.GetComponent<TrailNode>();
        if (animalFootprint != null)
        {
            followingTrail = true;
            if (animalFootprint.animal == null)
            {
                followingTrail = false;
                return;
            }
            targetAnimal = animalFootprint.animal;
            if (animalFootprint.nextTrailNode != null)
            {
                //If there is a next node referenced in the hit node, go to that
                navMeshAgent.SetDestination(animalFootprint.nextTrailNode.transform.position);
            }
            else
            {
                //If not, go in the trail node's general direction
                navMeshAgent.SetDestination(animalFootprint.transform.position + animalFootprint.transform.forward * 2.5f);
            }
        }
    }

    private IEnumerator ShootAnimal()
    {
        duringAnimation = true;
        StartCoroutine(TurnTowardsTarget(targetAnimal.transform, 0.5f));
        navMeshAgent.destination = transform.position;
        animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);
        yield return new WaitForSeconds(1);
        targetAnimal.StartCoroutine(targetAnimal.Die());
        duringAnimation = false;
        followingTrail = false;
    }
}

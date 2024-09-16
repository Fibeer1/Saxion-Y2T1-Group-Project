using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Poacher : Wanderer
{
    [SerializeField] private bool followingTrail = false;
    private Animal targetAnimal;
    [SerializeField] private Ranger targetRanger;
    private bool duringAnimation = false;
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private float trapTimer;
    private float defaultSpeed;
    private float runSpeed; //Must be lower than the rangers' speed

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        shouldKeepTrackOfTrails = false;
        trapTimer = 60;
        defaultSpeed = navMeshAgent.speed;
        runSpeed = defaultSpeed + 6.5f;
    }
    
    private void Update()
    {
        if (duringAnimation)
        {
            return;
        }
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
        HandleRunningFromRangers();
        if (targetRanger != null)
        {
            return;
        }
        if (!followingTrail)
        {
            HandleTraps();
            HandleWandering();
        }
        else
        {
            HandleTrailFollowing();
        }
    }

    void Footstep(int footstepIndex)
    {

        TrailNode currentTrailNode = Instantiate(trailNodePrefab, transform.position, transform.rotation).GetComponent<TrailNode>();
        if (footstepIndex == 0)
        {
            currentTrailNode.transform.Rotate(0, 0, 180);
        }
    }

    private void HandleTraps()
    {
        trapTimer -= Time.deltaTime;
        if (trapTimer <= 0)
        {
            trapTimer = 60;
            Instantiate(trapPrefab, transform.position, transform.rotation);
        }
    }

    public void TargetRanger(Ranger ranger)
    {
        targetAnimal = null;
        targetRanger = ranger;
        followingTrail = false;
        navMeshAgent.speed = runSpeed;
    }

    private void HandleRunningFromRangers()
    {
        if (targetRanger == null)
        {
            return;
        }
        Vector3 difference = transform.position - (targetRanger.transform.position - transform.position) * 2;
        
        NavMeshHit navHit;

        NavMesh.SamplePosition(difference, out navHit, 5, -1);

        navMeshAgent.SetDestination(navHit.position);

        float distance = Vector3.Distance(transform.position, targetRanger.transform.position);
        if (distance > 15)
        {
            navMeshAgent.speed = defaultSpeed;
            targetRanger = null;
            navMeshAgent.destination = transform.position;
        }
    }

    private void HandleTrailFollowing()
    {
        if (targetAnimal == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, targetAnimal.transform.position);
        Debug.Log(distance);
        if (distance < 10)
        {
            StartCoroutine(ShootAnimal(targetAnimal));
        }
    }

    public void FollowAnimalTrail(TrailNode animalFootprint)
    {
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

    public IEnumerator ShootAnimal(Animal animal)
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

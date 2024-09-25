using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Drone : Interactable
{
    private NavMeshAgent navMeshAgent;
    public Transform target;
    [SerializeField] private GameObject targetCirclePrefab;
    public GameObject currentTargetCircle;
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
        MoveTowardsTarget();
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

            PickTarget(null, randDirection);
        }
    }

    public void PickTarget(Transform pTarget, Vector3 targetPosition)
    {
        if (pTarget != null)
        {           
            target = pTarget;
            targetPosition = pTarget.position;
            currentTargetCircle = Instantiate(targetCirclePrefab, target.position, Quaternion.identity, target);
        }

        wanderTimer = wanderTime;

        NavMeshHit navHit;

        NavMesh.SamplePosition(targetPosition, out navHit, 35, -1);

        navMeshAgent.SetDestination(navHit.position);
    }

    private void MoveTowardsTarget()
    {
        if (target == null)
        {
            return;
        }
        NavMeshHit navHit;

        NavMesh.SamplePosition(target.position, out navHit, 35, -1);

        navMeshAgent.SetDestination(navHit.position);

        if (navMeshAgent.velocity.magnitude < 1 && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            if (currentTargetCircle != null)
            {
                Destroy(currentTargetCircle);
                currentTargetCircle = null;
            }
            if (target.GetComponent<Ranger>() != null)
            {
                GameObject targetItem = Instantiate(GetComponent<Equipment>().itemUIPrefab, FindObjectOfType<Canvas>().transform);
                player.SelectObject(target.GetComponent<Interactable>());
                FindObjectOfType<RangerBackground>().AddItemToInventory(targetItem);
                Destroy(gameObject);
            }
            target = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ranger : Interactable
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public Transform target;
    public float defaultSpeed;
    public float runSpeed;
    public bool duringAnimation = false;


    private void Start()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        defaultSpeed = navMeshAgent.speed;
        runSpeed = defaultSpeed + 7.5f;
    }


    private void Update()
    {
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
        if (target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    public void SelectTarget(Transform pTarget)
    {
        if (target != null)
        {
            DeselectTarget();
        }
        target = pTarget;
        target.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", new Color(0.5f, 0.5f, 0));
    }

    public void DeselectTarget()
    {
        if (target != null)
        {
            target.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", new Color(0, 0, 0));
            target = null;
        }        
    }

    void Footstep()
    {
        //Error prevention, must remove when the real assets are imported
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Poacher>() != null && !duringAnimation)
        {
            duringAnimation = true;
            Poacher poacherHit = other.GetComponent<Poacher>();
            poacherHit.GetComponent<NavMeshAgent>().destination = poacherHit.transform.position;
            DeselectTarget();
            StartCoroutine(poacherHit.TurnTowardsTarget(transform, 0.5f));
            StartCoroutine(TurnTowardsTarget(poacherHit.transform, 0.5f));           
            navMeshAgent.destination = transform.position;
            animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);           
            StartCoroutine(CatchPoacher(poacherHit));
        }
    }

    private IEnumerator CatchPoacher(Poacher poacher)
    {
        yield return new WaitForSeconds(1);
        Destroy(poacher.gameObject);
        duringAnimation = false;
        
    }
}

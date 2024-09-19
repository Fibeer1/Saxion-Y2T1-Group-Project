using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ranger : Interactable
{   
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public Transform target;
    [SerializeField] private GameObject targetCirclePrefab;
    public GameObject currentTargetCircle;
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
        currentTargetCircle = Instantiate(targetCirclePrefab, target.position, Quaternion.identity, target);
    }

    public void DeselectTarget()
    {
        if (target != null)
        {
            Destroy(currentTargetCircle);
            currentTargetCircle = null;
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
            poacherHit.GetComponent<NavMeshAgent>().enabled = false;
            poacherHit.duringAnimation = true;
            DeselectTarget();
            StartCoroutine(poacherHit.TurnTowardsTarget(transform, 0.5f));
            StartCoroutine(TurnTowardsTarget(poacherHit.transform, 0.5f));           
            navMeshAgent.destination = transform.position;
            animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);
            GameManager.poachers.Remove(poacherHit);
            StartCoroutine(RemoveObject(poacherHit.gameObject));
            FindObjectOfType<GameManager>().HandleMoneyChange("Poacher arrested!\nMoney received: " +
            poacherHit.poacherValue + "$", poacherHit.poacherValue);
        }
        else if (other.tag == "Trap" && !duringAnimation)
        {
            GameObject trap = other.gameObject;
            duringAnimation = true;
            DeselectTarget();
            StartCoroutine(TurnTowardsTarget(trap.transform, 0.5f));
            navMeshAgent.destination = transform.position;
            animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);
            StartCoroutine(RemoveObject(trap));
            FindObjectOfType<GameManager>().HandleMoneyChange("Trap dismantled!\nMoney received: " +
            GameManager.trapValue + "$", GameManager.trapValue);
        }
    }

    private IEnumerator RemoveObject(GameObject gameobject)
    {
        yield return new WaitForSeconds(1);
        Destroy(gameobject);
        duringAnimation = false;
        FOVDebug.FindFOVEntities();
        
    }
}

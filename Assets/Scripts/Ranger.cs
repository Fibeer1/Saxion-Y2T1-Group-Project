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
    public string actionToPerform;
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
        if (duringAnimation)
        {
            return;
        }
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);

        if (target != null)
        {
            navMeshAgent.SetDestination(target.position);
            if (Vector3.Distance(transform.position, target.position) < 3)
            {
                navMeshAgent.destination = transform.position;
                if (actionToPerform == "PlaceObject")
                {
                    StartCoroutine(PlaceItem());
                }
                else if (actionToPerform == "PickUpEquipment")
                {
                    PickUpItem();
                }
                else if (actionToPerform == "ChasePoacher")
                {
                    CatchPoacher();
                }
                else if (actionToPerform == "DisarmTrap")
                {
                    DisarmTrap();
                }
            }
        }
    }

    public void SelectTarget(Transform pTarget, string pActionToPerform, bool shouldSpawnTargetCircle = true)
    {
        if (target != null)
        {
            DeselectTarget();
        }
        actionToPerform = pActionToPerform;
        target = pTarget;
        if (actionToPerform == "ChasePoacher")
        {
            navMeshAgent.speed = runSpeed;
        }
        else
        {
            navMeshAgent.speed = defaultSpeed;
        }
        if (shouldSpawnTargetCircle)
        {
            currentTargetCircle = Instantiate(targetCirclePrefab, target.position, Quaternion.identity, target);
        }       
    }

    public void DeselectTarget()
    {
        if (target != null)
        {
            if (currentTargetCircle != null)
            {
                Destroy(currentTargetCircle);
                currentTargetCircle = null;
            }            
            target = null;
            actionToPerform = null;
        }        
    }

    void Footstep()
    {
        //Error prevention, must remove when the real assets are imported
    }

    private void PickUpItem()
    {
        if (duringAnimation)
        {
            return;
        }
        duringAnimation = true;
        Transform tempTarget = target;
        DeselectTarget();
        StartCoroutine(TurnTowardsTarget(tempTarget, 0.5f));
        animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);
        GameObject targetItem = Instantiate(tempTarget.GetComponent<Equipment>().itemUIPrefab, FindObjectOfType<Canvas>().transform);
        FindObjectOfType<RangerBackground>().AddItemToInventory(targetItem);
        FieldOfViewTrigger sensorScript = tempTarget.GetComponent<FieldOfViewTrigger>();
        if (sensorScript != null && sensorScript.currentMarker != null)
        {
            Destroy(sensorScript.currentMarker);
        }
        StartCoroutine(RemoveObject(tempTarget.gameObject));
    }

    private IEnumerator PlaceItem()
    {
        if (duringAnimation)
        {
            yield break;
        }
        duringAnimation = true;
        DeselectTarget();
        animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);
        FindObjectOfType<RangerBackground>().RemoveItemFromInventory(player.currentObjectToPlaceSprite);
        yield return new WaitForSeconds(1);
        player.DeselectObjectToPlace();
        duringAnimation = false;
    }

    private void CatchPoacher()
    {
        if (duringAnimation)
        {
            return;
        }
        duringAnimation = true;
        Poacher poacherHit = target.GetComponent<Poacher>();
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

    private void DisarmTrap()
    {
        if (duringAnimation)
        {
            return;
        }
        GameObject trap = target.gameObject;
        duringAnimation = true;
        DeselectTarget();
        StartCoroutine(TurnTowardsTarget(trap.transform, 0.5f));
        navMeshAgent.destination = transform.position;
        animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);
        StartCoroutine(RemoveObject(trap));
        FindObjectOfType<GameManager>().HandleMoneyChange("Trap dismantled!\nMoney received: " +
        GameManager.trapValue + "$", GameManager.trapValue);
    }

    private IEnumerator RemoveObject(GameObject gameobject)
    {
        yield return new WaitForSeconds(1);
        Destroy(gameobject);
        duringAnimation = false;
        FOVDebug.FindFOVEntities();
        
    }
}

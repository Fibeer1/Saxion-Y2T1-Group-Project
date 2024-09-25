using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ranger : Interactable
{   
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public Transform target;
    public Vector3 targetPosition;
    [SerializeField] private GameObject targetCirclePrefab;
    public GameObject currentTargetCircle;
    private LineRenderer pathLine;
    public string actionToPerform;
    public float fatigue = 0;
    private float fatigueSpeed;
    public float defaultSpeed;
    public float runSpeed;   
    public bool duringAnimation = false;


    private void Start()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        pathLine = GetComponent<LineRenderer>();
        fatigue = 0;
        defaultSpeed = navMeshAgent.speed;
        runSpeed = defaultSpeed + 7.5f;
        fatigueSpeed = defaultSpeed - 2;
        pathLine.startWidth = 0.15f;
        pathLine.endWidth = 0.15f;
        pathLine.positionCount = 0;
    }


    private void Update()
    {
        if (duringAnimation)
        {
            return;
        }
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);

        if (navMeshAgent.velocity.magnitude > 1)
        {
            fatigue += Time.deltaTime / 100;
            if (fatigue >= 1)
            {
                navMeshAgent.speed = fatigueSpeed;
            }
        }
        if (navMeshAgent.speed == fatigueSpeed)
        {
            if (navMeshAgent.velocity.magnitude > 1)
            {
                fatigue -= Time.deltaTime / 100;
            }
            else
            {
                fatigue -= Time.deltaTime / 25;
            }
            if (fatigue <= 0 && navMeshAgent.speed == fatigueSpeed)
            {
                navMeshAgent.speed = defaultSpeed;
                fatigue = 0;
            }
        }

        NavMeshHit navHit;

        NavMesh.SamplePosition(targetPosition, out navHit, 5, -1);

        navMeshAgent.SetDestination(navHit.position);

        DrawPath();
        

        if (target != null && Vector3.Distance(transform.position, navHit.position) < 3)
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

    private void DrawPath()
    {
        pathLine.positionCount = navMeshAgent.path.corners.Length;
        pathLine.SetPosition(0, Vector3.zero);

        if (navMeshAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 0; i < navMeshAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(navMeshAgent.path.corners[i].x, navMeshAgent.path.corners[i].y, navMeshAgent.path.corners[i].z);
            pathLine.SetPosition(i, pointPosition);
        }

        pathLine.SetPositions(navMeshAgent.path.corners);
    }

    public void SelectTarget(Transform pTarget, Vector3 pTargetPos, string pActionToPerform, bool shouldSpawnTargetCircle = true)
    {
        if (target != null)
        {
            DeselectTarget();
        }
        actionToPerform = pActionToPerform;
        target = pTarget;
        targetPosition = pTargetPos;

        if (actionToPerform == "ChasePoacher")
        {
            navMeshAgent.speed = runSpeed;
        }
        else
        {
            navMeshAgent.speed = defaultSpeed;
            pathLine.positionCount = navMeshAgent.path.corners.Length;

            for (var i = 1; i < navMeshAgent.path.corners.Length; i++)
            {
                //pathLine.SetPosition(i, navMeshAgent.path.corners[i]);
            }
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
            targetPosition = transform.position;
            actionToPerform = null;
        }        
    }

    void Footstep()
    {
        //Error prevention, must remove when the real assets are imported
    }

    private void PickUpItem()
    {
        if (duringAnimation || target.GetComponent<Equipment>().isBeingPickedUp)
        {
            return;
        }
        duringAnimation = true;
        Transform tempTarget = target;
        tempTarget.GetComponent<Equipment>().isBeingPickedUp = true;
        DeselectTarget();
        StartCoroutine(TurnTowardsTarget(tempTarget, 0.5f));
        animator.CrossFadeInFixedTime("Ranger|Jumpscare", 0.25f);
        GameObject targetItem = Instantiate(tempTarget.GetComponent<Equipment>().itemUIPrefab, FindObjectOfType<Canvas>().transform);
        FindObjectOfType<RangerBackground>().AddItemToInventory(targetItem);
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
        if (duringAnimation || target.GetComponent<Poacher>().isBeingArrested)
        {
            return;
        }
        duringAnimation = true;
        target.GetComponent<Poacher>().isBeingArrested = true;
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
        trap.GetComponent<Trap>().PlayAnimation("Disarm");
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

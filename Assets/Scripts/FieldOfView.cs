using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;
    public Transform origin;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return delay;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        visibleTargets.Clear();
        Collider[] rangeChecks = Physics.OverlapSphere(origin.position, radius, targetMask);        

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - origin.position).normalized;

            if (Vector3.Angle(origin.position, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(origin.position, target.position);
                
                if (!Physics.Raycast(origin.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    visibleTargets.Add(target);

                    target.GetComponentInChildren<MeshRenderer>().enabled = true;
                }
                else
                {
                    target.GetComponentInChildren<Renderer>().enabled = false;
                }
            }
            else
            {
                //canSeeCharacter = false;
                target.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
        //else if (canSeeCharacter)
        //{
        //    canSeeCharacter = false;
        //}

    }
}

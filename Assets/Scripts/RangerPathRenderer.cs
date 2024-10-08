using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangerPathRenderer : MonoBehaviour
{
    private LineRenderer line;
    private Ranger ranger;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        ranger = GetComponent<Ranger>();
        StartCoroutine(GetPath());
    }

    public IEnumerator GetPath()
    {
        line.SetPosition(0, Vector3.zero); //set the line's origin

        //yield return new WaitForEndOfFrame(); //wait for the path to generate
        yield return null;

        DrawPath(ranger.GetComponent<NavMeshAgent>().path);

    }

    private void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
        {
            Debug.Log("Path too short");
            return;
        }
            

        line.positionCount = path.corners.Length;

        for (var i = 1; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }
}

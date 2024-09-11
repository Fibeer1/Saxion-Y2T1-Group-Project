using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifeform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator TurnTowardsTarget(Transform pTarget, float duration)
    {
        float elapsedTime = 0;

        Vector3 difference = pTarget.position - transform.position;
        float rotationY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
        Quaternion intendedRotation = Quaternion.Euler(0, rotationY, 0);

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, intendedRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantAnimation : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("AnimSpeed", Mathf.Abs(rb.velocity.z));
    }
}

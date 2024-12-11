using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTransition : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float totalTransitionTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("fade in");
    }

    // Update is called once per frame
    void Update()
    {
        totalTransitionTime -= Time.deltaTime;
        if (totalTransitionTime <= 0)
        {
            animator.Play("fade out");
        }
    }
}

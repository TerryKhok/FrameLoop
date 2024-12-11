using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTransition : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float totalAnimationTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator.Play("fade in");
    }

    // Update is called once per frame
    void Update()
    {
        totalAnimationTime -= Time.deltaTime;
        if (totalAnimationTime <= 0)
        {
            animator.Play("fade out");
        }
    }
}

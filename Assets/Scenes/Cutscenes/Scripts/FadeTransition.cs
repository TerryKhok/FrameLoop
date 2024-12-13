using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeTransition : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float totalAnimationTime = 10.0f;

    [SerializeField]
    private float fadeInTime = 1f;
    [SerializeField]
    private float fadeOutTime = 1f;

    private bool isFadingOut = false;

    // Start is called before the first frame update
    void Start()
    {
        animator.speed = fadeInTime;
        animator.Play("fade in");
    }

    // Update is called once per frame
    void Update()
    {
        totalAnimationTime -= Time.deltaTime;
        if (totalAnimationTime <= 0 && !isFadingOut)
        {
            isFadingOut = true;
            animator.speed = fadeOutTime;
            animator.Play("fade out");
        }

        if (isFadingOut)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("fade out") && stateInfo.normalizedTime >= 1.0f)
            {
                SceneManager.LoadScene("lvl 1");
            }
        }
    }
}

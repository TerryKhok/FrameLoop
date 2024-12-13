using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Rocketlanding : MonoBehaviour
{
    [SerializeField]
    private float rocketPositionX;
    [SerializeField]
    private float rocketPositionY;

    [SerializeField]
    private float animationTime;

    private float rocketDestinationX;
    private float rocketDestinationY;

    private float elapsedTime;
    bool isLanding = false;
    bool isOpened = false;
    bool hasLanded = false;

    [SerializeField]
    private GameObject rocketParticles;
    [SerializeField]
    private float shakeAmount = 10f;
    [SerializeField]
    private float shakeIntensity = 1f;

    private Animator animator;
    [SerializeField]
    private float landingTime;
    [SerializeField]
    private float openTime;

    bool rocketSoundPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        rocketDestinationX = this.GetComponent<Transform>().position.x;
        rocketDestinationY = this.GetComponent<Transform>().position.y;

        rocketParticles.SetActive(true);

        this.GetComponent<Transform>().position = new Vector3(rocketPositionX, rocketPositionY, 0);

        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= animationTime)
        {
            AudioManager.instance.Stop("Rocket");

            if (!hasLanded)
            {
                AudioManager.instance.Play("Box Landing");
            }

            this.GetComponent<Transform>().position = new Vector3(rocketDestinationX, rocketDestinationY, 0);
            rocketParticles.SetActive(false);
            hasLanded = true;

            if (!isOpened && elapsedTime >= openTime)
            {
                animator.Play("rocket open");
                AudioManager.instance.Play("DoorOpen");
                isOpened = true;
            }
        }
        else if (!hasLanded)
        {
            if(!rocketSoundPlayed)
            {
                rocketSoundPlayed = true;
                AudioManager.instance.Play("Rocket");
            }

            if (!isLanding && elapsedTime >= landingTime)
            {
                animator.Play("rocket landing");
                isLanding = true;
            }

            float t = elapsedTime / animationTime;

            //shaking
            float offsetX = Mathf.PerlinNoise(Time.time * shakeAmount, 0f) * 2f - 1f;
            float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeAmount) * 2f - 1f;

            //set pos
            Vector3 lerpedPosition = new Vector3(Mathf.Lerp(rocketPositionX, rocketDestinationX, t), Mathf.Lerp(rocketPositionY, rocketDestinationY, t), 0);
            this.GetComponent<Transform>().position = lerpedPosition + (new Vector3(offsetX, offsetY, 0) * shakeIntensity);
        }
    }
}

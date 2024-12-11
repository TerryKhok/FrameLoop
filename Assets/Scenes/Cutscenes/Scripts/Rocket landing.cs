using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

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

    [SerializeField]
    private GameObject rocketParticles;
    [SerializeField]
    private float shakeAmount = 10f;
    [SerializeField]
    private float shakeIntensity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rocketDestinationX = this.GetComponent<Transform>().position.x;
        rocketDestinationY = this.GetComponent<Transform>().position.y;

        rocketParticles.SetActive(true);

        this.GetComponent<Transform>().position = new Vector3(rocketPositionX, rocketPositionY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= animationTime)
        {
            this.GetComponent<Transform>().position = new Vector3(rocketDestinationX, rocketDestinationY, 0);
            rocketParticles.SetActive(false);
        }
        else
        {
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

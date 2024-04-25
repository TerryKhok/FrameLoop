using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class ParticleFrameScript : MonoBehaviour
{
    [SerializeField]
    private Transform framePos;
    [SerializeField]
    private Transform particleFramePos;

    [SerializeField, Tooltip("Inactive top")]
    private ParticleSystem ps1; //top
    [SerializeField, Tooltip("Inactive bottom")]
    private ParticleSystem ps2; //bottom
    [SerializeField, Tooltip("Inactive left")]
    private ParticleSystem ps3; //left
    [SerializeField, Tooltip("Inactive right")]
    private ParticleSystem ps4; //right

    ParticleSystem.Particle[] particles;

    private float elapsedTime;
    [SerializeField]
    private float lerpSpeed = 15f;

    [SerializeField]
    private GameObject originalFrame; //non particle frame
    private FrameLoop frameLoop; //script
    [SerializeField]
    private GameObject player;

    //default values
    float particleStartingSpeed = 9.19f;
    float particleRateOverTime = 13.39f;

    //modifier values
    float updateStartingSpeed = 0f;
    float updateRate = 50f;

    bool spawnFlag = false;

    [SerializeField]
    private Material mat;
    private Color matColInactive;
    private Color matColActive;
    [SerializeField]
    private float frameTransparency = 0.2f;

    [SerializeField]
    private GameObject frameClick;
    [SerializeField, Tooltip("Inactive top")]
    private ParticleSystem burst1; //top
    [SerializeField, Tooltip("Inactive bottom")]
    private ParticleSystem burst2; //bottom
    [SerializeField, Tooltip("Inactive left")]
    private ParticleSystem burst3; //left
    [SerializeField, Tooltip("Inactive right")]
    private ParticleSystem burst4; //right

    bool burstFlag = false;

    float frameScale = 100f;
    [SerializeField]
    private float frameScaleFactor = 10f;
    private float currentFrameSize = 0f;

    private void Start()
    {
        frameLoop = originalFrame.GetComponent<FrameLoop>();
        transform.position = player.transform.position;
    }

    private void Update()
    {
        if(!frameLoop.g_isActive)
        {
            matColInactive = new Color(1f, 1f, 1f, frameTransparency);
            mat.color = matColInactive;
            burstFlag = false;
        }
        else
        {
            matColActive = new Color(0f, 0f, 0.3f, 1f);
            mat.color = matColActive;
            if (!burstFlag)
            {
                burst1.Play(); burst2.Play(); burst3.Play(); burst4.Play();
                burstFlag = true;
            }
        }

        if (particleFramePos.position != framePos.position)
        {
            elapsedTime += Time.deltaTime;
            float complete = elapsedTime / lerpSpeed;
            particleFramePos.position = Vector3.Lerp(particleFramePos.position, framePos.position, complete);
        }
        else
        {
            particleFramePos.position = framePos.position;
        }

        partTrans(ref ps1);
        partTrans(ref ps2);
        partTrans(ref ps3);
        partTrans(ref ps4);
    }

    private void startLifeTimeSet(ref ParticleSystem ps, float lifetime)
    {
        ParticleSystem.MainModule main = ps.main;
        main.startLifetime = lifetime;
    }

    private void rateOverTimeSet(ref ParticleSystem ps, float rate)
    {
        ParticleSystem.EmissionModule main = ps.emission;
        main.rateOverTime = rate;
    }

    private void partTrans(ref ParticleSystem ps)
    {
        int particlesAmount = ps.GetParticles(particles);
        for (int i = 0; i < particlesAmount; ++i)
        {
            particles[i].position = framePos.position;
        }
        ps.SetParticles(particles, particlesAmount);
    }
}

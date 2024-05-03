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
    private GameObject originalFrame; //non particle frame
    private FrameLoop frameLoop; //script
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private AudioManager _audioManager = null;

    ParticleSystem.Particle[] particles;

    private float elapsedTime;
    [SerializeField]
    private float lerpSpeed = 15f;

    [SerializeField]
    private Transform particleFramePos;

    [SerializeField]
    private GameObject activeFrameObject;

    [SerializeField]
    private float frameTransparency = 0.2f;
    [SerializeField]
    private Material mat;
    private Color matColInactive;
    private Color matColActive;

    [SerializeField]
    private ParticleSystem ps1; //top
    [SerializeField]
    private ParticleSystem ps2; //bottom
    [SerializeField]
    private ParticleSystem ps3; //left
    [SerializeField]
    private ParticleSystem ps4; //right

    [SerializeField]
    private GameObject frameClick;
    [SerializeField]
    private ParticleSystem burst1; //top
    [SerializeField]
    private ParticleSystem burst2; //bottom
    [SerializeField]
    private ParticleSystem burst3; //left
    [SerializeField]
    private ParticleSystem burst4; //right

    bool burstFlag = true;

    bool frameActivated = false;

    float frameColorVal = 1f;

    private void Start()
    {
        frameLoop = originalFrame.GetComponent<FrameLoop>();
        transform.position = player.transform.position;
        burstFlag = true;
        activeFrameObject.SetActive(false);

        //_audioManager.Play("Main BGM");
    }

    private void Update()
    {
        if(!frameLoop.g_isActive)
        {
            activeFrameObject.SetActive(false);
            frameActivated = true;
            frameColorVal = 1f;
            matColInactive = new Color(1f, 1f, 1f, frameTransparency);
            mat.color = matColInactive;
            if (!burstFlag)
            {
                burst1.Play(); burst2.Play(); burst3.Play(); burst4.Play();
                burstFlag = true;
            }
        }
        else
        {
            //while (!frameActivated)
            //{
            //    if (frameColorVal > 0.3f)
            //    {
            //        frameColorVal -= 0.1f;
            //        matColActive = new Color(0f, 0f, 0.3f, 1f);
            //        mat.color = matColActive;
            //    }
            //    else
            //    {
            //        frameColorVal = 0.3f;
            //        frameActivated = true;
            //    }
            //}
            activeFrameObject.SetActive(true);
            matColActive = new Color(0f, 0f, 0.3f, 1f);
            mat.color = matColActive;
            burstFlag = false;
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

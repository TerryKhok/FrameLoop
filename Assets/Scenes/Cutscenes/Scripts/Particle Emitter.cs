using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
    [SerializeField]
    private float xRotationSpeed = 10f;
    [SerializeField]
    private float yRotationSpeed = 10f;
    [SerializeField]
    private float zRotationSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xRotation = xRotationSpeed * Time.deltaTime;
        float yRotation = yRotationSpeed * Time.deltaTime;
        float zRotation = zRotationSpeed * Time.deltaTime;
        transform.Rotate(xRotation, yRotation, zRotation);
    }
}

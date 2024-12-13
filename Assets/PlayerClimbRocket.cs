using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbRocket : MonoBehaviour
{
    [SerializeField] float startPosX;
    [SerializeField] float startPosY;
    [SerializeField] float endPosX;
    [SerializeField] float endPosY;

    [SerializeField] float speed = 1;
    [SerializeField] float startTime = 0;
    [SerializeField] float endTime = 0;

    float elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(GetComponent<Rigidbody2D>().velocity);
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public void Break()
    {
        Destroy(gameObject);
    }
}

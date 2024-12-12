using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTimer : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour _behaviour = null;
    [SerializeField]
    private float _time = 0;
    private float _elapsedTIme = 0;

    private void Update()
    {
        _elapsedTIme += Time.deltaTime;
        
        if(_elapsedTIme >= _time)
        {
            _behaviour.enabled = false;
            Destroy(this);
        }
    }
}

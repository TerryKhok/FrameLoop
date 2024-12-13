using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTimer : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour _behaviour = null;
    [SerializeField]
    private float _time = 0;
    private float _elapsedTIme = 0;

    private void Awake()
    {
        _behaviour.enabled = false;
    }

    private void Update()
    {
        _elapsedTIme += Time.deltaTime;

        if (_elapsedTIme >= _time)
        {
            _behaviour.enabled = true;
            Destroy(this);
        }
    }
}

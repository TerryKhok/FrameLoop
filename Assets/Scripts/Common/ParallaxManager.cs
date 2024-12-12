using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _parallax1, _parallax2 = null;

    [SerializeField]
    private float _switchTiming = 0.0f;

    private bool _switched = false;
    private float _elapsedTime = 0.0f;

    private void Start()
    {
        foreach (var parallax in _parallax1)
        {
            parallax.SetActive(!_switched);
        }
        foreach(var parallax in _parallax2)
        {
            parallax.SetActive(_switched);
        }
    }

    private void Update()
    {
        if(_switched)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;

        if(_elapsedTime >= _switchTiming)
        {
            _switched = true;

            foreach (var parallax in _parallax1)
            {
                parallax.SetActive(!_switched);
            }
            foreach (var parallax in _parallax2)
            {
                parallax.SetActive(_switched);
            }
        }
    }
}

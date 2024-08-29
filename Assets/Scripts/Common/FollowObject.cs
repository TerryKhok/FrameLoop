using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Vector2 _offset = Vector2.zero;
    [SerializeField]
    private bool _ignoreZ = true;
    private Transform _transform;

    public Transform Target
    {
        set => _target = value;
    }

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        var pos = _target.position;
        if(_ignoreZ)
        {
            pos.z = _transform.position.z;
        }
        pos += (Vector3)_offset;

        _transform.position = pos;
    }
}

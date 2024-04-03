using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField]
    private GameObject _windPrefab = null;
    [SerializeField,Tag]
    private List<string> _tagList = new List<string> ();
    [SerializeField]
    private Vector2 _direction = Vector2.zero;
    [SerializeField]
    private float _velocity = 1f;
    [SerializeField]
    private float _lifeSpan = 1f;
    [SerializeField]
    private bool _enabledOnAwake = true;

    private GameObject _instance = null;
    private Wind _wind = null;
    private float _elapsedTime = 0f;
    private Transform _transform = null;
    private Vector3 _position = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    private float _interval = 0f;

    private bool _isEnabled = false;

    private void Start()
    {
        _isEnabled = _enabledOnAwake;
        _transform = transform;
        _rotation = Quaternion.LookRotation(_direction);
        _position = _transform.position;
        _position += (Vector3)_direction.normalized;
        _interval = 1 / _velocity;
    }

    private void Update()
    {
        if (!_isEnabled) { return; }
        _elapsedTime += Time.deltaTime;

        if (_interval <= _elapsedTime)
        {
            _instance = Instantiate(_windPrefab, _position, _rotation);
            _wind = _instance.GetComponent<Wind>();
            _wind.SetValues(_direction, _velocity, _lifeSpan, _tagList);
            _elapsedTime = 0f;
        }
    }

    public void PowerSwitch(bool supply)
    {
        _isEnabled = enabled;
    }
}

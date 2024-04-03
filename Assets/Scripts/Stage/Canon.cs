using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab = null;
    [SerializeField, Tag]
    private List<string> _breakTag = new List<string>();
    [SerializeField]
    private Vector2 _direction = Vector2.zero;
    [SerializeField]
    private float _velocity = 1f;
    [SerializeField]
    private float _range = 1f;
    [SerializeField]
    private float _interval = 1f;
    [SerializeField]
    private bool _enabledOnAwake = true;

    private GameObject _instance = null;
    private Bullet _bullet = null;
    private float _elapsedTime = 0f;
    private Transform _transform = null;
    private Vector3 _position = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    private bool _isEnabled = false;

    private void Start()
    {
        _isEnabled = _enabledOnAwake;
        _transform = transform;
        _rotation = Quaternion.LookRotation(_direction);
        _position = _transform.position;
        _position += (Vector3)_direction.normalized;

        if (!_isEnabled) { return; }
        _instance = Instantiate(_bulletPrefab, _position, _rotation);
        _bullet = _instance.GetComponent<Bullet>();
        _bullet.SetValues(_direction, _velocity, _range, _breakTag);
        _elapsedTime = 0f;
    }

    private void Update()
    {
        if (!_isEnabled) {  return; }

        _elapsedTime += Time.deltaTime;

        if(_interval < _elapsedTime)
        {
            _instance = Instantiate(_bulletPrefab,_position,_rotation);
            _bullet = _instance.GetComponent<Bullet>();
            _bullet.SetValues(_direction, _velocity, _range, _breakTag);
            _elapsedTime = 0f;
        }
    }

    public void PowerSwitch(bool supply)
    {
        _isEnabled = supply;
    }
}

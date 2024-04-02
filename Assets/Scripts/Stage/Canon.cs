using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab = null;
    [SerializeField]
    private Vector2 _direction = Vector2.zero;
    [SerializeField]
    private float _velocity = 1f;
    [SerializeField]
    private float _lifeSpan = 1f;
    [SerializeField]
    private float _interval = 1f;

    private GameObject _instance = null;
    private Bullet _bullet = null;
    private float _elapsedTime = 0f;
    private Transform _transform = null;
    private Vector3 _position = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;

    private void Start()
    {
        _transform = transform;
        _rotation = Quaternion.LookRotation(_direction);
        _position = _transform.position;
        _position += (Vector3)_direction.normalized;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if(_interval < _elapsedTime)
        {
            _instance = Instantiate(_bulletPrefab,_position,_rotation);
            _bullet = _instance.GetComponent<Bullet>();
            _bullet.SetValues(_direction, _velocity, _lifeSpan);
            _elapsedTime = 0f;
        }
    }
}

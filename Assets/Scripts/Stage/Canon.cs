using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField,Tooltip("���˂���Prefab")]
    private GameObject _bulletPrefab = null;
    [SerializeField, Tag,Tooltip("�j��\��Tag")]
    private List<string> _breakTag = new List<string>();
    [SerializeField,Tooltip("���˕���")]
    private Vector2 _direction = Vector2.zero;
    [SerializeField,Tooltip("���ˑ��x")]
    private float _velocity = 1f;
    [SerializeField,Tooltip("�˒�")]
    private float _range = 1f;
    [SerializeField,Tooltip("���ˊԊu(s)")]
    private float _interval = 1f;
    [SerializeField,Tooltip("���߂���L����")]
    private bool _enabledOnAwake = true;

    private GameObject _instance = null;
    private Bullet _bullet = null;
    private float _elapsedTime = 0f;
    private Transform _transform = null;
    private Vector3 _position = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    private bool _enable = false;

    private void Start()
    {
        _enable = _enabledOnAwake;
        _transform = transform.GetChild(0);
        _rotation = Quaternion.FromToRotation(Vector3.right,_direction);
        _position = _transform.position;
        _position += (Vector3)_direction.normalized;

        if (!_enable) { return; }
        _instance = Instantiate(_bulletPrefab, _position, _rotation);
        _bullet = _instance.GetComponent<Bullet>();
        _bullet.SetValues(_direction, _velocity, _range, _breakTag);
        _elapsedTime = 0f;
    }

    private void Update()
    {
        if (!_enable) {  return; }

        _elapsedTime += Time.deltaTime;

        if(_interval < _elapsedTime)
        {
            _instance = Instantiate(_bulletPrefab,_position,_rotation);
            _bullet = _instance.GetComponent<Bullet>();
            _bullet.SetValues(_direction, _velocity, _range, _breakTag);
            _elapsedTime = 0f;
        }
    }

    public void SetEnable(bool enable)
    {
        _enable = enable;
    }

    public void SwitchEnable()
    {
        _enable = !_enable;
    }
}

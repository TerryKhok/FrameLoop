using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTakeUp : MonoBehaviour
{
    private Transform _transform = null;
    private PlayerInfo _playerInfo = null;
    private BoxCollider2D _boxCollider = null;
    private Box _box = null;
    [SerializeField,Tooltip("�؂�ւ�")]
    private bool _toggle = false;

    private int _count = 0;

    private void Start()
    {
        _playerInfo = PlayerInfo.Instance;
        _transform = _playerInfo.g_transform;
        _boxCollider = _playerInfo.g_collider;
    }

    private void Update()
    {
        takeUp();
    }

    public void TakeUpStarted(InputAction.CallbackContext context)
    {
        if(_toggle && _playerInfo.g_takeUpFg)
        {
            _playerInfo.g_takeUpFg = false;
            _playerInfo.g_wall = 0;
            _box.Hold(null);
            _playerInfo.g_box = null;
            return;
        }
        _playerInfo.g_takeUpFg = true;
        Ray ray = new Ray(_transform.position, _transform.right);
        RaycastHit2D hit;
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);

        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 1f, 1 << 7);
        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Box"))
            {
                _box = hit.transform.GetComponent<Box>();
                _box.Hold(_transform);
                _playerInfo.g_box = hit.transform;
            }
        }
    }

    public void TakeUpCanceled(InputAction.CallbackContext context)
    {
        if (_toggle) { return; }
        _playerInfo.g_takeUpFg = false;
        _playerInfo.g_wall = 0;
        _box.Hold(null);
        _playerInfo.g_box = null;
    }

    private void takeUp()
    {
        if (!_playerInfo.g_takeUpFg) { return; }

        bool isTaking = false, movable = true;
        _playerInfo.g_takeUpFg = true;
        Ray ray = new Ray(_transform.position, _transform.right);
        RaycastHit2D[] hits;
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
        float length = 1 + _boxCollider.size.x/2;

        hits = Physics2D.BoxCastAll(ray.origin, size, 0, ray.direction, length, 1 << 6 | 1 << 7 | 1 << 9);
        foreach(var hit in hits)
        {
            if (hit.transform.CompareTag("Box"))
            {
                isTaking = true;
            }
            else
            {
                movable = false;
            }
        }

        if(isTaking)
        {
            _count = 0;
        }
        else
        {
            _count++;
        }

        if (movable)
        {
            _playerInfo.g_wall = 0;
        }
        else
        {
            _playerInfo.g_wall = _transform.right.normalized.x;
        }

        if (_count >= 10)
        {
            _count = 0;
            _playerInfo.g_wall = 0;
            _playerInfo.g_takeUpFg = false;
            _box.Hold(null);
            _playerInfo.g_box = null;
        }
    }
}

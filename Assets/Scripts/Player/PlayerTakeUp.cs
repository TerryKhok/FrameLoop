using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTakeUp : MonoBehaviour
{
    private Transform _transform = null;
    private PlayerInfo _playerInfo = null;
    private FrameLoop _frameLoop = null;
    private BoxCollider2D _boxCollider = null;
    private IBox _box = null;
    [SerializeField,Tooltip("êÿÇËë÷Ç¶")]
    private bool _toggle = false;

    private int _count = 0;
    private LayerMask _insideMask = 0, _outsideMask = 0;

    private void Start()
    {
        _playerInfo = PlayerInfo.Instance;
        _frameLoop = FrameLoop.Instance;
        _transform = _playerInfo.g_transform;
        _boxCollider = _playerInfo.g_collider;
        _insideMask = 1 << LayerMask.NameToLayer("IBox");
        _outsideMask = 1 << LayerMask.NameToLayer("OBox");
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
        Ray ray = new Ray(_transform.position, _transform.right);
        RaycastHit2D hit;
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);

        LayerMask mask = 0;
        if (_frameLoop.g_isActive)
        {
            mask = _insideMask;
        }
        else
        {
            mask = _outsideMask;
        }

        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 1f,mask);
        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Box"))
            {
                _box = hit.transform.GetComponent<IBox>();
                _box.Hold(_transform);
                _playerInfo.g_box = hit.transform;
                _playerInfo.g_takeUpFg = true;
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

        LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform") | 1 << LayerMask.NameToLayer("OBox");
        if (_frameLoop.g_isActive)
        {
            mask = 1 << LayerMask.NameToLayer("IPlatform") | 1 << LayerMask.NameToLayer("IBox");
        }

        hits = Physics2D.BoxCastAll(ray.origin, size, 0, ray.direction, length, mask);
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

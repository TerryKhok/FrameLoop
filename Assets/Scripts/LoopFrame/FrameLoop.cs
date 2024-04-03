using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FrameLoop : SingletonMonoBehaviour<FrameLoop>
{
    [SerializeField]
    private Material _material = null;
    [SerializeField]
    private Vector2 _size = Vector2.one;
    [SerializeField]
    private GameObject _colliderPrefab = null;
    [SerializeField]
    private float _yOffset = 1f;
    [SerializeField]
    private float _yOffset_Crouching = -2f;

    private List<Transform>
        _insiders = new List<Transform>();
    private Dictionary<Transform, Vector2>
        _outsiders = new Dictionary<Transform, Vector2>();
    private List<GameObject> _insideColliderList = new List<GameObject>(),
        _outsideColliderList = new List<GameObject> ();

    private (float min, float max) _loopRangeX = (0, 0), _loopRangeY = (0, 0);

    private BoxCollider _boxCollider = null;
    private Transform _playerTrans = null, _transform = null;
    private bool _isCrouching = false;

    [System.NonSerialized]
    public bool g_isActive = false, g_usable = true;
    private bool _prevActive = false;

    private void Start()
    {
        _transform = transform;
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.size = new Vector3(_size.x + 1, _size.y + 1, 1);
        _playerTrans = PlayerInfo.Instance.g_transform;
        _material.color = new Color32(255, 255, 0, 40);

        var children = transform.GetComponentsInChildren<Transform>().ToList();
        children.Remove(transform);

        children[0].localPosition = new Vector3(0, _size.y / 2);
        children[0].localScale = new Vector3(_size.x + 0.2f, 0.2f, 1);
        children[1].localPosition = new Vector3(0, -_size.y / 2);
        children[1].localScale = new Vector3(_size.x + 0.2f, 0.2f, 1);
        children[2].localPosition = new Vector3(_size.x / 2, 0);
        children[2].localScale = new Vector3(0.2f, _size.y, 1);
        children[3].localPosition = new Vector3(-_size.x / 2, 0);
        children[3].localScale = new Vector3(0.2f, _size.y, 1);
    }

    private void Update()
    {
        g_usable |= PlayerInfo.instance.g_isGround;
        loop();
        adjustPos();
        if(!_prevActive && g_isActive)
        {
            onActive();
        }
        if(_prevActive && !g_isActive)
        {
            onInactive();
        }
    }
    private void LateUpdate()
    {
        _prevActive = g_isActive;
    }

    private void onActive()
    {
        for (int i=0; i <= _size.x+1; i++)
        {
            for(int j=0; j <= _size.y+1; j++)
            {
                if(i == 0 || i == _size.x+1)
                {
                    if(j == 0 || j == _size.y + 1) { continue; }
                }
                if(2 <= i && i <= _size.x-1)
                {
                    if(2 <= j && j <= _size.y-1) { continue; }
                }

                var origin = new Vector2(_loopRangeX.min, _loopRangeY.min);
                origin.x += -0.5f + i;
                origin.y += -0.5f + j;
                RaycastHit[] hit;
                var screenPos = Camera.main.WorldToScreenPoint(origin);
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                hit = Physics.RaycastAll(ray.origin, ray.direction, 10f, 1 << 6);
                if (hit.Length == 0)
                {
                    continue;
                }
                else
                {
                    bool _instantFg = false;
                    foreach(var item in hit)
                    {
                        if (!_insideColliderList.Contains(item.transform.gameObject))
                        {
                            _instantFg = true;
                        }
                    }

                    if (_instantFg)
                    {
                        colliderInstantiate(origin, i, j);
                    }
                }
            }
        }

        foreach(var t in _insiders)
        {
            var collider1 = t.GetComponent<Collider>();
            foreach(var obj in _insideColliderList)
            {
                var collider2 = obj.GetComponent<Collider>();

                Physics.IgnoreCollision(collider1, collider2);
            }
        }

        foreach(var t in _outsiders.Keys)
        {
            var collider1 = t.GetComponent<Collider>();
            foreach (var obj in _insideColliderList)
            {
                var collider2 = obj.GetComponent<Collider>();

                Physics.IgnoreCollision(collider1, collider2);
            }
        }
    }

    private void colliderInstantiate(Vector2 origin,int i, int j)
    {
        Vector3 pos = origin;
        if (i <= 1) { pos.x += _size.x; }
        else if (i >= _size.x) { pos.x -= _size.x; }
        else if (j <= 1) { pos.y += _size.y; }
        else if (j >= _size.y) { pos.y -= _size.y; }
        var instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, _transform);
        if (pos.x < _loopRangeX.min || _loopRangeX.max < pos.x||
            pos.y < _loopRangeY.min || _loopRangeY.max < pos.y)
        {
            _outsideColliderList.Add(instance);
        }
        else
        {
            _insideColliderList.Add(instance);
        }

        if (i == 1 || i == _size.x)
        {
            if (j == 1 || j == _size.y)
            {
                pos = origin;
                if (j == 1) { pos.y += _size.y; }
                if (j == _size.y) { pos.y -= _size.y; }
                instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, _transform);
                _outsideColliderList.Add(instance);
            }
        }
    }

    private void onInactive()
    {
        g_usable = false;
        for(int i=0;i < _insideColliderList.Count; i++)
        {
            Destroy(_insideColliderList[i]);
        }
        for (int i = 0; i < _outsideColliderList.Count; i++)
        {
            Destroy(_outsideColliderList[i]);
        }
        _insideColliderList.Clear();
        _outsideColliderList.Clear();
    }

    public void FrameEnable(InputAction.CallbackContext context)
    {
        g_isActive = context.performed;

        _boxCollider.enabled = g_isActive && g_usable;
        if(g_isActive && g_usable)
        {
            _loopRangeX.min = _transform.position.x - (_size.x/2);
            _loopRangeX.max = _transform.position.x + (_size.x/2);
            _loopRangeY.min = _transform.position.y - (_size.y/2);
            _loopRangeY.max = _transform.position.y + (_size.y/2);

            _material.color = new Color32(50, 255, 0, 130);
        }
        else
        {
            _insiders.Clear();
            _outsiders.Clear();

            _material.color = new Color32(255, 255, 0, 40);
        }
    }

    private void adjustPos()
    {
        if(g_isActive && g_usable) { return; }
        var setPos = _playerTrans.position;
        if (_isCrouching)
        {
            setPos.y += _yOffset_Crouching;
        }
        else
        {
            setPos.y += _yOffset;
        }
        setPos.x = (float)Math.Round(setPos.x, MidpointRounding.AwayFromZero);
        setPos.y = (float)Math.Round(setPos.y, MidpointRounding.AwayFromZero);
        _transform.position = setPos;
    }

    private void loop()
    {
        if (!g_isActive) { return; }
        List<Transform> insiders_copy = new List<Transform>(_insiders);
        foreach (var t in insiders_copy)
        {
            if(t == null) 
            {
                _insiders.Remove(t);
                continue;
            }
            var pos = t.position;
            if (t.position.x <= _loopRangeX.min)
            {
                pos.x += _size.x;
            }
            else if (_loopRangeX.max <= t.position.x)
            {
                pos.x -= _size.x;
            }
            if (t.position.y <= _loopRangeY.min)
            {
                pos.y += _size.y;
                var rb = t.GetComponent<Rigidbody>();
                var velocity = rb.velocity;
                if(velocity.y < -15f) { velocity.y = -15f; }
                rb.velocity = velocity;
            }
            else if (_loopRangeY.max <= t.position.y)
            {
                pos.y -= _size.y;
            }
            t.position = pos;
        }

        Dictionary<Transform, Vector2> outsiders_copy = new Dictionary<Transform, Vector2>(_outsiders);
        foreach (var pair in outsiders_copy)
        {
            if (pair.Key == null)
            {
                _outsiders.Remove(pair.Key);
                continue;
            }
            var pos = pair.Key.position;
            if (_loopRangeX.min <= pair.Key.position.x && pair.Value.x == -1)
            {
                pos.x += _size.x;
                var value = pair.Value;
                value.x = 0;
                _outsiders[pair.Key] = value;
            }
            else if (pair.Key.position.x <= _loopRangeX.max && pair.Value.x == 1)
            {
                pos.x -= _size.x;
                var value = pair.Value;
                value.x = 0;
                _outsiders[pair.Key] = value;
            }
            if (_loopRangeY.min <= pair.Key.position.y && pair.Value.y == -1)
            {
                pos.y += _size.y;
                var value = pair.Value;
                value.y = 0;
                _outsiders[pair.Key] = value;
            }
            else if (pair.Key.position.y <= _loopRangeY.max && pair.Value.y == 1)
            {
                pos.y -= _size.y;
                var value = pair.Value;
                value.y = 0;
                _outsiders[pair.Key] = value;
            }
            pair.Key.position = pos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_insiders.Contains(other.transform) && !_outsiders.ContainsKey(other.transform))
        {
            var pos = other.transform.position;
            Vector2 vector = Vector2.zero;
            if (pos.x < _loopRangeX.min) { vector.x = -1; }
            else if (_loopRangeX.max < pos.x) { vector.x = 1; }
            if (pos.y < _loopRangeY.min) { vector.y = -1; }
            else if (_loopRangeY.max < pos.y) { vector.y = 1; }
            if (vector != Vector2.zero)
            {
                _outsiders.Add(other.transform, vector);
                var collider1 = other.GetComponent<Collider>();
                foreach (var t in _outsideColliderList)
                {
                    var collider2 = t.GetComponent<Collider>();
                    Physics.IgnoreCollision(collider1, collider2);
                }
            }
            else
            {
                _insiders.Add(other.transform);
                var collider1 = other.GetComponent<Collider>();
                foreach (var t in _insideColliderList)
                {
                    var collider2 = t.GetComponent<Collider>();
                    Physics.IgnoreCollision(collider1, collider2);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_insiders.Contains(other.transform))
        {
            _insiders.Remove(other.transform);
        }

        if(_outsiders.ContainsKey(other.transform))
        {
            _outsiders.Remove(other.transform);
        }
    }

    public void SetCrouching(bool isCrouching)
    {
        _isCrouching = isCrouching;
    }
}

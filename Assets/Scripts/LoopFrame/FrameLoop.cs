using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;

public class FrameLoop : SingletonMonoBehaviour<FrameLoop>
{
    //[SerializeField]
    //private GameObject _emptyObj = null;
    [SerializeField,Tooltip("当たり判定を生成するために設置するTile")]
    private Tile _tile = null;
    [SerializeField,Tooltip("内側の当たり判定用のTilemap")]
    private Tilemap _insideTile = null;
    [SerializeField, Tooltip("外側の当たり判定用のTilemap")]
    private Tilemap _outsideTile = null;
    [SerializeField,Tooltip("Frameに適用するMaterial(Scriptから色が変更されるので専用のものにする)")]
    private Material _material = null;
    [SerializeField,Tooltip("FrameのSize")]
    private Vector2Int _size = Vector2Int.one;
    [SerializeField,Tooltip("プレイヤーの座標からY方向にどれだけずらすか")]
    private float _yOffset = 1f;
    [SerializeField, Tooltip("しゃがみ中にプレイヤーの座標からY方向にどれだけずらすか")]
    private float _yOffset_Crouching = -2f;
    [SerializeField,Tooltip("切り替え")]
    private bool _toggle = false;

    private List<Transform>
        _insiders = new List<Transform>();
    private Dictionary<Transform, Vector2>
        _outsiders = new Dictionary<Transform, Vector2>();
    private List<GameObject> _insideColliderList = new List<GameObject>(),
        _outsideColliderList = new List<GameObject> ();
    private List<(Transform origin, Transform instance)>
        _insideCopyList = new List<(Transform origin, Transform instance)>(),
        _outsideCopyList = new List<(Transform origin, Transform instance)>();
    private List<Fan> _fanList = new List<Fan>();

    private (float min, float max) _loopRangeX = (0, 0), _loopRangeY = (0, 0);

    private BoxCollider2D _boxCollider = null;
    private Transform _playerTrans = null, _transform = null;
    private CompositeCollider2D _insideTileCol = null, _outsideTileCol = null;
    private bool _isCrouching = false;
    private InputManager _inputManager = null;
    private PlayerInfo _playerInfo = null;
    private GameObject _colliderPrefab = null;

    [System.NonSerialized]
    public bool g_isActive = false, g_usable = true;
    private bool _prevActive = false;

    private void Start()
    {
        _transform = transform;
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.size = new Vector3(_size.x+0.3f, _size.y+0.3f, 1);
        _playerInfo = PlayerInfo.Instance;
        _playerTrans = _playerInfo.g_transform;
        _insideTileCol = _insideTile.GetComponent<CompositeCollider2D>();
        _outsideTileCol = _outsideTile.GetComponent<CompositeCollider2D>();
        TilemapRenderer insideRenderer = _insideTile.GetComponent<TilemapRenderer>();
        insideRenderer.enabled = false;
        TilemapRenderer outsideRenderer = _outsideTile.GetComponent<TilemapRenderer>();
        outsideRenderer.enabled = false;
        _material.color = new Color32(255, 255, 0, 100);
        _colliderPrefab = (GameObject)Resources.Load("Collider");

        var managerObj = GameObject.FindGameObjectWithTag("GameManager");
        _inputManager = managerObj.GetComponent<InputManager>();

        var fanObjs = GameObject.FindGameObjectsWithTag("Fan");
        foreach (var fanObj in fanObjs)
        {
            _fanList.Add(fanObj.GetComponent<Fan>());
        }

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
        _boxCollider.enabled = g_isActive && g_usable;
        if (g_isActive && g_usable)
        {
            _loopRangeX.min = _transform.position.x - (_size.x / 2);
            _loopRangeX.max = _transform.position.x + (_size.x / 2);
            _loopRangeY.min = _transform.position.y - (_size.y / 2);
            _loopRangeY.max = _transform.position.y + (_size.y / 2);

            _material.color = new Color32(0, 255, 0, 150);
        }
        else
        {
            _insiders.Clear();
            _outsiders.Clear();

            _material.color = new Color32(255, 255, 0, 100);
        }

        g_usable |= PlayerInfo.instance.g_isGround;
        loop();
        adjustPos();

        //List<(Transform origin, Transform instance)> copy = new List<(Transform, Transform)>(_outsideCopyList);
        //foreach (var pair in copy)
        //{
        //    if(pair.origin == null || !_outsiders.ContainsKey(pair.origin))
        //    {
        //        _outsideCopyList.Remove(pair);
        //        continue;
        //    }
        //    var currentPos = pair.origin.position;
        //    currentPos -= Vector3.Scale(_outsiders[pair.origin], _size);
        //    pair.instance.position = currentPos;
        //}

        if (!_prevActive && g_isActive)
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
        foreach(var t in _insiders)
        {
            var collider = t.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(collider, _insideTileCol, false);
            Physics2D.IgnoreCollision(collider, _outsideTileCol, false);
            foreach(var t2 in _insideColliderList)
            {
                var insideCol = t2.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(collider, insideCol, false);
            }
            foreach (var t3 in _outsideColliderList)
            {
                var outsideCol = t3.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(collider, outsideCol, false);
            }
        }
        foreach (var t in _outsiders.Keys)
        {
            var collider = t.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(collider, _insideTileCol, false);
            Physics2D.IgnoreCollision(collider, _outsideTileCol, false);
            foreach (var t2 in _insideColliderList)
            {
                var insideCol = t2.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(collider, insideCol, false);
            }
            foreach (var t3 in _outsideColliderList)
            {
                var outsideCol = t3.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(collider, outsideCol, false);
            }
        }

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
                RaycastHit2D[] hit;
                var screenPos = Camera.main.WorldToScreenPoint(origin);
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                hit = Physics2D.RaycastAll(ray.origin, ray.direction, 15, 1 << 6 | 1 << 7);
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
                            if (item.transform.CompareTag("Box"))
                            {
                                ColliderInstantiate(item.transform.position, i, j);
                                continue;
                            }
                            _instantFg = true;
                        }
                    }

                    if (_instantFg)
                    {
                        setColliderTile(origin, i, j);
                    }
                }
            }
        }

        foreach(var t in _insiders)
        {
            var collider1 = t.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(collider1, _insideTileCol);
            foreach (var obj in _insideColliderList)
            {
                var collider2 = obj.GetComponent<Collider2D>();

                Physics2D.IgnoreCollision(collider1, collider2);
            }
        }

        foreach(var t in _outsiders.Keys)
        {
            var collider1 = t.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(collider1, _outsideTileCol);

            foreach (var obj in _insideColliderList)
            {
                var collider2 = obj.GetComponent<Collider2D>();

                Physics2D.IgnoreCollision(collider1, collider2);
            }
        }

        foreach(var fan in _fanList)
        {
            fan.FanLoopStarted();
        }
    }

    private void setColliderTile(Vector2 origin,int i, int j)
    {
        Vector3 pos = origin;
        if (i <= 1) { pos.x += _size.x; }
        else if (i >= _size.x) { pos.x -= _size.x; }
        else if (j <= 1) { pos.y += _size.y; }
        else if (j >= _size.y) { pos.y -= _size.y; }
        if (pos.x < _loopRangeX.min || _loopRangeX.max < pos.x||
            pos.y < _loopRangeY.min || _loopRangeY.max < pos.y)
        {
            Vector3Int intPos = new Vector3Int((int)(pos.x-0.5f), (int)(pos.y-0.5f));
            _outsideTile.SetTile(intPos, _tile);
        }
        else
        {
            Vector3Int intPos = new Vector3Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f));
            _insideTile.SetTile(intPos, _tile);
        }

        if (i == 1 || i == _size.x)
        {
            if (j <= 1 || j >= _size.y)
            {
                pos = origin;
                if (j <= 1) { pos.y += _size.y; }
                if (j >= _size.y) { pos.y -= _size.y; }
                Vector3Int intPos = new Vector3Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f));
                if(j == 1 || j == _size.y)
                {
                    _outsideTile.SetTile(intPos, _tile);
                }
                else
                {
                    _insideTile.SetTile(intPos, _tile);
                }
            }
        }
    }

    private void ColliderInstantiate(Vector3 pos,int i, int j)
    {
        if (i <= 1) { pos.x += _size.x; }
        else if (i >= _size.x) { pos.x -= _size.x; }
        else if (j <= 1) { pos.y += _size.y; }
        else if (j >= _size.y) { pos.y -= _size.y; }
        var instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, _transform);
        if (pos.x < _loopRangeX.min || _loopRangeX.max < pos.x ||
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
            if (j <= 1 || j >= _size.y)
            {
                if (j <= 1) { pos.y += _size.y; }
                if (j >= _size.y) { pos.y -= _size.y; }
                instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, _transform);
                if (j == 1 || j == _size.y)
                {
                    _outsideColliderList.Add(instance);
                }
                else
                {
                    _insideColliderList.Add(instance);
                }
            }
        }
    }

    private void onInactive()
    {
        g_usable = false;
        _insideTile.ClearAllTiles();
        _outsideTile.ClearAllTiles();
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

        foreach (var fan in _fanList)
        {
            fan.FanLoopCanceled();
        }
    }

    public void FrameStarted(InputAction.CallbackContext context)
    {
        if (_toggle)
        {
            g_isActive = !g_isActive;
            if (g_isActive)
            {
                _inputManager.SetVibration(0, 0.8f, 0.1f);
            }
            return;
        }
        g_isActive = true;
        _inputManager.SetVibration(0, 0.8f, 0.1f);
    }

    public void FrameCanceled(InputAction.CallbackContext context)
    {
        if(_toggle) { return; }
        g_isActive = false;
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
            if (t == _playerInfo.g_box)
            {
                continue;
            }
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
            }
            else if (_loopRangeY.max <= t.position.y)
            {
                pos.y -= _size.y;
            }

            var rb = t.GetComponent<Rigidbody2D>();
            var velocity = rb.velocity;
            if (velocity.y < -15f) { velocity.y = -15f; }
            rb.velocity = velocity;

            if(t.position != pos)
            {
                _inputManager.SetVibration(0, 0.8f, 0.1f);
            }

            t.position = pos;
        }

        Dictionary<Transform, Vector2> outsiders_copy = new Dictionary<Transform, Vector2>(_outsiders);
        foreach (var pair in outsiders_copy)
        {
            if (pair.Key == _playerInfo.g_box)
            {
                continue;
            }
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
            if (pair.Key.position != pos)
            {
                _inputManager.SetVibration(0, 0.8f, 0.1f);
            }

            pair.Key.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
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
                //var otherTransform = other.transform;
                //MeshRenderer renderer = other.GetComponent<MeshRenderer>();
                //MeshFilter filter = otherTransform.GetComponent<MeshFilter>();
                //BoxCollider collider = otherTransform.GetComponent<BoxCollider>();
                //pos -= Vector3.Scale(vector, _size);
                //GameObject obj = Instantiate(_emptyObj, pos, Quaternion.identity);
                //obj.name = otherTransform.name;
                //obj.transform.localScale = otherTransform.localScale;
                //obj.AddComponent(renderer);
                //obj.AddComponent(filter);
                //obj.AddComponent(collider);
                //_outsideCopyList.Add((otherTransform, obj.transform));

                _outsiders.Add(other.transform, vector);
                var collider = other.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(collider, _outsideTileCol);
                foreach(var t in _outsideColliderList)
                {
                    var collider2 = t.GetComponent<Collider2D>();
                    Physics2D.IgnoreCollision(collider, collider2);
                }
            }
            else
            {
                _insiders.Add(other.transform);
                var collider = other.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(collider, _insideTileCol);
                foreach (var t in _insideColliderList)
                {
                    var collider2 = t.GetComponent<Collider2D>();
                    Physics2D.IgnoreCollision(collider, collider2);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_outsiders.ContainsKey(other.transform))
        {
            _outsiders.Remove(other.transform);
        }

        if (g_isActive) { return; }
        if (_insiders.Contains(other.transform))
        {
            _insiders.Remove(other.transform);
        }
    }

    public void SetCrouching(bool isCrouching)
    {
        _isCrouching = isCrouching;
    }

    public int GetSizeX()
    {
        return _size.x;
    }
}

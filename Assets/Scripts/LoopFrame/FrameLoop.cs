using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
public class FrameLoop : SingletonMonoBehaviour<FrameLoop>,IParentOnTrigger
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

    private List<Collider2D>
        _insiders = new List<Collider2D>(),
        _enterOutsiders = new List<Collider2D>();

    private Dictionary<Collider2D, Vector2>
        _outsiders = new Dictionary<Collider2D, Vector2>(),
        _exitInsiders = new Dictionary<Collider2D, Vector2>();

    private List<Collider2D> 
        _insideColliderList = new List<Collider2D>(),
        _outsideColliderList = new List<Collider2D> ();

    private Dictionary<Collider2D, Transform>
        _insideCopyDic = new Dictionary<Collider2D, Transform>(),
        _outsideCopyDic = new Dictionary<Collider2D, Transform>();

    private List<Fan> _fanList = new List<Fan>();

    private (float min, float max) _loopRangeX = (0, 0), _loopRangeY = (0, 0);

    private BoxCollider2D _boxCollider = null;
    private Transform _playerTrans = null, _transform = null;
    private CompositeCollider2D _insideTileCol = null, _outsideTileCol = null;
    private bool _isCrouching = false;
    private InputManager _inputManager = null;
    private PlayerInfo _playerInfo = null;
    private GameObject _colliderPrefab = null;
    private Transform _topT = null, _bottomT = null, _rightT = null, _leftT = null;

    [System.NonSerialized]
    public bool g_isActive = false, g_usable = true;
    private bool _prevActive = false;

    private void Start()
    {
        _transform = transform;
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.size = new Vector3(_size.x + 0.3f, _size.y + 0.3f, 1);
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
        _topT = children[0];
        _bottomT = children[1];
        _rightT = children[2];
        _leftT = children[3];

        _topT.localPosition = new Vector3(0, _size.y / 2);
        _bottomT.localPosition = new Vector3(0, -_size.y / 2);
        _rightT.localPosition = new Vector3(_size.x / 2, 0);
        _leftT.localPosition = new Vector3(-_size.x / 2, 0);

        _topT.localScale = new Vector3(_size.x + 0.2f, 0.2f, 1);
        _bottomT.localScale = new Vector3(_size.x + 0.2f, 0.2f, 1);
        _rightT.localScale = new Vector3(0.2f, _size.y, 1);
        _leftT.localScale = new Vector3(0.2f, _size.y, 1);

        BoxCollider2D childCol = null;
        childCol = _topT.GetComponent<BoxCollider2D>();
        childCol.size = new Vector2(1f, 5f);
        childCol.offset = new Vector2(0, 2.5f);
        childCol = _bottomT.GetComponent<BoxCollider2D>();
        childCol.size = new Vector2(1f, 5f);
        childCol.offset = new Vector2(0, -2.5f);
        childCol = _rightT.GetComponent<BoxCollider2D>();
        childCol.size = new Vector2(5f, 1f + 2 / (float)_size.y);
        childCol.offset = new Vector2(2.5f, 0);
        childCol = _leftT.GetComponent<BoxCollider2D>();
        childCol.size = new Vector2(5f, 1f + 2 / (float)_size.y);
        childCol.offset = new Vector2(-2.5f, 0);
    }

    private void Update()
    {
        Debug.Log(_exitInsiders.Count);
        g_isActive &= g_usable;
        if (g_isActive)
        {
            copy();
        }

        g_usable |= PlayerInfo.instance.g_isGround;
        //loop();
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
    //private void FixedUpdate()
    //{
    //    Dictionary<Collider2D, Vector2> copyDic = new Dictionary<Collider2D, Vector2>(_outsiders);
    //    foreach(var col in copyDic.Keys)
    //    {
    //        if(col == null)
    //        {
    //            _outsiders.Remove(col);
    //            continue;
    //        }
    //        _outsiders[col] = Vector2.zero;
    //    }

    //    copyDic = new Dictionary<Collider2D, Vector2>(_exitInsiders);
    //    foreach (var col in copyDic.Keys)
    //    {
    //        if (col == null)
    //        {
    //            _exitInsiders.Remove(col);
    //            continue;
    //        }
    //        _exitInsiders[col] = Vector2.zero;
    //    }
    //}

    private void onActive()
    {
        _loopRangeX.min = _transform.position.x - (_size.x / 2);
        _loopRangeX.max = _transform.position.x + (_size.x / 2);
        _loopRangeY.min = _transform.position.y - (_size.y / 2);
        _loopRangeY.max = _transform.position.y + (_size.y / 2);

        _material.color = new Color32(0, 255, 0, 150);

        foreach (var col in _insiders)
        {
            Physics2D.IgnoreCollision(col, _insideTileCol, false);
            Physics2D.IgnoreCollision(col, _outsideTileCol, false);
            foreach (var insideCol in _insideColliderList)
            {
                Physics2D.IgnoreCollision(col, insideCol, false);
            }
            foreach (var outsideCol in _outsideColliderList)
            {
                Physics2D.IgnoreCollision(col, outsideCol, false);
            }
        }
        foreach (var col in _outsiders.Keys)
        {
            Physics2D.IgnoreCollision(col, _insideTileCol, false);
            Physics2D.IgnoreCollision(col, _outsideTileCol, false);
            foreach (var insideCol in _insideColliderList)
            {
                Physics2D.IgnoreCollision(col, insideCol, false);
            }
            foreach (var outsideCol in _outsideColliderList)
            {
                Physics2D.IgnoreCollision(col, outsideCol, false);
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
                        if (!_insideColliderList.Contains(item.collider))
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

        foreach(var col in _insiders)
        {
            Physics2D.IgnoreCollision(col, _insideTileCol);
            foreach (var insideCol in _insideColliderList)
            {
                Physics2D.IgnoreCollision(col, insideCol);
            }
        }

        foreach(var col in _outsiders.Keys)
        {
            Physics2D.IgnoreCollision(col, _outsideTileCol);

            foreach (var outsideCol in _outsideColliderList)
            {
                Physics2D.IgnoreCollision(col, outsideCol);
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
        var col = instance.GetComponent<Collider2D>();
        if (pos.x < _loopRangeX.min || _loopRangeX.max < pos.x ||
            pos.y < _loopRangeY.min || _loopRangeY.max < pos.y)
        {
            _outsideColliderList.Add(col);
        }
        else
        {
            _insideColliderList.Add(col);
        }

        if (i == 1 || i == _size.x)
        {
            if (j <= 1 || j >= _size.y)
            {
                if (j <= 1) { pos.y += _size.y; }
                if (j >= _size.y) { pos.y -= _size.y; }
                instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, _transform);
                col = instance.GetComponent<Collider2D>();
                if (j == 1 || j == _size.y)
                {
                    _outsideColliderList.Add(col);
                }
                else
                {
                    _insideColliderList.Add(col);
                }
            }
        }
    }

    private void onInactive()
    {
        _material.color = new Color32(255, 255, 0, 100);

        g_usable = false;
        _insideTile.ClearAllTiles();
        _outsideTile.ClearAllTiles();
        for(int i=0;i < _insideColliderList.Count; i++)
        {
            Destroy(_insideColliderList[i].gameObject);
        }
        for (int i = 0; i < _outsideColliderList.Count; i++)
        {
            Destroy(_outsideColliderList[i].gameObject);
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
                _inputManager.SetVibration(0.2f, 0f, 0f);
            }
            else
            {
                _inputManager.SetVibration(0f, 0f, 0f);
            }
            return;
        }
        g_isActive = true;
        _inputManager.SetVibration(0.2f, 0f, 0f);
    }

    public void FrameCanceled(InputAction.CallbackContext context)
    {
        if(_toggle) { return; }
        _inputManager.SetVibration(0, 0, 0);
        g_isActive = false;
    }

    private void adjustPos()
    {
        if(g_isActive) { return; }
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
        List<Collider2D> insiders_copy = new List<Collider2D>(_insiders);
        foreach (var col in insiders_copy)
        {
            if (col == null)
            {
                _insiders.Remove(col);
                continue;
            }
            var t = col.transform;
            if (t == _playerInfo.g_box)
            {
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
                _inputManager.SetVibration(0.2f, 0.6f, 0.1f);
            }

            t.position = pos;
        }

        Dictionary<Collider2D, Vector2> outsiders_copy = new Dictionary<Collider2D, Vector2>(_outsiders);
        foreach (var pair in outsiders_copy)
        {
            if (pair.Key == null)
            {
                _insiders.Remove(pair.Key);
                continue;
            }
            var t = pair.Key.transform;
            if (t == _playerInfo.g_box)
            {
                continue;
            }
            var pos = t.position;
            if (_loopRangeX.min <= t.position.x && pair.Value.x == -1)
            {
                pos.x += _size.x;
                var value = pair.Value;
                value.x = 0;
                _outsiders[pair.Key] = value;
            }
            else if (t.position.x <= _loopRangeX.max && pair.Value.x == 1)
            {
                pos.x -= _size.x;
                var value = pair.Value;
                value.x = 0;
                _outsiders[pair.Key] = value;
            }
            if (_loopRangeY.min <= t.position.y && pair.Value.y == -1)
            {
                pos.y += _size.y;
                var value = pair.Value;
                value.y = 0;
                _outsiders[pair.Key] = value;
            }
            else if (t.position.y <= _loopRangeY.max && pair.Value.y == 1)
            {
                pos.y -= _size.y;
                var value = pair.Value;
                value.y = 0;
                _outsiders[pair.Key] = value;
            }
            if (t.position != pos)
            {
                _inputManager.SetVibration(0.2f, 0.6f, 0.1f);
            }

            t.position = pos;
        }
    }

    private void copy()
    {
        foreach (var col in _exitInsiders.Keys)
        {
            if (!_insideCopyDic.ContainsKey(col))
            {
                GameObject instanceObject = new GameObject(col.transform.name + "_copy");
                SpriteRenderer setRenderer = col.GetComponent<SpriteRenderer>();
                Rigidbody2D setRigidbody = col.GetComponent<Rigidbody2D>();

                instanceObject.AddComponent(setRenderer);
                var rb = instanceObject.AddComponent(setRigidbody);
                rb.isKinematic = true;

                var pos = col.transform.position;
                var vec = _exitInsiders[col];
                switch (col)
                {
                    case BoxCollider2D:
                        BoxCollider2D setBoxCol = col as BoxCollider2D;
                        instanceObject.AddComponent(setBoxCol);
                        break;
                    case CircleCollider2D:
                        CircleCollider2D setCircleCol = col as CircleCollider2D;
                        instanceObject.AddComponent(setCircleCol);
                        break;
                    case CapsuleCollider2D:
                        CapsuleCollider2D setCapsuleCol = col as CapsuleCollider2D;
                        instanceObject.AddComponent(setCapsuleCol);
                        break;
                }

                vec *= _size;
                pos -= (Vector3)vec;

                instanceObject.transform.SetParent(col.transform, false);
                instanceObject.transform.position = pos;
                _insideCopyDic.Add(col, instanceObject.transform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_insideCopyDic.ContainsValue(other.transform) || _outsideCopyDic.ContainsValue(other.transform))
        {
            return;
        }

        if (_outsiders.ContainsKey(other))
        {
            if (!_enterOutsiders.Contains(other))
            {
                _enterOutsiders.Add(other);
            }
        }
        else
        {
            if (!_insiders.Contains(other))
            {
                _insiders.Add(other);
            }
        }
#if false
        if (!_insiders.Contains(other) && !_outsiders.ContainsKey(other))
        {
            var pos = other.transform.position;
            Vector2 vector = Vector2.zero;
            if (pos.x < _loopRangeX.min) { vector.x = -1; }
            else if (_loopRangeX.max < pos.x) { vector.x = 1; }
            if (pos.y < _loopRangeY.min) { vector.y = -1; }
            else if (_loopRangeY.max < pos.y) { vector.y = 1; }
            if (vector != Vector2.zero && !other.CompareTag("Player"))
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

                _outsiders.Add(other, vector);
                var collider = other.GetComponent<Collider2D>();
                Physics2D.Ignoreother(collider, _outsideTileCol);
                foreach(var col in _outsideColliderList)
                {
                    Physics2D.Ignoreother(collider, col);
                }
            }
            else
            {
                _insiders.Add(other);
                var collider = other.GetComponent<Collider2D>();
                Physics2D.Ignoreother(collider, _insideTileCol);
                foreach (var col in _insideColliderList)
                {
                    Physics2D.Ignoreother(collider, col);
                }
            }
        }
#endif
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_exitInsiders.ContainsKey(other) && g_isActive)
        {
            Vector2 vec = _exitInsiders[other];
            Transform t = other.transform;
            var pos = t.position;

            vec *= _size;
            pos -= (Vector3)vec;
            t.position = pos;
        }

        if (_insiders.Contains(other))
        {
            if (!g_isActive)
            {
                _insiders.Remove(other);
            }
        }

        if (_enterOutsiders.Contains(other))
        {
            _enterOutsiders.Remove(other);
        }
    }

    public void OnEnter(Collider2D other,Transform transform)
    {
        if (_insideCopyDic.ContainsValue(other.transform) || _outsideCopyDic.ContainsValue(other.transform))
        {
            return;
        }

        Vector2 vec = Vector2.zero;
        if (transform == _topT) { vec.y = 1; }
        if (transform == _bottomT) { vec.y = -1; }
        if (transform == _rightT) { vec.x = 1; }
        if (transform == _leftT) { vec.x = -1; }

        if (_insiders.Contains(other))
        {
            if (!_exitInsiders.ContainsKey(other))
            {
                _exitInsiders.Add(other, vec);
            }
            else
            {;
                _exitInsiders[other] = vec;
            }
        }
        else
        {
            if (other.CompareTag("Player")) { return; }

            if(!_outsiders.ContainsKey(other))
            {
                _outsiders.Add(other, vec);
            }
            else
            {
                _outsiders[other] = vec;
            }
        }
    }

    public void OnExit(Collider2D other, Transform transform)
    {
        if (_enterOutsiders.Contains(other) && g_isActive)
        {
            Vector2 vec = _outsiders[other];
            Transform t = other.transform;
            var pos = t.position;

            vec *= _size;
            pos -= (Vector3)vec;
            t.position = pos;
        }

        if (_outsiders.ContainsKey(other))
        {
            _outsiders.Remove(other);
        }

        if (_exitInsiders.ContainsKey(other))
        {
            _exitInsiders.Remove(other);
        }
    }

    public void OnStay(Collider2D other, Transform transform)
    {

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

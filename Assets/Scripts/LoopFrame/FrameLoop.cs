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
        _outsideCopyDic = new Dictionary<Collider2D, Transform>();
    private Dictionary<Collider2D, List<Transform>>
    _insideCopyDic = new Dictionary<Collider2D, List<Transform>>();

    private List<Fan> _fanList = new List<Fan>();

    private (float min, float max) _loopRangeX = (0, 0), _loopRangeY = (0, 0);

    private BoxCollider2D _boxCollider = null;
    private Transform _playerTrans = null, _transform = null;
    private SpriteMask _spriteMask = null;
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
        _spriteMask = GetComponent<SpriteMask>();
        _spriteMask.enabled = false;
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.size = new Vector3(_size.x - 0.2f, _size.y - 0.2f, 1);
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
        g_isActive &= g_usable;
        if (g_isActive)
        {
            instantiateCopy();
        }
        List<Collider2D> workList = new List<Collider2D>(_insiders);
        foreach(var col in workList)
        {
            if(col == null) 
            {
                _insiders.Remove(col);
                if (_exitInsiders.ContainsKey(col))
                {
                    _exitInsiders.Remove(col);
                }
                continue;
            }
            var rb = col.GetComponent<Rigidbody2D>();
            var velocity = rb.velocity;
            if(velocity.y < -15f)
            {
                velocity.y = -15f;
                rb.velocity = velocity;
            }
        }

        Dictionary<Collider2D, Vector2> workDict = new Dictionary<Collider2D, Vector2>(_outsiders);
        foreach (var col in workDict.Keys)
        {
            if (col == null)
            {
                _outsiders.Remove(col);
                if (_exitInsiders.ContainsKey(col))
                {
                    _enterOutsiders.Remove(col);
                }
                continue;
            }
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

        _spriteMask.enabled = true;
        _material.color = new Color32(0, 255, 0, 150);

        List<Collider2D> removeList = new List<Collider2D>();

        foreach (var col in _insiders)
        {
            if (_outsiders.ContainsKey(col))
            {
                var pos = col.transform.position;
                if (pos.x < _loopRangeX.min || pos.x > _loopRangeX.max ||
                    pos.y < _loopRangeY.min || pos.y > _loopRangeY.max)
                {
                    removeList.Add(col);
                    continue;
                }
            }

            SpriteRenderer renderer = col.GetComponent<SpriteRenderer>();
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            if (col.CompareTag("Player"))
            {
                continue;
            }
            col.gameObject.layer++;

            //Physics2D.IgnoreCollision(col, _insideTileCol, false);
            //Physics2D.IgnoreCollision(col, _outsideTileCol, false);
            //foreach (var insideCol in _insideColliderList)
            //{
            //    Physics2D.IgnoreCollision(col, insideCol, false);
            //}
            //foreach (var outsideCol in _outsideColliderList)
            //{
            //    Physics2D.IgnoreCollision(col, outsideCol, false);
            //}
        }

        foreach(var col in removeList)
        {
            _insiders.Remove(col);
            if (_exitInsiders.ContainsKey(col))
            {
                _exitInsiders.Remove(col);
            }
        }

        //foreach (var col in _outsiders.Keys)
        //{
        //    Physics2D.IgnoreCollision(col, _insideTileCol, false);
        //    Physics2D.IgnoreCollision(col, _outsideTileCol, false);
        //    foreach (var insideCol in _insideColliderList)
        //    {
        //        Physics2D.IgnoreCollision(col, insideCol, false);
        //    }
        //    foreach (var outsideCol in _outsideColliderList)
        //    {
        //        Physics2D.IgnoreCollision(col, outsideCol, false);
        //    }
        //}

        for (int i=0; i <= _size.x+1; i++)
        {
            for(int j=0; j <= _size.y+1; j++)
            {
                if (i == 0 || i == _size.x + 1)
                {
                    if (j == 0 || j == _size.y + 1)
                    {
                        continue;
                    }
                }

                var origin = new Vector2(_loopRangeX.min, _loopRangeY.min);
                origin.x += -0.5f + i;
                origin.y += -0.5f + j;
                RaycastHit2D[] hit;
                var screenPos = Camera.main.WorldToScreenPoint(origin);
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                LayerMask layerMask = 0;
                layerMask |= 1 << LayerMask.NameToLayer("OPlatform");
                layerMask |= 1 << LayerMask.NameToLayer("OBox");
                hit = Physics2D.RaycastAll(ray.origin, ray.direction, 15, layerMask);
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
                                if (i == 0 || i < _size.x + 1 || j == 0 || j < _size.y + 1)
                                {
                                    ColliderInstantiate(item.transform.position, i, j, item.transform);
                                }
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

        //foreach(var col in _insiders)
        //{
        //    Physics2D.IgnoreCollision(col, _insideTileCol);
        //    foreach (var insideCol in _insideColliderList)
        //    {
        //        Physics2D.IgnoreCollision(col, insideCol);
        //    }
        //}

        //foreach(var col in _outsiders.Keys)
        //{
        //    Physics2D.IgnoreCollision(col, _outsideTileCol);

        //    foreach (var outsideCol in _outsideColliderList)
        //    {
        //        Physics2D.IgnoreCollision(col, outsideCol);
        //    }
        //}

        foreach(var fan in _fanList)
        {
            fan.FanLoopStarted();
        }
    }

    private void setColliderTile(Vector2 origin,int i, int j)
    {
        Vector3 pos = origin;

        if (1 <= i && i <= _size.x)
        {
            if (1 <= j && j <= _size.y)
            {
                Vector3Int intPos = new Vector3Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f));
                _insideTile.SetTile(intPos, _tile);
            }
        }

        if (i <= 1) { pos.x += _size.x; }
        else if (i >= _size.x) { pos.x -= _size.x; }
        else if (j <= 1) { pos.y += _size.y; }
        else if (j >= _size.y) { pos.y -= _size.y; }
        if (pos.x < _loopRangeX.min || _loopRangeX.max < pos.x||
            pos.y < _loopRangeY.min || _loopRangeY.max < pos.y)
        {
            Vector3Int intPos = new Vector3Int((int)(pos.x-0.5f), (int)(pos.y-0.5f));
            _insideTile.SetTile(intPos, _tile);
            intPos = new Vector3Int((int)(origin.x - 0.5f), (int)(origin.y - 0.5f));
            _insideTile.SetTile(intPos, _tile);
        }
        else
        {
            Vector3Int intPos = new Vector3Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f));
            _outsideTile.SetTile(intPos, _tile);
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
                    _insideTile.SetTile(intPos, _tile);
                }
                else
                {
                    _outsideTile.SetTile(intPos, _tile);
                }
            }
        }
    }

    private void ColliderInstantiate(Vector3 pos,int i, int j, Transform parent)
    {
        foreach(var tList in _insideCopyDic.Values)
        {
            if (tList.Contains(parent))
            {
                return;
            }
        }

        if (i <= 1) { pos.x += _size.x; }
        else if (i >= _size.x) { pos.x -= _size.x; }
        else if (j <= 1) { pos.y += _size.y; }
        else if (j >= _size.y) { pos.y -= _size.y; }
        var instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, parent);
        var col = instance.GetComponent<Collider2D>();
        instance.layer = 11;
        _outsideColliderList.Add(col);
        //if (pos.x < _loopRangeX.min || _loopRangeX.max < pos.x ||
        //    pos.y < _loopRangeY.min || _loopRangeY.max < pos.y)
        //{
        //    instance.layer = 12;
        //    _insideColliderList.Add(col);
        //}
        //else
        //{
        //    instance.layer = 11;
        //    _outsideColliderList.Add(col);
        //}

        //if (i == 1 || i == _size.x)
        //{
        //    if (j <= 1 || j >= _size.y)
        //    {
        //        if (j <= 1) { pos.y += _size.y; }
        //        if (j >= _size.y) { pos.y -= _size.y; }
        //        instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, _transform);
        //        col = instance.GetComponent<Collider2D>();
        //        if (j == 1 || j == _size.y)
        //        {
        //            _insideColliderList.Add(col);
        //        }
        //        else
        //        {
        //            _outsideColliderList.Add(col);
        //        }
        //    }
        //}
    }

    private void onInactive()
    {
        _material.color = new Color32(255, 255, 0, 100);

        _spriteMask.enabled = false;

        foreach(var col in _insiders)
        {
            SpriteRenderer renderer = col.GetComponent<SpriteRenderer>();
            renderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            if (_exitInsiders.ContainsKey(col))
            {
                var pos = col.transform.position;
                if(pos.x < _loopRangeX.min)
                {
                    pos.x += _size.x;
                }
                else if(pos.x > _loopRangeX.max)
                {
                    pos.x -= _size.x;
                }

                if (pos.y < _loopRangeY.min)
                {
                    pos.y += _size.y;
                }
                else if (pos.y > _loopRangeY.max)
                {
                    pos.y -= _size.y;
                }
                col.transform.position = pos;
            }

            if (col.CompareTag("Player"))
            {
                continue;
            }
            col.gameObject.layer--;
        }

        g_usable = false;
        _insideTile.ClearAllTiles();
        _outsideTile.ClearAllTiles();
        for(int i=0;i < _insideColliderList.Count; i++)
        {
            if (_insideColliderList[i] == null) { continue; }
            Destroy(_insideColliderList[i].gameObject);
        }
        for (int i = 0; i < _outsideColliderList.Count; i++)
        {
            if (_outsideColliderList[i] == null) { continue; }
            Destroy(_outsideColliderList[i].gameObject);
        }

        Dictionary<Collider2D, List<Transform>> workDic = new Dictionary<Collider2D, List<Transform>>(_insideCopyDic); 
        foreach(var col in workDic.Keys)
        {
            foreach(Transform t in _insideCopyDic[col])
            {
                if (t == null)
                {
                    continue;
                }
                Destroy(t.gameObject);
            }
        }
        Dictionary<Collider2D, Transform> workDic2 = new Dictionary<Collider2D, Transform>(_outsideCopyDic);
        foreach (var col in workDic2.Keys)
        {
            if (_outsideCopyDic[col] == null)
            {
                continue;
            }
            Destroy(_outsideCopyDic[col].gameObject);
        }
        _insideColliderList.Clear();
        _outsideColliderList.Clear();
        _insideCopyDic.Clear();
        _outsideCopyDic.Clear();
        _exitInsiders.Clear();
        _enterOutsiders.Clear();

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

    private void instantiateCopy()
    {
        foreach (var col in _exitInsiders.Keys)
        {
            if (!_insideCopyDic.ContainsKey(col))
            {
                GameObject obj = copy(col.transform);

                var pos = col.transform.position;
                pos -= new Vector3(_size.x,_size.y);

                List<Transform> tList = new List<Transform>();
                for(int i = 0; i < 3; i++)
                {
                    for(int j = 0; j < 3; j++)
                    {
                        if(i == 1 &&  j == 1) { continue; }

                        Vector3 setPos = pos;
                        setPos += new Vector3(_size.x * i, _size.y * j);
                        var instanceObj = Instantiate(obj, setPos, col.transform.rotation, col.transform);
                        tList.Add(instanceObj.transform);
                    }
                }

                _insideCopyDic.Add(col, tList);
                Destroy( obj );
            }
        }

        foreach(var col in _enterOutsiders)
        {
            if (!_outsideCopyDic.ContainsKey(col))
            {
                GameObject obj = copy(col.transform);

                var vec = _outsiders[col];

                var pos = col.transform.position;
                pos -= Vector3.Scale(vec, new Vector3(_size.x,_size.y));

                var instanceObj = Instantiate(obj, pos, col.transform.rotation, col.transform);

                _outsideCopyDic.Add(col, instanceObj.transform);
                Destroy(obj);
            }
        }
    }

    private GameObject copy(Transform t)
    {
        GameObject obj = new GameObject(t.name + "_copy");
        obj.layer = t.gameObject.layer;
        SpriteRenderer setRenderer = t.GetComponent<SpriteRenderer>();
        Rigidbody2D setRigidbody = t.GetComponent<Rigidbody2D>();

        obj.AddComponent<ParentDestroy>();
        obj.AddComponent(setRenderer);
        var rb = obj.AddComponent(setRigidbody);
        rb.isKinematic = true;
        rb.useAutoMass = false;

        if (t.CompareTag("Box"))
        {
            obj.AddComponent<BoxChild>();
        }

        var col = t.GetComponent<Collider2D>();

        switch (col)
        {
            case BoxCollider2D:
                BoxCollider2D setBoxCol = col as BoxCollider2D;
                obj.AddComponent(setBoxCol);
                break;
            case CircleCollider2D:
                CircleCollider2D setCircleCol = col as CircleCollider2D;
                obj.AddComponent(setCircleCol);
                break;
            case CapsuleCollider2D:
                CapsuleCollider2D setCapsuleCol = col as CapsuleCollider2D;
                obj.AddComponent(setCapsuleCol);
                break;
        }

        return obj;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        foreach(var col in _insideCopyDic.Keys)
        {
            if (_insideCopyDic[col].Contains(other.transform))
            {
                return;
            }
        }

        if (_outsideCopyDic.ContainsValue(other.transform))
        {
            return;
        }

        if(_insideColliderList.Contains(other) | _outsideColliderList.Contains(other))
        {
            return;
        }

        if (_outsiders.ContainsKey(other) && g_isActive)
        {
            if (!_enterOutsiders.Contains(other))
            {
                _enterOutsiders.Add(other);
            }
        }
        else
        {
            if (!_insiders.Contains(other) && !g_isActive)
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
                if (_exitInsiders.ContainsKey(other))
                {
                    _exitInsiders.Remove(other);
                }
            }
        }

        if (_enterOutsiders.Contains(other))
        {
            _enterOutsiders.Remove(other);
        }
    }

    public void OnStay(Collider2D other,Transform transform)
    {
        foreach (var col in _insideCopyDic.Keys)
        {
            if (_insideCopyDic[col].Contains(other.transform))
            {
                return;
            }
        }
        if (_outsideCopyDic.ContainsValue(other.transform))
        {
            return;
        }
        if(_insideColliderList.Contains(other) || _outsideColliderList.Contains(other))
        {
            return;
        }

        Vector2 vec = Vector2.zero;
        if (transform == _topT) { vec.y = 1; }
        if (transform == _bottomT) { vec.y = -1; }
        if (transform == _rightT) { vec.x = 1; }
        if (transform == _leftT) { vec.x = -1; }

        if (_insiders.Contains(other) && g_isActive)
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
            if (_enterOutsiders.Contains(other))
            {
                _enterOutsiders.Remove(other);
            }
        }

        if (_exitInsiders.ContainsKey(other))
        {
            _exitInsiders.Remove(other);
        }
    }

    public void OnEnter(Collider2D other, Transform transform)
    {

    }

    public void SetCrouching(bool isCrouching)
    {
        _isCrouching = isCrouching;
    }

    public Vector2Int GetSize()
    {
        return _size;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Fan : MonoBehaviour,IParentOnTrigger
{
    [SerializeField,Tooltip("風域に設置するTile")]
    private Tile _tile = null;
    [SerializeField,Tooltip("風の射程")]
    private int _range = 1;
    [SerializeField,Tooltip("与える速度(m/s)")]
    private float _force = 1f;
    [SerializeField,Tooltip("左を向ける")]
    private bool _inverse = false;
    [SerializeField,Tooltip("影響範囲を非表示にする")]
    private bool _invisible = false;
    [SerializeField, Tag,Tooltip("影響を与えるTag")]
    private List<string> _tagList = new List<string>() { "Player"};
    [SerializeField, Tag,Tooltip("遮られるTag")]
    private List<string> _blockTagList = new List<string>() { "Platform"}; 
    [SerializeField,Tooltip("初めから有効か")]
    private bool _enableOnAwake = true;
    private bool _enable = false;

    private Transform _transform = null;
    private Tilemap _tilemap = null, _tilemap_out;
    private TilemapRenderer _tilemapRenderer = null, _tilemapRenderer_out;
    private Dictionary<Collider2D, Rigidbody2D> _rbDic = new Dictionary<Collider2D, Rigidbody2D>();
    private int _direction = 1;
    private int _sizeX = 0;
    private Transform _outsideT = null;
    
    private void Reset()
    {
        Transform child = transform.GetChild(0);
        child.localPosition = new Vector3(-0.5f, -0.5f, 0);
    }

    private void Start()
    {
        _camera = Camera.main;
        _enable = _enableOnAwake;
        _transform = transform;
        Transform child1 = _transform.GetChild(0);
        _outsideT = _transform.GetChild(1);
        _tilemap = child1.GetComponent<Tilemap>();
        _tilemapRenderer = _tilemap.GetComponent<TilemapRenderer>();
        _tilemapRenderer.enabled = !_invisible;
        _tilemap_out = _outsideT.GetComponent<Tilemap>();
        _tilemapRenderer_out = _tilemap_out.GetComponent<TilemapRenderer>();
        _tilemapRenderer_out.enabled = !_invisible;
        _direction = _inverse ? -1 : 1;

        var frameObj = GameObject.FindGameObjectWithTag("Frame");
        _sizeX = frameObj.GetComponent<FrameLoop>().GetSizeX();
        SetTiles();
    }

    private void FixedUpdate()
    {
        _tilemapRenderer.enabled = _enable && !_invisible;
        if (!_enable) { return; }

        foreach (var rb in _rbDic.Values)
        {
            var currentPos = rb.position;
            currentPos += (Vector2)_transform.right * _direction * _force * Time.fixedDeltaTime;
            rb.position = currentPos;
        }
    }

    public void OnEnter(Collider2D other, Transform transform)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        if(transform == _outsideT)
        {
            if (other.CompareTag("Player"))
            {
                return;
            }
        }

        if(!_rbDic.ContainsKey(other))
        {
            var rb = other.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                _rbDic.Add(other, rb);
            }
        }
    }

    public void OnExit(Collider2D other, Transform transform)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        if (transform == _outsideT)
        {
            if (other.CompareTag("Player"))
            {
                return;
            }
        }

        if (_rbDic.ContainsKey(other))
        {
            _rbDic.Remove(other);
        }
    }
    public void OnStay(Collider2D other, Transform transform)
    {

    }

    private Camera _camera = null;
    public void FanLoopStarted()
    {
        _tilemap.ClearAllTiles();
        _tilemap_out.ClearAllTiles();
        bool inside = false;

        var pos = _transform.position;
        Vector3Int intPos = Vector3Int.zero;

        for(int i = 0; i <= _range; i++,pos.x += _direction,intPos.x += _direction)
        {
            Ray ray = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos));

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10 , 1<<6 | 1<<8);

            if (i == 0)
            {
                inside = hits.Length > 0; 
            }
            else if (inside)//ファンがフレームの中か
            {
                //設置したい位置にオブジェクトがある場合
                if(hits.Length > 0)
                {
                    bool instance_here = false, blocking = false;
                    foreach (var hit in hits)
                    {
                        //障害物があったらblockingをtrueに
                        if (_blockTagList.Contains(hit.transform.tag))
                        {
                            blocking = true;
                        }
                        //フレームの中ならinstance_hereをtrueに
                        if (hit.transform.CompareTag("Frame"))
                        {
                            instance_here = true;
                        }
                    }
                    //障害物のある座標に生成しようとしたらreturn
                    if (blocking && instance_here)
                    {
                        return;
                    }
                    //フレームの中なら座標をずらさず生成して次の座標を確認
                    if (instance_here)
                    {
                        _tilemap.SetTile(intPos, _tile);
                        continue;
                    }
                }
                var pos_sub = pos;
                pos_sub.x -= _direction * _sizeX;
                Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));
                //生成先にRaycast
                RaycastHit2D hit_sub = Physics2D.Raycast(ray_sub.origin, ray_sub.direction, 10, 1 << 6);

                //障害物があったらreturn
                if (hit_sub) { return; }
                var setPos = intPos;
                setPos.x -= _direction * _sizeX;
                _tilemap.SetTile(setPos, _tile);
            }
            else
            {
                //設置したい位置にオブジェクトがある場合
                if (hits.Length > 0)
                {
                    bool instance_here = true, blocking = false;
                    foreach (var hit in hits)
                    {
                        //障害物があったらblockingをtrueに
                        if (_blockTagList.Contains(hit.transform.tag))
                        {
                            blocking = true;
                        }
                        //フレームの中ならinstance_hereをfalseに
                        if (hit.transform.CompareTag("Frame"))
                        {
                            instance_here = false;
                        }
                    }
                    //フレームの中なら座標をずらして生成
                    if (!instance_here)
                    {
                        var pos_sub = pos;
                        pos_sub.x += _direction * _sizeX;
                        Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));
                        //生成先にRaycast
                        RaycastHit2D hit_sub = Physics2D.Raycast(ray_sub.origin, ray_sub.direction, 10, 1 << 6);

                        //障害物があったらreturn
                        if (hit_sub) { return; }
                        var setPos = intPos;
                        setPos.x += _direction * _sizeX;
                        _tilemap.SetTile(setPos, _tile);
                        continue;

                    }
                    //障害物のある座標に生成しようとしたらreturn
                    if (blocking && instance_here)
                    {
                        return;
                    }
                }
                _tilemap.SetTile(intPos, _tile);
            }
        }
    }

    public void FanLoopCanceled()
    {
        SetTiles();
    }

    private void SetTiles()
    {
        _tilemap.ClearAllTiles();
        _tilemap_out.ClearAllTiles();

        var pos = _transform.position;
        Vector3Int intPos = Vector3Int.zero;

        for (int i = 0; i < _range; i++)
        {
            pos.x += _direction;
            intPos.x += _direction;
            Ray ray = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos));
            //Debug.DrawRay(ray.origin, ray.direction*10,Color.red,0.1f);

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10, 1 << 6 | 1 << 8);
            foreach(var hit in hits)
            {
                if (_blockTagList.Contains(hit.transform.tag))
                {
                    return;
                }
            }
            _tilemap.SetTile(intPos, _tile);
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

#if else
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
        _isEnabled = supply;
    }
#endif
}

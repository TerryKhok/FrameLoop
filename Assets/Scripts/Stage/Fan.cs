using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Fan : MonoBehaviour,IParentOnTrigger
{
    [SerializeField]
    private Tile _tile = null;
    [SerializeField]
    private int _range = 1;
    [SerializeField]
    private float _force = 1f;
    [SerializeField]
    private bool _inverse = false;
    [SerializeField]
    private bool _invisible = false;
    [SerializeField, Tag]
    private List<string> _tagList = new List<string>() { "Player"};
    [SerializeField]
    private bool _enableOnAwake = true;
    private bool _enable = true;

    private Transform _transform = null;
    private Tilemap _tilemap = null;
    private Dictionary<Collider2D, Rigidbody2D> _rbDic = new Dictionary<Collider2D, Rigidbody2D>();
    private int _direction = 1;
    private int _sizeX = 0;
    
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
        Transform child = _transform.GetChild(0);
        _tilemap = child.GetComponent<Tilemap>();
        child.AddComponent<ChildOnTrigger>();
        TilemapRenderer renderer = _tilemap.GetComponent<TilemapRenderer>();
        renderer.enabled = !_invisible;
        _direction = _inverse ? -1 : 1;

        var frameObj = GameObject.FindGameObjectWithTag("Frame");
        _sizeX = frameObj.GetComponent<FrameLoop>().GetSizeX();
        Vector3Int intPos = Vector3Int.zero;
        for (int i=0; i < _range; i++)
        {
            intPos.x += _direction;
            _tilemap.SetTile(intPos, _tile);
        }
    }

    private void FixedUpdate()
    {
        if (!_enable) { return; }

        foreach (var rb in _rbDic.Values)
        {
            var currentPos = rb.position;
            currentPos += (Vector2)_transform.right * _force * Time.fixedDeltaTime;
            rb.position = currentPos;
        }
    }

    public void OnEnter(Collider2D collision)
    {
        if (!_tagList.Contains(collision.tag)) { return; }

        if(!_rbDic.ContainsKey(collision))
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                _rbDic.Add(collision, rb);
            }
        }
    }

    public void OnExit(Collider2D collision)
    {
        if (!_tagList.Contains(collision.tag)) { return; }

        if (_rbDic.ContainsKey(collision))
        {
            _rbDic.Remove(collision);
        }
    }
    public void OnStay(Collider2D collision)
    {

    }

    private Camera _camera = null;
    public void FanLoopStarted()
    {
        bool inside = false;

        var pos = _transform.position;
        Vector3Int intPos = Vector3Int.zero;

        for(int i = 0; i <= _range; i++,pos.x += _direction,intPos.x += _direction)
        {
            Ray ray = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos));
            //Debug.DrawRay(ray.origin, ray.direction*10,Color.red,0.1f);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10, 1<<8);

            if (i == 0) { inside = hit; }
            else if (inside)
            {
                if (!hit)
                {
                    _tilemap.SetTile(intPos, null);

                    var setPos = intPos;
                    setPos.x -= _direction * _sizeX;
                    _tilemap.SetTile(setPos, _tile);
                }
            }
            else
            {
                if (hit)
                {
                    _tilemap.SetTile(intPos, null);

                    var setPos = intPos;
                    setPos.x += _direction * _sizeX;
                    _tilemap.SetTile(setPos, _tile);
                }
            }
        }
    }

    public void FanLoopCanceled()
    {
        _tilemap.ClearAllTiles();

        Vector3Int intPos = Vector3Int.zero;
        for (int i = 0; i < _range; i++)
        {
            intPos.x += _direction;
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

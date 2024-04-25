using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fan : MonoBehaviour,IParentOnTrigger
{
    private enum Direction
    {
        UP,DOWN,LEFT,RIGHT
    }

    [SerializeField,Tooltip("風の方向")]
    private Direction _direction = Direction.RIGHT;
    [SerializeField,Tooltip("風域に設置するTile")]
    private Tile _tile = null;
    [SerializeField,Tooltip("風の射程")]
    private int _range = 1;
    [SerializeField,Tooltip("与える速度(m/s)")]
    private float _force = 1f;
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
    private Vector3Int _frameSize = Vector3Int.zero;
    private Transform _outsideT = null;
    private Vector3Int _actualDirection = Vector3Int.right;
    
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
        switch (_direction)
        {
            case Direction.UP:
                _actualDirection = Vector3Int.up;
                break;
            case Direction.DOWN:
                _actualDirection = Vector3Int.down;
                break;
            case Direction.RIGHT:
                _actualDirection = Vector3Int.right;
                break;
            case Direction.LEFT:
                _actualDirection = Vector3Int.left;
                break;
        }
        Transform child1 = _transform.GetChild(0);
        _outsideT = _transform.GetChild(1);
        _tilemap = child1.GetComponent<Tilemap>();
        _tilemapRenderer = _tilemap.GetComponent<TilemapRenderer>();
        _tilemapRenderer.enabled = !_invisible;
        _tilemap_out = _outsideT.GetComponent<Tilemap>();
        _tilemapRenderer_out = _tilemap_out.GetComponent<TilemapRenderer>();
        _tilemapRenderer_out.enabled = !_invisible;

        var frameObj = GameObject.FindGameObjectWithTag("Frame");
        _frameSize = (Vector3Int)frameObj.GetComponent<FrameLoop>().GetSize();
        SetTiles();
    }

    private void FixedUpdate()
    {
        _tilemapRenderer.enabled = _enable && !_invisible;
        if (!_enable) { return; }

        Vector2 forceDirection = new Vector2(_actualDirection.x, _actualDirection.y);
        foreach (var rb in _rbDic.Values)
        {
            var currentPos = rb.position;
            currentPos += (Vector2)_transform.right * forceDirection * _force * Time.fixedDeltaTime;
            rb.position = currentPos;
        }
    }

    public void OnEnter(Collider2D collision, Transform transform)
    {
        if (!_tagList.Contains(collision.tag)) { return; }

        if(transform == _outsideT)
        {
            if (collision.CompareTag("Player"))
            {
                return;
            }
        }

        if(!_rbDic.ContainsKey(collision))
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                _rbDic.Add(collision, rb);
            }
        }
    }

    public void OnExit(Collider2D collision, Transform transform)
    {
        if (!_tagList.Contains(collision.tag)) { return; }

        if (transform == _outsideT)
        {
            if (collision.CompareTag("Player"))
            {
                return;
            }
        }

        if (_rbDic.ContainsKey(collision))
        {
            _rbDic.Remove(collision);
        }
    }
    public void OnStay(Collider2D collision, Transform transform)
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

        for(int i = 0; i <= _range; i++,pos += (Vector3)_actualDirection,intPos += (Vector3Int)_actualDirection)
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
                        SetTile(intPos, Quaternion.FromToRotation(Vector3.right,_actualDirection), _tilemap, _tile);
                        continue;
                    }
                }
                var pos_sub = pos;
                pos_sub -= _actualDirection * _frameSize;
                Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));
                //生成先にRaycast
                RaycastHit2D hit_sub = Physics2D.Raycast(ray_sub.origin, ray_sub.direction, 10, 1 << 6);

                //障害物があったらreturn
                if (hit_sub) { return; }
                var setPos = intPos;
                setPos -= _actualDirection * _frameSize;
                SetTile(setPos, Quaternion.FromToRotation(Vector3.right,_actualDirection), _tilemap, _tile);
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
                        pos_sub += _actualDirection * _frameSize;
                        Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));
                        //生成先にRaycast
                        RaycastHit2D hit_sub = Physics2D.Raycast(ray_sub.origin, ray_sub.direction, 10, 1 << 6);

                        //障害物があったらreturn
                        if (hit_sub) { return; }
                        var setPos = intPos;
                        setPos += _actualDirection * _frameSize;
                        SetTile(setPos, Quaternion.FromToRotation(Vector3.right,_actualDirection), _tilemap, _tile);
                        continue;

                    }
                    //障害物のある座標に生成しようとしたらreturn
                    if (blocking && instance_here)
                    {
                        return;
                    }
                }
                SetTile(intPos, Quaternion.FromToRotation(Vector3.right, _actualDirection), _tilemap, _tile);
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
            pos += _actualDirection;
            intPos += _actualDirection;
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
            SetTile(intPos, Quaternion.FromToRotation(Vector3.right,_actualDirection),_tilemap, _tile);
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

    private void SetTile(Vector3Int pos, Quaternion rot, Tilemap tilemap, TileBase tile)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTransformMatrix(pos, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*  ProjectName :FrameLoop
 *  ClassName   :Fan
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :風のタイルを生成する
 *               
 *  Created     :2024/04/27
 */
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
    [SerializeField,Tooltip("左向きのアテリアル")]
    private Material _leftMaterial = null;
    [SerializeField,Tooltip("上向きのアテリアル")]
    private Material _upMaterial = null;
    [SerializeField,Tooltip("下向きのアテリアル")]
    private Material _downMaterial = null;
    private bool _isEnable = false;

    private Transform _transform = null;
    private Tilemap _tilemapOutside = null, _tilemapInside;
    private TilemapRenderer _tilemapRenderer = null, _tilemapRenderer_out;

    //風に触れてるRigidbodyのdictionary
    private Dictionary<Collider2D, Rigidbody2D> _rbDic = new Dictionary<Collider2D, Rigidbody2D>();

    private Vector3Int _frameSize = Vector3Int.zero;
    private Transform _outsideT = null;
    private Vector3Int _actualDirection = Vector3Int.right;

    private Animator _animator;

    private PlayerInfo _playerInfo;
    private Camera _camera = null;

    private bool _playerBoxFlag = false;
    private bool windSoundFlag = false;


    //Tilemapの座標を調整
    private void Reset()
    {
        Transform child = transform.GetChild(0);
        child.localPosition = new Vector3(-0.5f, -0.5f, 0);

        child = transform.GetChild(1);
        child.localPosition = new Vector3(-0.5f,-0.5f, 0);
    }

    private void Start()
    {
        AudioManager.instance.Stop("Wind");

        _camera = Camera.main;
        _playerInfo = PlayerInfo.Instance;
        _isEnable = _enableOnAwake;
        _transform = transform;

        _animator = GetComponentInChildren<Animator>();

        //各種Componentを取得--------------------------------------------------
        Transform child1 = _transform.GetChild(0);
        _outsideT = _transform.GetChild(1);
        _tilemapOutside = _outsideT.GetComponent<Tilemap>();
        _tilemapRenderer = _tilemapOutside.GetComponent<TilemapRenderer>();
        _tilemapRenderer.enabled = !_invisible;

        _tilemapInside = child1.GetComponent<Tilemap>();
        _tilemapRenderer_out = _tilemapInside.GetComponent<TilemapRenderer>();
        _tilemapRenderer_out.enabled = !_invisible;
        //--------------------------------------------------------------------

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Transform rendererTransform = spriteRenderer.transform;

        //_directionで風の発射方向を決める
        switch (_direction)
        {
            case Direction.UP:
                _actualDirection = Vector3Int.up;
                _tilemapRenderer.material = _upMaterial;
                _tilemapRenderer_out.material = _upMaterial;
                rendererTransform.localEulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.DOWN:
                _actualDirection = Vector3Int.down;
                _tilemapRenderer.material = _downMaterial;
                _tilemapRenderer_out.material = _downMaterial;
                rendererTransform.localEulerAngles = new Vector3(0, 0, -90);
                break;
            case Direction.RIGHT:
                _actualDirection = Vector3Int.right;
                break;
            case Direction.LEFT:
                _actualDirection = Vector3Int.left;
                _tilemapRenderer.material = _leftMaterial;
                _tilemapRenderer_out.material = _leftMaterial;
                rendererTransform.localEulerAngles = new Vector3(0, 180, 0);
                break;
        }

        //フレームのサイズを取得
        var frameObj = GameObject.FindGameObjectWithTag("Frame");
        _frameSize = (Vector3Int)frameObj.GetComponent<FrameLoop>().GetSize();

        //風を発射
        SetTiles();
    }

    private void OnDestroy()
    {
        AudioManager.instance.Stop("Wind");
    }

    private void Update()
    {
        _animator.SetBool("isEnable", _isEnable);

        if (!windSoundFlag && _isEnable)
        {
            AudioManager.instance.Play("Wind");
            windSoundFlag = true;
        }
        if (windSoundFlag && !_isEnable)
        {
            AudioManager.instance.Stop("Wind");
            windSoundFlag = false;
        }
    }

    private void FixedUpdate()
    {
        //有効で見える状態ならrendererを有効にする
        _tilemapRenderer.enabled = _isEnable && !_invisible;

        if (!_isEnable || Goal.Instance.g_clear) 
        {
            return; 
        }

        _playerBoxFlag = false;

        //発射方向をVector2に変換
        Vector2 forceDirection = new Vector2(_actualDirection.x, _actualDirection.y);
        List<Collider2D> temp = new List<Collider2D>();

        //風に触れているrigidbodyを全て確認
        foreach (var item in _rbDic)
        {
            if(item.Value == null)
            {
                temp.Add(item.Key);
                continue;
            }

            //Playerか、押してる箱ならフラグを立てる
            if(item.Value.transform == _playerInfo.g_transform || 
               item.Value.transform == _playerInfo.g_box)
            {
                //既にフラグが立っていたらreturn
                if(_playerBoxFlag)
                {
                    return;
                }

                //Playerか箱かに関わらずPlayerのrigidbodyを指定
                var playerRb = _playerInfo.g_rb;

                //発射方向に一定速度で移動させる
                var currentPlayerPos = playerRb.position;
                currentPlayerPos += forceDirection * _force * Time.fixedDeltaTime;
                playerRb.position = currentPlayerPos;

                _playerBoxFlag = true;
                if (_playerInfo.g_takeUpFg)
                {
                    _playerInfo.g_box.GetComponent<Box>().SetMovable(true);
                }
                return;
            }

            //発射方向に一定速度で移動させる
            var currentPos = item.Value.position;

            Ray ray = new Ray(currentPos, forceDirection);
            string layerName = LayerMask.LayerToName(item.Value.gameObject.layer);
            LayerMask mask = 0;
            if(layerName[0] == 'O')
            {
                mask |= 1 << LayerMask.NameToLayer("OPlatform");
                mask |= 1 << LayerMask.NameToLayer("OPlayer");
                mask |= 1 << LayerMask.NameToLayer("OBox");
            }
            else
            {
                mask |= 1 << LayerMask.NameToLayer("IPlatform");
                mask |= 1 << LayerMask.NameToLayer("IPlayer");
                mask |= 1 << LayerMask.NameToLayer("IBox");
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 0.6f, mask);
            bool hitFlag = false;

            if(hits.Length > 0)
            {
                foreach(var hit in hits)
                {
                    // 自分以外のオブジェクトが前にあったら移動しない
                    if(hit.transform != item.Value.transform)
                    {
                        hitFlag = true;
                        continue;
                    }
                }
            }

            if (hitFlag)
            {
                if(item.Value.CompareTag("Box"))
                {
                    var box = item.Value.GetComponent<Box>();
                    box.AdjustPosition();
                }
                continue;
            }

            currentPos += forceDirection * _force * Time.fixedDeltaTime;
            item.Value.position = currentPos;
        }

        foreach(var col in temp)
        {
            _rbDic.Remove(col);
        }
    }

    //Tilemapに触れているcolliderを受け取る
    public void OnStay(Collider2D other, Transform transform)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        // 箱のコピーなら親で判断する
        if(other.GetComponent<BoxChild>() != null)
        {
            other = other.GetComponentInParent<Collider2D>();
        }

        if(!_rbDic.ContainsKey(other))
        {
            //rigidbodyを取得してdictionaryへ追加
            var rb = other.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                _rbDic.Add(other, rb);

                //if (other.CompareTag("Box"))
                //{
                //    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                //}
            }
        }
    }

    //Tilemapから離れたcolliderを受け取る
    public void OnExit(Collider2D other, Transform transform)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        // 箱のコピーなら親で判断する
        if (other.GetComponent<BoxChild>() != null)
        {
            other = other.GetComponentInParent<Collider2D>();
        }

        //dictionaryに存在していたら削除する
        if (_rbDic.ContainsKey(other))
        {
            _rbDic.Remove(other);

            //if (other.CompareTag("Box"))
            //{
            //    var rb = other.GetComponent<Rigidbody2D>();
            //    rb.constraints |= RigidbodyConstraints2D.FreezePositionX;

            //    other.GetComponent<Box>().SetMovable(false);
            //}
        }
    }
    public void OnEnter(Collider2D other, Transform transform)
    {
        //IParentOnTriggerの必要メソッド
    }

    //フレームが有効になったときに一度呼ばれる
    public void FanLoopStarted()
    {
        StartCoroutine("windLoop");
    }

    //フレームが無効になったときに一度呼ばれる
    public void FanLoopCanceled()
    {
        StartCoroutine("asyncSetTiles");
    }

    //フレームがあるときの風の生成
    private IEnumerator windLoop()
    {
        if (!_isEnable) 
        {
            yield break; 
        }

        //フレームの終了を待つ
        //待たないとフレームで生成したColliderにRayが当たらない
        yield return new WaitForEndOfFrame();

        //風に触れてるオブジェクトのリストをクリア
        _rbDic.Clear();

        //Tilemapを全てクリア
        _tilemapOutside.ClearAllTiles();
        _tilemapInside.ClearAllTiles();

        //Fanがフレームの内側にあるか
        bool inside = false, enterFlag = false;

        var pos = _transform.position;
        Vector3Int intPos = Vector3Int.zero;

        //フレームにだけ当たるLayerMaskを作成
        LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

        //風の最大距離+1回分のループ
        for (int i = 0; i <= _range; i++, pos += (Vector3)_actualDirection, intPos += _actualDirection)
        {
            //Fanの位置から1マスずつずらしてRayを飛ばす
            Ray ray = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos));

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10, mask);

            if (i == 0)
            {
                if(hits.Length > 0)
                {
                    //フレームの内側ならRayを内側の足場に当たるようにする
                    mask |= 1 << LayerMask.NameToLayer("IPlatform");
                    inside = true;
                }
                else
                {
                    //フレームの外側ならRayを外側の足場に当たるようにする
                    mask |= 1 << LayerMask.NameToLayer("OPlatform");
                }

            }
            else if (inside)//ファンがフレームの中か
            {
                //設置したい位置にオブジェクトがある場合
                if (hits.Length > 0)
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
                    //障害物のある座標に生成しようとしたら終了
                    if (blocking && instance_here)
                    {
                        yield break;
                    }
                    //フレームの中なら座標をずらさず生成して次の座標を確認
                    if (instance_here)
                    {
                        SetTile(intPos, _actualDirection, _tilemapInside, _tile);
                        continue;
                    }
                }

                var pos_sub = pos;
                pos_sub -= _actualDirection * _frameSize;
                Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));

                //生成先にRaycast
                RaycastHit2D[] hits_sub = Physics2D.RaycastAll(ray_sub.origin, ray_sub.direction, 10, 1 << LayerMask.NameToLayer("IPlatform") | 1 << LayerMask.NameToLayer("Frame"));

                bool frameOutside = true;
                foreach (var hit in hits_sub)
                {
                    //障害物があったら終了
                    if (_blockTagList.Contains(hit.transform.tag))
                    {
                        yield break;
                    }

                    if (hit.transform.CompareTag("Frame"))
                    {
                        frameOutside = false;
                    }
                }

                //フレームの外側なら終了
                if (frameOutside)
                {
                    yield break;
                }

                var setPos = intPos;
                setPos -= _actualDirection * _frameSize;
                SetTile(setPos, _actualDirection, _tilemapInside, _tile);
            }
            else
            {
                //設置したい位置にオブジェクトがある場合
                if (hits.Length > 0)
                {
                    bool blocking = false;
                    foreach (var hit in hits)
                    {
                        //障害物があったらblockingをtrueに
                        if (_blockTagList.Contains(hit.transform.tag))
                        {
                            blocking = true;
                        }
                        //フレームの中ならenterFlagをtrueに
                        if (hit.transform.CompareTag("Frame"))
                        {
                            enterFlag = true;
                        }
                    }


                    //障害物のある座標に生成しようとしたら終了
                    if (blocking && !enterFlag)
                    {
                        yield break;
                    }
                }

                //フレームの中なら座標をずらして生成
                if (enterFlag)
                {
                    var pos_sub = pos;
                    pos_sub += _actualDirection * _frameSize;
                    Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));

                    //生成先にRaycast
                    RaycastHit2D hit_sub = Physics2D.Raycast(ray_sub.origin, ray_sub.direction, 10, 1 << LayerMask.NameToLayer("OPlatform"));

                    //障害物があったら終了
                    if (hit_sub && _blockTagList.Contains(hit_sub.transform.tag))
                    {
                        yield break;
                    }

                    var setPos = intPos;
                    setPos += _actualDirection * _frameSize;
                    SetTile(setPos, _actualDirection, _tilemapOutside, _tile);
                    continue;

                }
                SetTile(intPos, _actualDirection, _tilemapOutside, _tile);
            }
        }
    }

    //フレームがない時の風の生成
    private void SetTiles()
    {
        if (!_isEnable) { return; }

        //風に触れてるオブジェクトのリストをクリア
        _rbDic.Clear();

        //Tilemapを全てクリア
        _tilemapOutside.ClearAllTiles();
        _tilemapInside.ClearAllTiles();

        var pos = _transform.position;
        Vector3Int intPos = Vector3Int.zero;

        //風の最大距離分ループ
        for (int i = 0; i < _range; i++)
        {
            pos += _actualDirection;
            intPos += _actualDirection;
            Ray ray = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos));
            //Debug.DrawRay(ray.origin, ray.direction*10,Color.red,0.1f);

            //フレームの外側の足場に当たるLayermaskを作成
            LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform");

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10, mask);

            //障害物に当たったら終了
            foreach(var hit in hits)
            {
                if (_blockTagList.Contains(hit.transform.tag))
                {
                    return;
                }
            }
            SetTile(intPos, _actualDirection,_tilemapOutside, _tile);
        }
    }

    //フレーム終了を待ってから風を生成する
    private IEnumerator asyncSetTiles()
    {
        yield return new WaitForEndOfFrame();

        SetTiles();
    }

    //有効かどうかを引数で変更する
    public void SetEnable(bool enable)
    {
        _isEnable = enable;

        if (FrameLoop.Instance.g_isActive)
        {
            FanLoopStarted();
        }
        else
        {
            SetTiles();
        }
    }

    //有効か無効かを反転させる
    public void SwitchEnable()
    {
        _isEnable = !_isEnable;

        if (FrameLoop.Instance.g_isActive)
        {
            FanLoopStarted();
        }
        else
        {
            SetTiles();
        }
    }

    //タイルの向きを指定してセットする
    private void SetTile(Vector3Int pos, Vector3 targetVec, Tilemap tilemap, TileBase tile)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, targetVec);

        if(rot.y == 1)
        {
            rot = new Quaternion(0, 0, 1, 0);
        }

        tilemap.SetTile(pos, tile);
        tilemap.SetTransformMatrix(pos, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
    }
}

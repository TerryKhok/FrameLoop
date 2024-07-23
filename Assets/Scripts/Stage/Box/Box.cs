using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Box
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :箱の挙動を管理するクラス
 *               破壊可能な床の破壊、プレイヤーに掴まれている時の移動
 *               
 *  Created     :2024/04/27
 */
public class Box : MonoBehaviour,IBox
{
    private float _height = 0f;
    private Transform _transform, _playerTransform;
    [SerializeField,Tooltip("箱の横幅")]
    private float _width = 1f;
    [SerializeField,Tooltip("破壊するのに必要な高さ")]
    private float _breakHeight = 5f;
    [SerializeField,Tag,Tooltip("破壊可能なTag")]
    private List<string> _tagList = new List<string>() { "Breakable"};

    private Rigidbody2D _rb = null;
    private BoxCollider2D _collider2D = null;

    private PlayerInfo _playerInfo = null;
    private PlayerMove _playerMove = null;

    private Vector2 _offset = Vector2.zero;

    private List<Transform> _copyList = new List<Transform>();

    private bool _movable = false;
    private bool soundFlag = false;

    private void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<BoxCollider2D>();

        _playerInfo = PlayerInfo.Instance;

        //y軸以外の動きを制限
        //_rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    private void Update()
    {
        if (!_playerInfo.g_takeUpFg)
        {
            holdCancel();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(UpdateLateFixedUpdate));
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(UpdateLateFixedUpdate));
    }

    //FixedUpdateの後にLateFixedUpdateを呼び出す
    private IEnumerator UpdateLateFixedUpdate()
    {
        var waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            yield return waitForFixedUpdate;
            LateFixedUpdate();
        }
    }

    //FixedUpdateの後に呼び出されるメソッド
    private void LateFixedUpdate()
    {
        platformBreak();
        isHold();
    }

    //破壊可能な床を壊す
    private void platformBreak()
    {
        //最高点の座標を更新
        if (_height < _transform.position.y)
        {
            _height = _transform.position.y;
        }

        Ray ray = new Ray(_transform.position, Vector3.down);
        RaycastHit2D[] hits;
        Vector2 size = new Vector2(_width / 2, 0.5f);

        LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform") | 
                         1 << LayerMask.NameToLayer("OBox") | 
                         1 << LayerMask.NameToLayer("OPlayer");
        if(gameObject.layer == LayerMask.NameToLayer("IBox"))
        {
            mask = 1 << LayerMask.NameToLayer("IPlatform") |
                   1 << LayerMask.NameToLayer("IBox") |
                   1 << LayerMask.NameToLayer("IPlayer");
        }

        hits = Physics2D.BoxCastAll(ray.origin, size, 0, ray.direction, 1, mask);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                //Rayが当たったのが自身ならcontinueする
                if(hit.transform == _transform) { continue; }

                //空中に押し出されたら掴みをキャンセルする
                if (hit.distance > 0.3f && _rb.velocity.y <= -0.1f)
                {
                    holdCancel();
                    return;
                }

                if (_tagList.Contains(hit.transform.tag))
                {
                    //最高点との差が一定以上なら破壊する
                    //最高点のリセットは行わない
                    if (_height - _transform.position.y >= _breakHeight)
                    {
                        Destroy(hit.transform.gameObject);
                    }

                    return;
                }
                else
                {
                    //地面に触れたら最高点をリセット
                    _height = _transform.position.y;
                    return;
                }
            }

            //自身以外にRayが当たってなかったら掴みをキャンセル
            if(_rb.velocity.y <= -0.1f)
            {
                holdCancel();
            }
        }
        else if(_rb.velocity.y <= -0.1f)
        {
            holdCancel();
        }
    }

    private void holdCancel()
    {
        if(_playerTransform == null)
        {
            return;
        }

        //箱を押す音を止める
        AudioManager.instance.Stop("Box Pull");
        soundFlag = false;

        //--------------------------------------------------------------------------------
        //プレイヤーと箱の当たり判定を復活
        //--------------------------------------------------------------------------------
        var playerCol = _playerTransform.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(_collider2D, playerCol,false);


        foreach (var boxCopy in _copyList)
        {
            var boxCopyCol = boxCopy.GetComponent<Collider2D>();

            foreach (var playerCopy in _playerInfo.GetCopyList())
            {
                var playerCopyCol = playerCopy.GetComponent<Collider2D>();

                Physics2D.IgnoreCollision(boxCopyCol, playerCopyCol,false);
            }

            Physics2D.IgnoreCollision(boxCopyCol, playerCol,false);
        }
        //---------------------------------------------------------------------------------

        _playerTransform = null;
        _playerInfo.g_takeUpFg = false;
        _playerInfo.g_box = null;
        _playerInfo.g_wall = 0;
        _playerInfo.g_boxDirection = 0;
        SetOffset(Vector2.zero);

        AdjustPosition();
    }

    //プレイヤーが箱を持っているときの処理
    private void isHold()
    {
        if(_playerTransform == null) { return; }

        //プレイヤーが移動中してない時は音を止める
        if (_playerMove._isMoving == false)
        {
            AudioManager.instance.Stop("Box Pull");
            soundFlag = false;
        }
        else
        {
            if(!soundFlag)
            {
                AudioManager.instance.Play("Box Pull");
                soundFlag = true;
            }
        }

        var pos = _rb.position;
        var direction = new Vector2(_playerInfo.g_currentInputX, 0);

        //入力がないときはg_wallを更新しない
        if (direction.x == 0 && !_movable)
        {
            return;
        }

        Ray ray = new Ray(pos, direction);
        RaycastHit2D[] hits;
        Vector2 size = new Vector2((_width+0.1f) / 2, 0.5f);


        //自分と同じレイヤーのBoxが進行方向にあるかチェック
        LayerMask mask = 1 << gameObject.layer;

        if (LayerMask.LayerToName(gameObject.layer)[0] == 'I')
        {
            mask |= 1 << LayerMask.NameToLayer("IPlatform");
        }
        else
        {
            mask |= 1 << LayerMask.NameToLayer("OPlatform");
        }

        bool hitWall = false;
        bool hitBox = true;
        int count = 0;
        Vector3 origin = pos;
        List<Transform> hittenBoxList = new List<Transform>() { _transform };
        while (hitBox)
        {
            ++count;
            hits = Physics2D.BoxCastAll(origin, size, 0, ray.direction, 0.3f, mask);

            hitBox = false;

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    //自分を除外して、座標を移動させる
                    if (hittenBoxList.Contains(hit.transform)) { continue; }

                    if (hit.transform.CompareTag("Box"))
                    {
                        var rb = hit.transform.GetComponent<Rigidbody2D>();
                        pos.x += _width * ray.direction.normalized.x;
                        rb.position = pos;
                        hittenBoxList.Add(hit.transform);
                        origin = hit.transform.position;
                        hitBox = true;

                        continue;
                    }

                    hitWall = true;
                    _playerInfo.g_wall = ray.direction.normalized.x;
                }
            }
        }

        if (!hitWall)
        {
            _playerInfo.g_wall = 0;
        }

        if(hitWall)
        {
            return;
        }

        pos = _playerTransform.position;

        //箱とプレイヤーの座標のズレを合わせる
        pos.y -= 0.5f; 

        float offset = 0.95f;
        int relativeDirection = _playerInfo.g_currentInputX * _playerInfo.g_boxDirection;

        if (relativeDirection < 0)
        {
            offset = 0.85f;
        }
        
        if(_playerInfo.g_walkCancel)
        {
            offset = 1.0f;
        }

        pos += (Vector2)_playerTransform.right * offset;

        //箱かプレイヤーのどちらかがループしている場合は座標をずらす
        pos += _offset;

        //x座標をプレイヤーの座標から一定距離ずらした位置にする
        _rb.position = pos;

    }

    //箱を移動させる基準のtransformを受け取る
    public void Hold(Transform t)
    {
        if(t == null)
        {
            holdCancel();
            return;
        }
        _playerTransform = t;

        _playerInfo.g_box = _transform;
        _playerInfo.g_takeUpFg = true;

        //--------------------------------------------------------------------------------
        //プレイヤーと箱の当たり判定を無くす
        //--------------------------------------------------------------------------------
        var playerCol = _playerTransform.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(_collider2D, playerCol);


        foreach (var boxCopy in _copyList)
        {
            var boxCopyCol = boxCopy.GetComponent<Collider2D>();

            foreach (var playerCopy in _playerInfo.GetCopyList())
            {
                var playerCopyCol = playerCopy.GetComponent<Collider2D>();

                Physics2D.IgnoreCollision(boxCopyCol, playerCopyCol);
            }

            Physics2D.IgnoreCollision(boxCopyCol, playerCol);
        }
        //---------------------------------------------------------------------------------

        _playerMove = PlayerInfo.Instance.g_transform.GetComponent<PlayerMove>();

        //箱を押す音を再生
        if(!soundFlag)
        {
            AudioManager.instance.Play("Box Pull");
            soundFlag = true;
        }
    }

    public void AdjustPosition()
    {
        //座標が中途半端な時に修正する処理---------------------------------------------------
        Vector2 pos = _transform.position;
        var gap = new Vector2(pos.x % 0.5f, pos.y % 0.5f);

        //次のグリッドのほうが近いときは次のグリッドからの差に変換する
        if (gap.x > 0.25f)
        {
            gap.x = gap.x - 0.5f;
        }
        else if (gap.x < -0.25f)
        {
            gap.x = gap.x + 0.5f;
        }

        if (gap.y > 0.25f)
        {
            gap.y = gap.y - 0.5f;
        }
        else if (gap.y < -0.25f)
        {
            gap.y = gap.y + 0.5f;
        }

        var absGap = new Vector2(Mathf.Abs(gap.x), Mathf.Abs(gap.y));

        //一番近くのグリッドからの距離の絶対値で比較する(値の範囲は0.0~0.25)
        //ズレが大きいなら座標の補正をしない
        if (absGap.x > 0.15f)
        {
            gap.x = 0;
        }

        if (absGap.y > 0.15f)
        {
            gap.y = 0;
        }

        pos -= gap;
        _transform.position = pos;
        //-----------------------------------------------------------
    }

    public void SetOffset(Vector2 vec)
    {
        _offset = vec;
    }

    public Vector2 GetOffset()
    {
        return _offset;
    }

    public void AddCopyList(Transform t)
    {
        if (!_copyList.Contains(t))
        {
            _copyList.Add(t);
        }

        var col = t.GetComponent<Collider2D>();

        if(_playerTransform != null)
        {
            var playerCol = _playerTransform.GetComponent<Collider2D>();

            Physics2D.IgnoreCollision(col, playerCol);
        }

        foreach (var playerCopy in _playerInfo.GetCopyList())
        {
            var playerCopyCol = playerCopy.GetComponent<Collider2D>();

            Physics2D.IgnoreCollision(col, playerCopyCol);
        }
    }

    public void RemoveCopyList(Transform t)
    {
        if (_copyList.Contains(t))
        {
            _copyList.Remove(t);
        }
    }

    public void ClearCopyList()
    {
        _copyList.Clear();
    }

    public bool ContainsCopyList(Transform t)
    {
        return _copyList.Contains(t) || t == _transform;
    }

    public void SetMovable(bool movable)
    {
        _movable = movable;
    }
}

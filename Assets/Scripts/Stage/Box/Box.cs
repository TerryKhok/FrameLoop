using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Box
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :���̋������Ǘ�����N���X
 *               �j��\�ȏ��̔j��A�v���C���[�ɒ͂܂�Ă��鎞�̈ړ�
 *               
 *  Created     :2024/04/27
 *  
 *  2024/10/12�@������󂷃M�~�b�N���X�N���v�g���Q�Ƃ���悤�ɕύX
 */
public class Box : MonoBehaviour,IBox
{
    private float _height = 0f;
    private float _lastGroundHeight = 0f;
    private Transform _transform, _playerTransform;
    [SerializeField,Tooltip("���̉���")]
    private float _width = 1f;
    [SerializeField,Tooltip("�j�󂷂�̂ɕK�v�ȍ���")]
    private float _breakHeight = 5f;
    [SerializeField, Tooltip("�j�󎞂Ɏ~�܂�t���[����(50fps)")]
    private int _hitStop = 3;
    [SerializeField,Tag,Tooltip("�j��\��Tag")]
    private List<string> _tagList = new List<string>() { "Breakable"};
    [SerializeField]
    private float _afterimageLifetime = 0.1f;

    private Vector2 _prevVelocity = Vector2.zero;
    private int _stopCount = 0;
    private bool _isWaiting = false;

    private Rigidbody2D _rb = null;
    private BoxCollider2D _collider2D = null;
    private SpriteRenderer _spriteRenderer = null;

    private GameObject _afterimageOrigin = null;

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

        _spriteRenderer = GetComponent<SpriteRenderer>();

        _afterimageOrigin = new GameObject();
        _afterimageOrigin.SetActive(false);
        var renderer = _afterimageOrigin.AddComponent(_spriteRenderer);
        Color color = renderer.color;
        color.a = 0.1f;
        renderer.color = color;


        _playerInfo = PlayerInfo.Instance;

        //y���ȊO�̓����𐧌�
        //_rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    private void Update()
    {
        _rb.WakeUp();

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

    //FixedUpdate�̌��LateFixedUpdate���Ăяo��
    private IEnumerator UpdateLateFixedUpdate()
    {
        var waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            yield return waitForFixedUpdate;
            LateFixedUpdate();
        }
    }

    //FixedUpdate�̌�ɌĂяo����郁�\�b�h
    private void LateFixedUpdate()
    {
        //Debug.Log(_rb.velocity);
        platformBreak();
        MakeAfterimage();
        isHold();
    }

    private void MakeAfterimage()
    {
        if (_height - _lastGroundHeight >= _breakHeight)
        {
            var instance = Instantiate(_afterimageOrigin, _transform.position, Quaternion.identity);
            instance.SetActive(true);
            instance.GetComponent<SpriteRenderer>().maskInteraction = _spriteRenderer.maskInteraction;
            Destroy(instance, _afterimageLifetime);
        }
    }


    //�j��\�ȏ�����
    private void platformBreak()
    {
        if (_isWaiting)
        {
            _stopCount++;
            // �q�b�g�X�g�b�v�̎��ԓ��Ȃ�return����
            if (_stopCount < _hitStop)
            {
                _rb.velocity = Vector3.zero;
                return;
            }
            // �q�b�g�X�g�b�v�̎��Ԃ��z������ҋ@����������
            else
            {
                _rb.velocity = _prevVelocity;
                _rb.gravityScale = 1;
                _isWaiting = false;
            }
        }

        //�ō��_�̍��W���X�V
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
                //Ray�����������̂����g�Ȃ�continue����
                if(hit.transform.CompareTag("Box"))
                {
                    continue; 
                }

                //�󒆂ɉ����o���ꂽ��݂͂��L�����Z������
                if (hit.distance > 0.3f && _rb.velocity.y <= -0.1f)
                {
                    holdCancel();
                    return;
                }

                if (_tagList.Contains(hit.transform.tag))
                {
                    //�ō��_�Ƃ̍������ȏ�Ȃ�j�󂷂�
                    //�ō��_�̃��Z�b�g�͍s��Ȃ�
                    if (_height - _lastGroundHeight >= _breakHeight)
                    {
                        //var breakablePlatform = hit.transform.GetComponent<BreakablePlatform>();
                        //breakablePlatform.Break();
                        Destroy(hit.transform.gameObject);

                        _prevVelocity = _rb.velocity;
                        _rb.gravityScale = 0;
                        _stopCount = 0;
                        _isWaiting = true;
                    }
                    else
                    {
                        _lastGroundHeight = _transform.position.y;
                        _height = _transform.position.y;
                    }

                    return;
                }
                else
                {
                    //�n�ʂɐG�ꂽ��ō��_�����Z�b�g
                    _lastGroundHeight = _transform.position.y;
                    _height = _transform.position.y;
                    return;
                }
            }

            //���g�ȊO��Ray���������ĂȂ�������݂͂��L�����Z��
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

        //�������������~�߂�
        AudioManager.instance.Stop("Box Pull");
        soundFlag = false;

        //--------------------------------------------------------------------------------
        //�v���C���[�Ɣ��̓����蔻��𕜊�
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

    //�v���C���[�����������Ă���Ƃ��̏���
    private void isHold()
    {
        if(_playerTransform == null) { return; }

        if (_playerInfo.g_walkCancel) 
        {
            Debug.Log("stop");
            return;
        }

        //�v���C���[���ړ������ĂȂ����͉����~�߂�
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

        //���͂��Ȃ��Ƃ��͓������Ȃ�
        if (direction.x == 0 && !_movable)
        {
            return;
        }

        Ray ray = new Ray(pos, direction);
        RaycastHit2D[] hits;
        Vector2 size = new Vector2((_width+0.1f) / 2, 0.5f);


        //�����Ɠ������C���[��Box���i�s�����ɂ��邩�`�F�b�N
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
                    //���������O���āA���W���ړ�������
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

        //���ƃv���C���[�̍��W�̃Y�������킹��
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

        //�����v���C���[�̂ǂ��炩�����[�v���Ă���ꍇ�͍��W�����炷
        pos += _offset;

        //x���W���v���C���[�̍��W�����苗�����炵���ʒu�ɂ���
        _rb.position = pos;

    }

    //�����ړ���������transform���󂯎��
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
        //�v���C���[�Ɣ��̓����蔻��𖳂���
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

        //�������������Đ�
        //if (!soundFlag)
        //{
        //    AudioManager.instance.Play("Box Pull");
        //    soundFlag = true;
        //}
    }

    public void AdjustPosition()
    {
        //���W�����r���[�Ȏ��ɏC�����鏈��---------------------------------------------------
        Vector2 pos = _transform.position;
        var gap = new Vector2(pos.x % 0.5f, pos.y % 0.5f);

        //���̃O���b�h�̂ق����߂��Ƃ��͎��̃O���b�h����̍��ɕϊ�����
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

        //��ԋ߂��̃O���b�h����̋����̐�Βl�Ŕ�r����(�l�͈̔͂�0.0~0.25)
        //�Y�����傫���Ȃ���W�̕␳�����Ȃ�
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

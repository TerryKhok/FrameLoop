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
 */
public class Box : MonoBehaviour,IBox
{
    private float _height = 0f;
    private Transform _transform, _playerTransform;
    [SerializeField,Tooltip("���̉���")]
    private float _width = 1f;
    [SerializeField,Tooltip("�j�󂷂�̂ɕK�v�ȍ���")]
    private float _breakHeight = 5f;
    [SerializeField,Tag,Tooltip("�j��\��Tag")]
    private List<string> _tagList = new List<string>() { "Breakable"};

    private Rigidbody2D _rb = null;
    private BoxCollider2D _collider2D = null;

    private PlayerInfo _playerInfo = null;
    private PlayerMove _playerMove = null;

    private Vector2 _offset = Vector2.zero;

    private List<Transform> _copyList = new List<Transform>();

    private void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<BoxCollider2D>();

        _playerInfo = PlayerInfo.Instance;

        //y���ȊO�̓����𐧌�
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
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
        platformBreak();
        isHold();
    }

    //�j��\�ȏ�����
    private void platformBreak()
    {
        //�ō��_�̍��W���X�V
        if (_height < _transform.position.y)
        {
            _height = _transform.position.y;
        }

        Ray ray = new Ray(_transform.position, Vector3.down);
        RaycastHit2D[] hits;
        Vector2 size = new Vector2(_width / 2, 0.5f);

        /*���̂܂܂��ƃt���[���̒��ɂ��鎞Breakable�̏����󂹂Ȃ�
         * 
         * �v�C��
         */
        LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform") | 1 << LayerMask.NameToLayer("OBox");
        if(gameObject.layer == LayerMask.NameToLayer("IBox"))
        {
            mask = 1 << LayerMask.NameToLayer("IPlatform") | 1 << LayerMask.NameToLayer("IBox");
        }

        hits = Physics2D.BoxCastAll(ray.origin, size, 0, ray.direction, 1, mask);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                //Ray�����������̂����g�Ȃ�continue����
                if(hit.transform == _transform) { continue; }

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
                    if (_height - _transform.position.y >= _breakHeight)
                    {
                        Destroy(hit.transform.gameObject);
                    }

                    return;
                }
                else
                {
                    //�n�ʂɐG�ꂽ��ō��_�����Z�b�g
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

        Vector2 pos = _transform.position;
        var gap = new Vector2(pos.x % 0.5f, pos.y % 0.5f);

        if(gap.x > 0.25f)
        {
            gap.x = gap.x - 0.5f;
        }
        else if(gap.x < -0.25f)
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

        if (0.1f < absGap.x && absGap.x < 0.4f)
        {
            gap.x = 0;
        }

        if (0.1f < absGap.y && absGap.y < 0.4f)
        {
            gap.y = 0;
        }

        pos -= gap;
        _transform.position = pos;
    }

    //�v���C���[�����������Ă���Ƃ��̏���
    private void isHold()
    {
        if(_playerTransform == null) { return; }

        //�v���C���[���ړ������ĂȂ����͉����~�߂�
        if (_playerMove._isMoving == false)
        {
            AudioManager.instance.Stop("Box Pull");
        }
        else
        {
            AudioManager.instance.Play("Box Pull");
        }

        var pos = _rb.position;

        var direction = new Vector2(_playerInfo.g_currentInputX,0);
        pos.x = _playerTransform.position.x;

        float offset = 0.95f;
        int relativeDirection = _playerInfo.g_currentInputX * _playerInfo.g_boxDirection;

        if (relativeDirection < 0)
        {
            offset = 0.8f;
        }

        pos += (Vector2)_playerTransform.right * offset;

        //�����v���C���[�̂ǂ��炩�����[�v���Ă���ꍇ�͍��W�����炷
        pos += _offset;

        //x���W���v���C���[�̍��W�����苗�����炵���ʒu�ɂ���
        _rb.position = pos;

        Ray ray = new Ray(pos, direction);
        RaycastHit2D[] hits;
        Vector2 size = new Vector2(_width / 2, 0.5f);


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

        hits = Physics2D.BoxCastAll(ray.origin, size, 0, ray.direction, 0.25f, mask);

        bool hitWall = false;
        if(hits.Length > 0)
        {
            foreach(var hit in hits)
            {
                //���������O���āA���W���ړ�������
                if(hit.transform == _transform) { continue; }

                if (hit.transform.CompareTag("Box"))
                {
                    var rb = hit.transform.GetComponent<Rigidbody2D>();
                    pos.x += _width * ray.direction.normalized.x;
                    rb.position = pos;

                    continue;
                }

                hitWall = true;
                _playerInfo.g_wall = ray.direction.normalized.x;
            }
        }

        if (!hitWall)
        {
            _playerInfo.g_wall = 0;
        }
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
        AudioManager.instance.Play("Box Pull");
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
}
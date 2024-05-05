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
    private PlayerInfo _playerInfo = null;

    private void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();

        _playerInfo = PlayerInfo.Instance;

        //y���ȊO�̓����𐧌�
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    private void Update()
    {
        if (!_playerInfo.g_takeUpFg)
        {
            _playerTransform = null;
        }
        platformBreak();
    }

    private void FixedUpdate()
    {
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
                if (hit.distance > 0.3f)
                {
                    _playerTransform = null;
                    _playerInfo.g_takeUpFg = false;
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
                }
                else
                {
                    //�n�ʂɐG�ꂽ��ō��_�����Z�b�g
                    _height = _transform.position.y;
                }
            }
        }
        else
        {
            _playerTransform = null;
            _playerInfo.g_takeUpFg = false;
        }
    }

    //�v���C���[�����������Ă���Ƃ��̏���
    private void isHold()
    {
        if(_playerTransform == null) { return; }

        var pos = _rb.position;

        //Debug.Log(pos);
        var direction = ((Vector2)_playerTransform.position - pos).normalized;
        pos.x = _playerTransform.position.x;
        pos += (Vector2)_playerTransform.right * 1;

        //x���W���v���C���[�̍��W�����苗�����炵���ʒu�ɂ���
        _rb.position = pos;

        Ray ray = new Ray(pos, direction);
        RaycastHit2D[] hits;
        Vector2 size = new Vector2(_width / 2, 0.5f);


        //�����Ɠ������C���[��Box���i�s�����ɂ��邩�`�F�b�N
        LayerMask mask = 1 << gameObject.layer;
        hits = Physics2D.BoxCastAll(ray.origin, size, 0, ray.direction, 0.2f, mask);

        if(hits.Length > 0)
        {
            foreach(var hit in hits)
            {
                //���������O���āA���W���ړ�������
                if(hit.transform == _transform) { continue; }

                var rb = hit.transform.GetComponent<Rigidbody2D>();
                pos.x += _width;
                rb.position = pos;
            }
        }
    }

    //�����ړ���������transform���󂯎��
    public void Hold(Transform t)
    {
        _playerTransform = t;
    }
}

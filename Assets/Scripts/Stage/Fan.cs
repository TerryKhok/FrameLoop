using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*  ProjectName :FrameLoop
 *  ClassName   :Fan
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :���̃^�C���𐶐�����
 *               
 *  Created     :2024/04/27
 */
public class Fan : MonoBehaviour,IParentOnTrigger
{
    private enum Direction
    {
        UP,DOWN,LEFT,RIGHT
    }

    [SerializeField,Tooltip("���̕���")]
    private Direction _direction = Direction.RIGHT;
    [SerializeField,Tooltip("����ɐݒu����Tile")]
    private Tile _tile = null;
    [SerializeField,Tooltip("���̎˒�")]
    private int _range = 1;
    [SerializeField,Tooltip("�^���鑬�x(m/s)")]
    private float _force = 1f;
    [SerializeField,Tooltip("�e���͈͂��\���ɂ���")]
    private bool _invisible = false;
    [SerializeField, Tag,Tooltip("�e����^����Tag")]
    private List<string> _tagList = new List<string>() { "Player"};
    [SerializeField, Tag,Tooltip("�Ղ���Tag")]
    private List<string> _blockTagList = new List<string>() { "Platform"}; 
    [SerializeField,Tooltip("���߂���L����")]
    private bool _enableOnAwake = true;
    [SerializeField,Tooltip("�������̃A�e���A��")]
    private Material _leftMaterial = null;
    [SerializeField,Tooltip("������̃A�e���A��")]
    private Material _upMaterial = null;
    [SerializeField,Tooltip("�������̃A�e���A��")]
    private Material _downMaterial = null;
    private bool _isEnable = false;

    private Transform _transform = null;
    private Tilemap _tilemapOutside = null, _tilemapInside;
    private TilemapRenderer _tilemapRenderer = null, _tilemapRenderer_out;

    //���ɐG��Ă�Rigidbody��dictionary
    private Dictionary<Collider2D, Rigidbody2D> _rbDic = new Dictionary<Collider2D, Rigidbody2D>();

    private Vector3Int _frameSize = Vector3Int.zero;
    private Transform _outsideT = null;
    private Vector3Int _actualDirection = Vector3Int.right;

    private Animator _animator;

    private PlayerInfo _playerInfo;
    private Camera _camera = null;

    private bool _playerBoxFlag = false;
    private bool windSoundFlag = false;


    //Tilemap�̍��W�𒲐�
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

        //�e��Component���擾--------------------------------------------------
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

        //_direction�ŕ��̔��˕��������߂�
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

        //�t���[���̃T�C�Y���擾
        var frameObj = GameObject.FindGameObjectWithTag("Frame");
        _frameSize = (Vector3Int)frameObj.GetComponent<FrameLoop>().GetSize();

        //���𔭎�
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
        //�L���Ō������ԂȂ�renderer��L���ɂ���
        _tilemapRenderer.enabled = _isEnable && !_invisible;

        if (!_isEnable || Goal.Instance.g_clear) 
        {
            return; 
        }

        _playerBoxFlag = false;

        //���˕�����Vector2�ɕϊ�
        Vector2 forceDirection = new Vector2(_actualDirection.x, _actualDirection.y);
        List<Collider2D> temp = new List<Collider2D>();

        //���ɐG��Ă���rigidbody��S�Ċm�F
        foreach (var item in _rbDic)
        {
            if(item.Value == null)
            {
                temp.Add(item.Key);
                continue;
            }

            //Player���A�����Ă锠�Ȃ�t���O�𗧂Ă�
            if(item.Value.transform == _playerInfo.g_transform || 
               item.Value.transform == _playerInfo.g_box)
            {
                //���Ƀt���O�������Ă�����return
                if(_playerBoxFlag)
                {
                    return;
                }

                //Player�������Ɋւ�炸Player��rigidbody���w��
                var playerRb = _playerInfo.g_rb;

                //���˕����Ɉ�葬�x�ňړ�������
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

            //���˕����Ɉ�葬�x�ňړ�������
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
                    // �����ȊO�̃I�u�W�F�N�g���O�ɂ�������ړ����Ȃ�
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

    //Tilemap�ɐG��Ă���collider���󂯎��
    public void OnStay(Collider2D other, Transform transform)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        // ���̃R�s�[�Ȃ�e�Ŕ��f����
        if(other.GetComponent<BoxChild>() != null)
        {
            other = other.GetComponentInParent<Collider2D>();
        }

        if(!_rbDic.ContainsKey(other))
        {
            //rigidbody���擾����dictionary�֒ǉ�
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

    //Tilemap���痣�ꂽcollider���󂯎��
    public void OnExit(Collider2D other, Transform transform)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        // ���̃R�s�[�Ȃ�e�Ŕ��f����
        if (other.GetComponent<BoxChild>() != null)
        {
            other = other.GetComponentInParent<Collider2D>();
        }

        //dictionary�ɑ��݂��Ă�����폜����
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
        //IParentOnTrigger�̕K�v���\�b�h
    }

    //�t���[�����L���ɂȂ����Ƃ��Ɉ�x�Ă΂��
    public void FanLoopStarted()
    {
        StartCoroutine("windLoop");
    }

    //�t���[���������ɂȂ����Ƃ��Ɉ�x�Ă΂��
    public void FanLoopCanceled()
    {
        StartCoroutine("asyncSetTiles");
    }

    //�t���[��������Ƃ��̕��̐���
    private IEnumerator windLoop()
    {
        if (!_isEnable) 
        {
            yield break; 
        }

        //�t���[���̏I����҂�
        //�҂��Ȃ��ƃt���[���Ő�������Collider��Ray��������Ȃ�
        yield return new WaitForEndOfFrame();

        //���ɐG��Ă�I�u�W�F�N�g�̃��X�g���N���A
        _rbDic.Clear();

        //Tilemap��S�ăN���A
        _tilemapOutside.ClearAllTiles();
        _tilemapInside.ClearAllTiles();

        //Fan���t���[���̓����ɂ��邩
        bool inside = false, enterFlag = false;

        var pos = _transform.position;
        Vector3Int intPos = Vector3Int.zero;

        //�t���[���ɂ���������LayerMask���쐬
        LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

        //���̍ő勗��+1�񕪂̃��[�v
        for (int i = 0; i <= _range; i++, pos += (Vector3)_actualDirection, intPos += _actualDirection)
        {
            //Fan�̈ʒu����1�}�X�����炵��Ray���΂�
            Ray ray = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos));

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10, mask);

            if (i == 0)
            {
                if(hits.Length > 0)
                {
                    //�t���[���̓����Ȃ�Ray������̑���ɓ�����悤�ɂ���
                    mask |= 1 << LayerMask.NameToLayer("IPlatform");
                    inside = true;
                }
                else
                {
                    //�t���[���̊O���Ȃ�Ray���O���̑���ɓ�����悤�ɂ���
                    mask |= 1 << LayerMask.NameToLayer("OPlatform");
                }

            }
            else if (inside)//�t�@�����t���[���̒���
            {
                //�ݒu�������ʒu�ɃI�u�W�F�N�g������ꍇ
                if (hits.Length > 0)
                {
                    bool instance_here = false, blocking = false;
                    foreach (var hit in hits)
                    {
                        //��Q������������blocking��true��
                        if (_blockTagList.Contains(hit.transform.tag))
                        {
                            blocking = true;
                        }
                        //�t���[���̒��Ȃ�instance_here��true��
                        if (hit.transform.CompareTag("Frame"))
                        {
                            instance_here = true;
                        }
                    }
                    //��Q���̂�����W�ɐ������悤�Ƃ�����I��
                    if (blocking && instance_here)
                    {
                        yield break;
                    }
                    //�t���[���̒��Ȃ���W�����炳���������Ď��̍��W���m�F
                    if (instance_here)
                    {
                        SetTile(intPos, _actualDirection, _tilemapInside, _tile);
                        continue;
                    }
                }

                var pos_sub = pos;
                pos_sub -= _actualDirection * _frameSize;
                Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));

                //�������Raycast
                RaycastHit2D[] hits_sub = Physics2D.RaycastAll(ray_sub.origin, ray_sub.direction, 10, 1 << LayerMask.NameToLayer("IPlatform") | 1 << LayerMask.NameToLayer("Frame"));

                bool frameOutside = true;
                foreach (var hit in hits_sub)
                {
                    //��Q������������I��
                    if (_blockTagList.Contains(hit.transform.tag))
                    {
                        yield break;
                    }

                    if (hit.transform.CompareTag("Frame"))
                    {
                        frameOutside = false;
                    }
                }

                //�t���[���̊O���Ȃ�I��
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
                //�ݒu�������ʒu�ɃI�u�W�F�N�g������ꍇ
                if (hits.Length > 0)
                {
                    bool blocking = false;
                    foreach (var hit in hits)
                    {
                        //��Q������������blocking��true��
                        if (_blockTagList.Contains(hit.transform.tag))
                        {
                            blocking = true;
                        }
                        //�t���[���̒��Ȃ�enterFlag��true��
                        if (hit.transform.CompareTag("Frame"))
                        {
                            enterFlag = true;
                        }
                    }


                    //��Q���̂�����W�ɐ������悤�Ƃ�����I��
                    if (blocking && !enterFlag)
                    {
                        yield break;
                    }
                }

                //�t���[���̒��Ȃ���W�����炵�Đ���
                if (enterFlag)
                {
                    var pos_sub = pos;
                    pos_sub += _actualDirection * _frameSize;
                    Ray ray_sub = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos_sub));

                    //�������Raycast
                    RaycastHit2D hit_sub = Physics2D.Raycast(ray_sub.origin, ray_sub.direction, 10, 1 << LayerMask.NameToLayer("OPlatform"));

                    //��Q������������I��
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

    //�t���[�����Ȃ����̕��̐���
    private void SetTiles()
    {
        if (!_isEnable) { return; }

        //���ɐG��Ă�I�u�W�F�N�g�̃��X�g���N���A
        _rbDic.Clear();

        //Tilemap��S�ăN���A
        _tilemapOutside.ClearAllTiles();
        _tilemapInside.ClearAllTiles();

        var pos = _transform.position;
        Vector3Int intPos = Vector3Int.zero;

        //���̍ő勗�������[�v
        for (int i = 0; i < _range; i++)
        {
            pos += _actualDirection;
            intPos += _actualDirection;
            Ray ray = _camera.ScreenPointToRay(_camera.WorldToScreenPoint(pos));
            //Debug.DrawRay(ray.origin, ray.direction*10,Color.red,0.1f);

            //�t���[���̊O���̑���ɓ�����Layermask���쐬
            LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform");

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10, mask);

            //��Q���ɓ���������I��
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

    //�t���[���I����҂��Ă��畗�𐶐�����
    private IEnumerator asyncSetTiles()
    {
        yield return new WaitForEndOfFrame();

        SetTiles();
    }

    //�L�����ǂ����������ŕύX����
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

    //�L�����������𔽓]������
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

    //�^�C���̌������w�肵�ăZ�b�g����
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

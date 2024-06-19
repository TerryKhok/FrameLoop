using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static PlayerTitleMotion;

/*  ProjectName :FrameLoop
 *  ClassName   :FrameLoop
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�t���[���̋����𐧌䂷��
 *               
 *  Created     :2024/05/30
 */
public class FrameLoop : SingletonMonoBehaviour<FrameLoop>,IParentOnTrigger
{
    [SerializeField,Tooltip("�����蔻��𐶐����邽�߂ɐݒu����Tile")]
    private Tile _tile = null;
    [SerializeField,Tooltip("�����̓����蔻��p��Tilemap")]
    private Tilemap _insideTile = null;
    [SerializeField, Tooltip("�O���̓����蔻��p��Tilemap")]
    private Tilemap _outsideTile = null;
    [SerializeField,Tooltip("Frame�ɓK�p����Material(Script����F���ύX�����̂Ő�p�̂��̂ɂ���)")]
    private Material _material = null;
    [SerializeField,Tooltip("Frame��Size")]
    private Vector2Int _size = Vector2Int.one;
    [SerializeField,Tooltip("�v���C���[�̍��W����Y�����ɂǂꂾ�����炷��")]
    private float _yOffset = 1f;
    [SerializeField, Tooltip("���Ⴊ�ݒ��Ƀv���C���[�̍��W����Y�����ɂǂꂾ�����炷��")]
    private float _yOffset_Crouching = -2f;
    [SerializeField,Tooltip("�؂�ւ�")]
    private bool _toggle = false;

    //[SerializeField]    //SE
    //private AudioManager _audioManager = null;

    private List<Collider2D>
        _insiders = new List<Collider2D>(),                         //�t���[���̓����̃I�u�W�F�N�g�̃��X�g
        _enterOutsiders = new List<Collider2D>();                   //�t���[���̊O�ɏo�悤�Ƃ��Ă���I�u�W�F�N�g�̃��X�g

    private Dictionary<Collider2D, Vector2>
        _outsiders = new Dictionary<Collider2D, Vector2>(),         //�t���[���̊O���̃I�u�W�F�N�g�Ɠ����Ă�������̃��X�g
        _exitInsiders = new Dictionary<Collider2D, Vector2>(),      //�t���[���̒��ɓ��낤�Ƃ��Ă���I�u�W�F�N�g�Ɠ����Ă�������̃��X�g
        _prevExitInsiders = new Dictionary<Collider2D, Vector2>();  //�O�t���[���̏�̃��X�g

    private List<Collider2D> 
        _insideColliderList = new List<Collider2D>(),               //�����ɐ��������R���C�_�[�̃��X�g
        _outsideColliderList = new List<Collider2D> ();             //�O���ɐ��������R���C�_�[�̃��X�g

    private Dictionary<Collider2D, Transform>
        _outsideCopyDic = new Dictionary<Collider2D, Transform>();  //�t���[���̊O���̃I�u�W�F�N�g�̃R�s�[�̃��X�g
    private Dictionary<Collider2D, List<Transform>>
    _insideCopyDic = new Dictionary<Collider2D, List<Transform>>(); //�t���[���̓����̃I�u�W�F�N�g�̃R�s�[�̃��X�g

    private bool[] _ableToLoop = { true, true, true, true };    //�p��ʂ�邩�ǂ����i���A����A���ӁA�E�ӂ̏��j

    private List<Fan> _fanList = new List<Fan>();                   //Fan�N���X���擾�������X�g
    private List<Button> _buttonList = new List<Button>();          //Button�N���X���擾�������X�g
    private List<TileReplace> _replaceTileList = new List<TileReplace>();// �^�C���̒u�������p�R���|�[�l���g�̃��X�g

    private (float min, float max) 
        _loopRangeX = (0, 0), _loopRangeY = (0, 0);                 //�t���[���̒[�̍��W

    private BoxCollider2D _boxCollider = null;
    private Transform _playerTrans = null, _transform = null;
    private SpriteMask _spriteMask = null;
    private CompositeCollider2D
        _insideTileCol = null, _outsideTileCol = null;              //�����蔻�萶���p�̃^�C���}�b�v�̃R���C�_�[
    private bool _isCrouching = false;
    private InputManager _inputManager = null;
    private PlayerInfo _playerInfo = null;
    private Goal _goal = null;
    private GameObject _colliderPrefab = null;
    private Transform _topT = null, _bottomT = null, _rightT = null, _leftT = null;

    private bool[] _topHitArray = new bool[8],
                   _bottomHitArray = new bool[8],
                   _rightHitArray = new bool[6],
                   _leftHitArray = new bool[6];

    private Dictionary<TileReplace, List<(int switchNum, int num)>> _breakableDic = new Dictionary<TileReplace, List<(int switchNum, int num)>>();

    [System.NonSerialized]
    public bool g_isActive = false, g_usable = true, g_activeTrigger;
    private bool _prevActive = false;

    private void Start()
    {
        _transform = transform;

        //SpriteMask���擾���Ė����ɂ��Ă���
        _spriteMask = GetComponent<SpriteMask>();
        _spriteMask.enabled = false;

        //BoxCollider���擾���ăT�C�Y���m�肳����
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.size = new Vector3(_size.x - 0.2f, _size.y - 0.2f, 1);

        //Singleton�̃X�N���v�g���擾���Ă���
        _playerInfo = PlayerInfo.Instance;
        _goal = Goal.Instance;

        //Player��Transform���擾
        _playerTrans = _playerInfo.g_transform;

        //Tilemap�̃R���C�_�[���擾����
        _insideTileCol = _insideTile.GetComponent<CompositeCollider2D>();
        _outsideTileCol = _outsideTile.GetComponent<CompositeCollider2D>();

        //TilemapRenderer�𖳌��ɂ���
        TilemapRenderer insideRenderer = _insideTile.GetComponent<TilemapRenderer>();
        insideRenderer.enabled = false;
        TilemapRenderer outsideRenderer = _outsideTile.GetComponent<TilemapRenderer>();
        outsideRenderer.enabled = false;

        _material.color = new Color32(255, 255, 0, 100);

        //��������v���n�u�����[�h���Ă���
        _colliderPrefab = (GameObject)Resources.Load("Collider");

        //InputManager�N���X���擾
        var managerObj = GameObject.FindGameObjectWithTag("GameManager");
        _inputManager = managerObj.GetComponent<InputManager>();

        //Fan�X�N���v�g�����ׂĎ擾
        var fanObjs = GameObject.FindGameObjectsWithTag("Fan");
        foreach (var fanObj in fanObjs)
        {
            _fanList.Add(fanObj.GetComponent<Fan>());
        }

        //Button�X�N���v�g���擾
        var buttonObjs = GameObject.FindGameObjectsWithTag("Button");
        foreach (var buttonObj in buttonObjs)
        {
            _buttonList.Add(buttonObj.GetComponent<Button>());
        }

        //Scnen��TileReplace�X�N���v�g�����ׂĎ擾
        var tileReplaceObjs = GameObject.FindGameObjectsWithTag("BreakableParent");
        foreach(var tileReplaceObj in tileReplaceObjs)
        {
            _replaceTileList.Add(tileReplaceObj.GetComponent<TileReplace>());
        }

        //�q�I�u�W�F�N�g���擾���ď㉺���E�����蓖�Ă�
        var children = transform.GetComponentsInChildren<Transform>().ToList();
        children.Remove(transform);
        _topT = children[0];
        _bottomT = children[1];
        _rightT = children[2];
        _leftT = children[3];

        //�q�I�u�W�F�N�g�̍��W�𒲐�����
        _topT.localPosition = new Vector3(0, _size.y / 2);
        _bottomT.localPosition = new Vector3(0, -_size.y / 2);
        _rightT.localPosition = new Vector3(_size.x / 2, 0);
        _leftT.localPosition = new Vector3(-_size.x / 2, 0);

        //�q�I�u�W�F�N�g�̃X�P�[���𒲐�����
        _topT.localScale = new Vector3(_size.x + 0.2f, 0.2f, 1);
        _bottomT.localScale = new Vector3(_size.x + 0.2f, 0.2f, 1);
        _rightT.localScale = new Vector3(0.2f, _size.y, 1);
        _leftT.localScale = new Vector3(0.2f, _size.y, 1);

        //�q�I�u�W�F�N�g�̓����蔻��𒲐�
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

        _insiders.Add(_playerInfo.g_collider);
    }

    private void Update()
    {
        if(_playerTrans == null)
        {
            return;
        }

        //�g�p�\���ƍ��킹�ď�Ԃ����肷��
        g_isActive &= g_usable;

        //null�`�F�b�N������null�Ȃ烊�X�g����폜
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

            //���[�v����I�u�W�F�N�g�̗������x�𐧌�
            var rb = col.GetComponent<Rigidbody2D>();
            var velocity = rb.velocity;
            if(velocity.y < -15f)
            {
                velocity.y = -15f;
                rb.velocity = velocity;
            }
        }

        //null�`�F�b�N������null�Ȃ烊�X�g����폜
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

        if (g_isActive)
        {
            //�R�s�[�̃X�v���C�g���R�s�[���ƃ����N������
            insiderSpriteLink();

            //�I�u�W�F�N�g�̃R�s�[�𐶐�
            instantiateCopy();
        }

        //���n������g�p�\�ɖ߂�
        g_usable |= PlayerInfo.instance.g_isGround;

        //�t���[���̍��W�𒲐�
        adjustPos();

        g_activeTrigger = !_prevActive && g_isActive;

        //�t���[�����L���ɂȂ����Ƃ��s������
        if (g_activeTrigger)
        {
            onActive();
        }

        //�t���[���������ɂȂ����Ƃ��s������
        if(_prevActive && !g_isActive)
        {
            onInactive();
        }
    }

    private void LateUpdate()
    {   
        //�O�t���[���̏�Ԃ�ۑ�
        _prevActive = g_isActive;
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
        _prevExitInsiders = new Dictionary<Collider2D, Vector2>(_exitInsiders);
        foreach(var col in _prevExitInsiders.Keys)
        {
            _exitInsiders[col] = Vector2.zero;
        }
    }

    //�t���[�����L���ɂȂ������Ɉ�x���s���郁�\�b�h
    private void onActive()
    {
        //�p�Ń��[�v�ł��邩�����Z�b�g
        for(int i=0; i < _ableToLoop.Length;i++)
        {
            _ableToLoop[i] = true;
        }

        //�t���[���̒[�̍��W���X�V���Ċm��
        _loopRangeX.min = _transform.position.x - (_size.x / 2);
        _loopRangeX.max = _transform.position.x + (_size.x / 2);
        _loopRangeY.min = _transform.position.y - (_size.y / 2);
        _loopRangeY.max = _transform.position.y + (_size.y / 2);

        //SpriteMask��L���ɂ���
        _spriteMask.enabled = true;
        _material.color = new Color32(0, 255, 0, 150);

        List<Collider2D> removeList = new List<Collider2D>();
        _breakableDic.Clear();

        foreach (var col in _insiders)
        {
            //�O���̃I�u�W�F�N�g�̃��X�g�ɂ����݂���ꍇ�A���W�łǂ��炩�ɕ��ނ���
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

            //�����̃I�u�W�F�N�g��SpriteMask�̒��ł̂ݕ\�������悤�ύX
            SpriteRenderer renderer = col.GetComponentInChildren<SpriteRenderer>();
            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            //Player�ȊO�̓����̃I�u�W�F�N�g�̓��C���[��I~~~���C���[�ɕύX
            if (col.CompareTag("Player"))
            {
                continue;
            }
            col.gameObject.layer++;
        }

        //�O���̃��X�g�ɕ��ނ��ꂽ�I�u�W�F�N�g������̃��X�g����폜
        foreach(var col in removeList)
        {
            _insiders.Remove(col);
            if (_exitInsiders.ContainsKey(col))
            {
                _exitInsiders.Remove(col);
            }
        }

        //--------------------------------------------------------
        //�����蔻��̔z���S�ď�����
        //--------------------------------------------------------

        for(int i=0; i < _topHitArray.Length; i++)
        {
            _topHitArray[i] = false;
        }
        for (int i = 0; i < _bottomHitArray.Length; i++)
        {
            _bottomHitArray[i] = false;
        }
        for (int i = 0; i < _leftHitArray.Length; i++)
        {
            _leftHitArray[i] = false;
        }
        for (int i = 0; i < _rightHitArray.Length; i++)
        {
            _rightHitArray[i] = false;
        }

        //---------------------------------------------------------

        //�t���[���͈̔�+-1�}�X���͈̔͂����[�v
        for (int i=0; i <= _size.x+1; i++)
        {
            for(int j=0; j <= _size.y+1; j++)
            {

                //���W���p�Ȃ玟�̃��[�v��
                if (i == 0 || i == _size.x + 1)
                {
                    if (j == 0 || j == _size.y + 1)
                    {
                        continue;
                    }
                }

                //Ray�̌��_���t���[���̍�������Ɍ��肷��
                var origin = new Vector2(_loopRangeX.min, _loopRangeY.min);
                origin.x += -0.5f + i;
                origin.y += -0.5f + j;

                RaycastHit2D[] hit;

                //Ray���쐬
                var screenPos = Camera.main.WorldToScreenPoint(origin);
                Ray ray = Camera.main.ScreenPointToRay(screenPos);

                //LayerMask���O���̓����蔻��ɂ̂ݓ�����悤�ɂ���
                LayerMask layerMask = 0;
                layerMask |= 1 << LayerMask.NameToLayer("OPlatform");
                layerMask |= 1 << LayerMask.NameToLayer("OBox");

                hit = Physics2D.RaycastAll(ray.origin, ray.direction, 15, layerMask);

                bool breakable = false;
                TileReplace tileReplace = null;

                //Ray��������Ȃ������玟�̃��[�v��
                if (hit.Length == 0)
                {
                    continue;
                }
                else
                {
                    bool instantFg = false;
                    foreach(var item in hit)
                    {
                        if (!_insideColliderList.Contains(item.collider))
                        {
                            if (item.transform.CompareTag("Box"))
                            {

                                //�t���[���̒[�ɔ����������甠�𕡐�����
                                if (i == 0 || i < _size.x + 1 || j == 0 || j < _size.y + 1)
                                {
                                    ColliderInstantiate(item.transform.position, i, j, item.transform);
                                }
                                continue;
                            }
                            else if(item.transform.CompareTag("Breakable"))
                            {
                                breakable = true;
                                tileReplace = item.transform.GetComponentInParent<TileReplace>();
                            }

                            //���ȊO�ɓ���������instantFg��True��
                            instantFg = true;
                        }
                    }

                    //���ȊO�ɓ������Ă�����Tile���Z�b�g����
                    if (instantFg)
                    {
                        if( j == _size.y && (i == 0 || i == _size.x+1))
                        {
                            _ableToLoop[0] = false;
                        }
                        if (j == 1 && (i == 0 || i == _size.x+1))
                        {
                            _ableToLoop[1] = false;
                        }
                        if (i == 1 && (j == 0 || j == _size.y+1))
                        {
                            _ableToLoop[2] = false;
                        }
                        if (i == _size.x && (j == 0 || j == _size.y+1))
                        {
                            _ableToLoop[3] = false;
                        }

                        setColliderTile(origin, i, j, breakable, tileReplace);
                        breakable = false;
                        tileReplace = null;
                    }
                }
            }
        }

        copyInsiders();

        //�������[�v������
        foreach(var fan in _fanList)
        {
            fan.FanLoopStarted();
        }

        //�S�[���̃��C���[���v���C���[���G����郌�C���[�ɕύX
        if (_goal != null)
        {
            _goal.GoalLayerCheck();
            _goal.FrameCount();
        }

        //�{�^���̃��C���[���v���C���[���G����郌�C���[�ɕύX
        foreach (var button in _buttonList)
        {
            button.ButtonLayerCheck();
        }
    }

    //Tile���Z�b�g����
    private void setColliderTile(Vector2 origin,int i, int j, bool breakble = false, TileReplace tileReplace = null)
    {
        Vector3 pos = origin;

        //���W���t���[���̓����Ȃ炻�̏��Tile���Z�b�g
        if (1 <= i && i <= _size.x)
        {
            if (1 <= j && j <= _size.y)
            {
                Vector3Int intPos = new Vector3Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f));

                if(breakble)
                {
                    tileReplace.Replace(intPos,intPos,true);
                }
                else
                {
                    for(int k=0; k < 3; k++)
                    {
                        for(int l=0; l < 3; l++)
                        {
                            Vector3Int setPos = intPos;
                            setPos.x -= _size.x*(-1+k);
                            setPos.y -= _size.y*(-1+l);

                            _insideTile.SetTile(setPos, _tile);

                        }
                    }

                }

                //-----------------------------------------
                //�����蔻�肪���镔����true�ɏ㏑��
                //-----------------------------------------

                if (j == _size.y)
                {
                    _topHitArray[i - 1] = true;
                    if(breakble)
                    {
                        if(_breakableDic.ContainsKey(tileReplace))
                        {
                            _breakableDic[tileReplace].Add((0, i - 1));
                        }
                        else
                        {
                            _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (0, i - 1) });
                        }
                    }
                }
                else if (j == 1)
                {
                    _bottomHitArray[i - 1] = true;
                    if (breakble)
                    {
                        if (_breakableDic.ContainsKey(tileReplace))
                        {
                            _breakableDic[tileReplace].Add((1, i - 1));
                        }
                        else
                        {
                            _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (1, i - 1) });
                        }
                    }
                }

                if (i == _size.x)
                {
                    _rightHitArray[j - 1] = true;
                    if (breakble)
                    {
                        if (_breakableDic.ContainsKey(tileReplace))
                        {
                            _breakableDic[tileReplace].Add((3, j - 1));
                        }
                        else
                        {
                            _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (3, j - 1) });
                        }
                    }
                }
                else if (i == 1)
                {
                    _leftHitArray[j - 1] = true;
                    if (breakble)
                    {
                        if (_breakableDic.ContainsKey(tileReplace))
                        {
                            _breakableDic[tileReplace].Add((2, j - 1));
                        }
                        else
                        {
                            _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (2, j - 1) });
                        }
                    }
                }
                //------------------------------------------
            }
        }

        //���W���t���[���̒[���O���Ȃ���W�����[�v������
        if (i <= 1 && j != 0 && j != _size.y+1) { pos.x += _size.x; }
        else if (i >= _size.x && j != 0 && j != _size.y + 1) { pos.x -= _size.x; }
        else if (j <= 1 && i != 0 && i != _size.x + 1) { pos.y += _size.y; }
        else if (j >= _size.y && i != 0 && i != _size.x + 1) { pos.y -= _size.y; }

        //����������W���t���[���̊O���Ȃ�����p�̓����蔻��𐶐�
        if (pos.x < _loopRangeX.min || _loopRangeX.max < pos.x||
            pos.y < _loopRangeY.min || _loopRangeY.max < pos.y)
        {
            Vector3Int intPos = new Vector3Int((int)(pos.x-0.5f), (int)(pos.y-0.5f));

            if (breakble)
            {
                Vector3Int beforePos = new Vector3Int((int)(origin.x - 0.5f), (int)(origin.y - 0.5f));
                tileReplace.Replace(intPos, beforePos, true);
            }
            else
            {
                _insideTile.SetTile(intPos, _tile);

            }

            //----------------------------------------------
            //�����蔻�肪���镔�������[�v���l������true�ɏ㏑��
            //----------------------------------------------
            if (j == 1)
            {
                _topHitArray[i-1] = true;
                if (breakble)
                {
                    if (_breakableDic.ContainsKey(tileReplace))
                    {
                        _breakableDic[tileReplace].Add((0, i - 1));
                    }
                    else
                    {
                        _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (0, i - 1) });
                    }
                }
            }
            else if(j == _size.y)
            {
                _bottomHitArray[i-1] = true;
                if (breakble)
                {
                    if (_breakableDic.ContainsKey(tileReplace))
                    {
                        _breakableDic[tileReplace].Add((1, i - 1));
                    }
                    else
                    {
                        _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (1, i - 1) });
                    }
                }
            }

            if (i == 1)
            {
                _rightHitArray[j-1] = true;
                if (breakble)
                {
                    if (_breakableDic.ContainsKey(tileReplace))
                    {
                        _breakableDic[tileReplace].Add((3, j - 1));
                    }
                    else
                    {
                        _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (3, j - 1) });
                    }
                }
            }
            else if (i == _size.x)
            {
                _leftHitArray[j-1] = true;
                if (breakble)
                {
                    if (_breakableDic.ContainsKey(tileReplace))
                    {
                        _breakableDic[tileReplace].Add((2, j - 1));
                    }
                    else
                    {
                        _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (2, j - 1) });
                    }
                }
            }
            //----------------------------------------------
        }
        else
        {
            //����������W���t���[���̓����Ȃ�O���p�̓����蔻��𐶐�
            Vector3Int intPos = new Vector3Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f));
            if (breakble)
            {
                tileReplace.Replace(intPos);
            }
            else
            {
                _outsideTile.SetTile(intPos, _tile);

            }
        }

        //�t���[���̍��E�̒[�ŁA�㉺�̒[���O���Ȃ�㉺�Ƀ��[�v�����ē����蔻��𐶐�
        if (i == 1 || i == _size.x)
        {
            if (j <= 1 || j >= _size.y)
            {
                pos = origin;
                if (j <= 1) { pos.y += _size.y; }
                if (j >= _size.y) { pos.y -= _size.y; }
                Vector3Int intPos = new Vector3Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f));

                //�㉺�̒[�̍��W�Ȃ�����p�̓����蔻��𐶐�
                if(j == 1 || j == _size.y)
                {
                    if (breakble)
                    {
                        Vector3Int beforePos = new Vector3Int((int)(origin.x - 0.5f), (int)(origin.y - 0.5f));
                        tileReplace.Replace(intPos, beforePos, true);
                    }
                    else
                    {
                        _insideTile.SetTile(intPos, _tile);

                    }

                    if(j == 1)
                    {
                        _topHitArray[i-1] = true;
                        if (breakble)
                        {
                            if (_breakableDic.ContainsKey(tileReplace))
                            {
                                _breakableDic[tileReplace].Add((0, i - 1));
                            }
                            else
                            {
                                _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (0, i - 1) });
                            }
                        }
                    }
                    else if(j == _size.y)
                    {
                        _bottomHitArray[i-1] = true;
                        if (breakble)
                        {
                            if (_breakableDic.ContainsKey(tileReplace))
                            {
                                _breakableDic[tileReplace].Add((1, i - 1));
                            }
                            else
                            {
                                _breakableDic.Add(tileReplace, new List<(int switchNum, int num)>() { (1, i - 1) });
                            }
                        }
                    }
                }
                else
                {
                    //�O���Ȃ�O���p�̓����蔻��𐶐�
                    if (breakble)
                    {
                        tileReplace.Replace(intPos);
                    }
                    else
                    {
                        _outsideTile.SetTile(intPos, _tile);
                    }
                }
            }
        }
    }

    //�u���b�N�̓����蔻��𐶐�
    private void ColliderInstantiate(Vector3 pos,int i, int j, Transform parent)
    {
        //���������I�u�W�F�N�g�����R�s�[�I�u�W�F�N�g�Ȃ琶������߂�
        foreach(var tList in _insideCopyDic.Values)
        {
            if (tList.Contains(parent))
            {
                return;
            }
        }

        foreach(var item in _outsideColliderList)
        {
            if(item.transform.parent == parent)
            {
                return;
            }
        }

        //���W�����[�v������
        if (i <= 1 && j != 0 && j != _size.y + 1) { pos.x += _size.x; }
        else if (i >= _size.x && j != 0 && j != _size.y + 1) { pos.x -= _size.x; }
        else if (j <= 1 && i != 0 && i != _size.x + 1) { pos.y += _size.y; }
        else if (j >= _size.y && i != 0 && i != _size.x + 1) { pos.y -= _size.y; }

        //�����悪�����蔻��̒����ǂ������擾
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit2D hit;
        LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform");
        mask |= 1 << LayerMask.NameToLayer("Outside");
        mask |= 1 << LayerMask.NameToLayer("OBox");

        hit = Physics2D.Raycast(ray.origin, ray.direction, 1.0f, mask);

        if(hit.collider != null)
        {
            return;
        }

        //�����蔻��𔠂̎q�I�u�W�F�N�g�Ƃ��Đ���
        var instance = Instantiate(_colliderPrefab, pos, Quaternion.identity, parent);
        var col = instance.GetComponent<Collider2D>();
        instance.layer = 11;
        _outsideColliderList.Add(col);
    }

    //�t���[���������ɂȂ����Ƃ��Ɉ�x���s���郁�\�b�h
    private void onInactive()
    {
        _material.color = new Color32(255, 255, 0, 100);

        //SpriteMask�𖳌��ɂ���
        _spriteMask.enabled = false;

        //�����̃I�u�W�F�N�g�����ׂă`�F�b�N
        foreach(var col in _insiders)
        {
            //SpriteMask�̊O���ŕ\�������悤�ɕύX
            SpriteRenderer renderer = col.GetComponentInChildren<SpriteRenderer>();
            renderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            //�O�ɏo�悤�Ƃ��Ă���I�u�W�F�N�g�̈ʒu���m��
            if (_exitInsiders.ContainsKey(col))
            {
                var currentPos = col.transform.position;
                var setPos = currentPos;

                if (col.CompareTag("Player"))
                {
                    setPos.y -= 0.1f;
                }

                if (setPos.x < _loopRangeX.min)
                {
                    setPos.x += _size.x;
                }
                else if(setPos.x > _loopRangeX.max)
                {
                    setPos.x -= _size.x;
                }

                if (setPos.y < _loopRangeY.min)
                {
                    setPos.y += _size.y;
                }
                else if (setPos.y > _loopRangeY.max)
                {
                    setPos.y -= _size.y;
                }

                //�ʒu�}�X�̍����̍��W�Ƀ��[�v���Ȃ��悤�ɂ���
                if(col.CompareTag("Player"))
                {
                    //���[�v�\��̍��W���擾
                    setPos.y += 0.1f;
                    Vector2 checkPos = setPos;

                    //���̈ʒu�ɓ����蔻�肪���邩�𒲂ׂ�--------------------------------------------------------------
                    checkPos.y += 0.5f;
                    Vector2 screenPos = Camera.main.WorldToScreenPoint(checkPos);
                    Ray ray = Camera.main.ScreenPointToRay(screenPos);
                    RaycastHit2D hit;

                    hit = Physics2D.Raycast(ray.origin, ray.direction, 5.0f, 1 << LayerMask.NameToLayer("OPlatform"));

                    //--------------------------------------------------------------------------------------------------
                    
                    //�����蔻�肪����΃��[�v�O�̍��W�̂܂܂ɂ���
                    if(hit.collider != null)
                    {
                        setPos = currentPos;
                    }
                }

                col.transform.position = setPos;

            }

            //Player�ȊO�̃��C���[��O~~~���C���[�ɕύX
            if (col.CompareTag("Player"))
            {
                continue;
            }

            if (col.CompareTag("Box"))
            {
                var box = col.GetComponent<Box>();
                var offset = box.GetOffset();


                if (offset != Vector2.zero)
                {
                    box.Hold(null);
                }

                box.SetOffset(Vector2.zero);
            }
            col.gameObject.layer--;
        }

        //�g�p�s�\�ɂ���
        g_usable = false;

        //Tilemap��S�ăN���A����
        _insideTile.ClearAllTiles();
        _outsideTile.ClearAllTiles();

        //���������R���C�_�[��Destroy
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

        //�R�s�[�����I�u�W�F�N�g��Destroy
        Dictionary<Collider2D, List<Transform>> workDic = new Dictionary<Collider2D, List<Transform>>(_insideCopyDic); 
        foreach(var col in workDic.Keys)
        {
            foreach(Transform t in _insideCopyDic[col])
            {
                if (t == null)
                {
                    continue;
                }
                _playerInfo.RemoveCopyList(t);
                if (t.CompareTag("Box"))
                {
                    t.GetComponentInParent<Box>().RemoveCopyList(t);
                }
                Destroy(t.gameObject);
            }
        }

        //�R�s�[�����I�u�W�F�N�g��Destroy
        Dictionary<Collider2D, Transform> workDic2 = new Dictionary<Collider2D, Transform>(_outsideCopyDic);
        foreach (var col in workDic2.Keys)
        {
            if (_outsideCopyDic[col] == null)
            {
                continue;
            }
            Destroy(_outsideCopyDic[col].gameObject);
        }

        //���X�g���N���A
        _insideColliderList.Clear();
        _outsideColliderList.Clear();
        _insideCopyDic.Clear();
        _outsideCopyDic.Clear();
        _exitInsiders.Clear();
        _enterOutsiders.Clear();

        //���̃��[�v����߂�
        foreach (var fan in _fanList)
        {
            fan.FanLoopCanceled();
        }

        //�S�[���̃��C���[���v���C���[���G����郌�C���[�ɕύX
        if (_goal != null)
        {
            _goal.SetOutsideLayer();
        }

        //�{�^���̃��C���[���v���C���[���G����郌�C���[�ɕύX
        foreach (var button in _buttonList)
        {
            button.SetOutsideLayer();
        }

        //breakable�I�u�W�F�N�g�̃��Z�b�g
        for (int i = 0; i < _replaceTileList.Count; i++)
        {
            if (_replaceTileList[i] == null)
            {
                _replaceTileList.Remove(_replaceTileList[i]);
                continue;
            }
            _replaceTileList[i].UnReplace();
        }
    }

    //�L�[�������ꂽ���Ɉ�x���s����郁�\�b�h
    public void FrameStarted(InputAction.CallbackContext context)
    {
        //���삪�؂�ւ��Ȃ牟����邽�тɏ�Ԃ�؂�ւ�
        if (_toggle)
        {
            g_isActive = !g_isActive;
            if (g_isActive)
            {
                if (_inputManager != null)
                {
                    _inputManager.SetVibration(0.2f, 0.2f, 0.1f);
                    _inputManager.SetVibration(0.07f, 0.0f, 0f);
                }

                if (g_isActive) { AudioManager.instance.Play("Frame"); }
            }
            else
            {
                if (_inputManager != null)
                {
                    _inputManager.SetVibration(0f, 0f, 0f);
                    _inputManager.SetVibration(0.2f, 0.2f, 0.1f);
                }

                if (!g_isActive)
                {
                    AudioManager.instance.Stop("Frame");
                    AudioManager.instance.Play("FrameTP");
                }
            }
            return;
        }

        //���삪�z�[���h�Ȃ�t���[����L���ɂ���
        g_isActive = true;
        if (_inputManager != null)
        {
            _inputManager.SetVibration(0.2f, 0f, 0f);
        }
    }

    //�L�[�������ꂽ���Ɉ�x���s����郁�\�b�h
    public void FrameCanceled(InputAction.CallbackContext context)
    {
        //���삪�؂�ւ��Ȃ�return
        if (_toggle) { return; }

        //���삪�z�[���h�Ȃ�t���[���𖳌��ɂ���
        _inputManager.SetVibration(0, 0, 0);
        g_isActive = false;
    }

    //���W���v���C���[�̍��W�Œ���
    private void adjustPos()
    {
        //�t���[�����L���Ȃ���W���Œ肷��
        if(g_isActive) { return; }

        //���Ⴊ��ł��邩��y���W�����肷��
        var setPos = _playerTrans.position;
        if (_isCrouching)
        {
            setPos.y += _yOffset_Crouching;
        }
        else
        {
            setPos.y += _yOffset;
        }

        //���W���l�̌ܓ����Đ������W�����߂�
        setPos.x = (float)Math.Round(setPos.x, MidpointRounding.AwayFromZero);
        setPos.y = (float)Math.Round(setPos.y, MidpointRounding.AwayFromZero);
        _transform.position = setPos;
    }

    private void insiderSpriteLink()
    {
        foreach(var col in _insiders)
        {
            if (!_insideCopyDic.ContainsKey(col)) { continue; }

            var renderer = col.GetComponentInChildren<SpriteRenderer>();
            var sprite = renderer.sprite;

            foreach(var t in _insideCopyDic[col])
            {
                if(t == null) { continue; }

                var copyRenderer = t.GetComponent<SpriteRenderer>();
                copyRenderer.sprite = sprite;
            }
        }
    }

    private void copyInsiders()
    {
        //�O���ɏo��I�u�W�F�N�g��S�Ċm�F
        foreach (var col in _insiders)
        {
            //�R�s�[��������Ε�������
            if (!_insideCopyDic.ContainsKey(col))
            {
                bool isBox = false;
                Box box = null;
                if (col.CompareTag("Box"))
                {
                    isBox = true;
                    box = col.GetComponent<Box>();
                }

                //�O���ɏo��I�u�W�F�N�g�̃R�s�[���擾
                GameObject obj = copy(col.transform);

                //���W���R�s�[���̍��W�Ƒ�����
                var pos = col.transform.position;

                //���W���t���[���̑傫�����Ђ��艺�ɂ��炷
                pos -= new Vector3(_size.x, _size.y);

                List<Transform> tList = new List<Transform>();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i == 1 && j == 1) { continue; }

                        //���W���������烋�[�v���Ďq�I�u�W�F�N�g�Ƃ��Đ���
                        //�㉺���E�ɔ��̃R�s�[�����������
                        Vector3 setPos = pos;
                        setPos += new Vector3(_size.x * i, _size.y * j);
                        var instanceObj = Instantiate(obj, setPos, col.transform.rotation, col.transform);
                        tList.Add(instanceObj.transform);

                        if (col.CompareTag("Player"))
                        {
                            _playerInfo.AddCopyList(instanceObj.transform);
                        }

                        if (isBox)
                        {
                            box.AddCopyList(instanceObj.transform);
                        }
                    }
                }

                //�R�s�[�̃��X�g�����X�g�ւ܂Ƃ߂�
                _insideCopyDic.Add(col, tList);

                //�R�s�[�p�̃I�u�W�F�N�g���폜����
                Destroy(obj);
            }
        }
    }

    //�t���[���ɓ����Ă��邷��I�u�W�F�N�g���R�s�[����
    private void instantiateCopy()
    {
        //�����ɓ���I�u�W�F�N�g��S�Ċm�F
        foreach(var col in _enterOutsiders)
        {
            //�R�s�[��������Ε�������
            if (!_outsideCopyDic.ContainsKey(col))
            {
                //�����ɓ���I�u�W�F�N�g�̃R�s�[���擾
                GameObject obj = copy(col.transform);

                //�ǂ���������Ă��Ă��邩���擾
                var vec = _outsiders[col];

                //�����Ă���̂Ɣ��Ε����ɍ��W�����炷
                var pos = col.transform.position;
                pos -= Vector3.Scale(vec, new Vector3(_size.x,_size.y));

                //�q�I�u�W�F�N�g�Ƃ��Đ�������
                var instanceObj = Instantiate(obj, pos, col.transform.rotation, col.transform);

                //���X�g�ɒǉ�����
                _outsideCopyDic.Add(col, instanceObj.transform);

                //�R�s�[�p�̃I�u�W�F�N�g���폜����
                Destroy(obj);
            }
        }
    }

    //�I�u�W�F�N�g���R�s�[
    private GameObject copy(Transform t)
    {
        GameObject obj = new GameObject(t.name + "_copy");
        obj.layer = t.gameObject.layer;

        //�R���|�[�l���g���R�s�[
        SpriteRenderer setRenderer = t.GetComponentInChildren<SpriteRenderer>();
        Rigidbody2D setRigidbody = t.GetComponent<Rigidbody2D>();

        //�R�s�[�����R���|�[�l���g���A�^�b�`
        obj.AddComponent(setRenderer);
        var rb = obj.AddComponent(setRigidbody);
        rb.isKinematic = true;
        rb.useAutoMass = false;

        if (t.CompareTag("Box"))
        {
            //���𕡐�����Ƃ���BoxChild���A�^�b�`
            obj.AddComponent<BoxChild>();
        }

        //�R���C�_�[�R���|�[�l���g���擾
        var col = t.GetComponent<Collider2D>();


        //�R���C�_�[�̃^�C�v�ɂ���ăA�^�b�`����R���|�[�l���g�̃^�C�v��ϊ�
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

        //�I�u�W�F�N�g��Ԃ�
        return obj;
    }

    //�t���[���̓����ɓ��������̃��\�b�h
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("GoalHitBox")) { return; }

        //�R�s�[�I�u�W�F�N�g�Ȃ�return
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

        //���������u���b�N�Ȃ�return
        if(_insideColliderList.Contains(other) || _outsideColliderList.Contains(other))
        {
            return;
        }

        //�O���̃I�u�W�F�N�g�ŁA�t���[�����L���Ȏ�
        if (_outsiders.ContainsKey(other) && g_isActive)
        {
            //�����ɓ���I�u�W�F�N�g�̃��X�g�ɒǉ�
            if (!_enterOutsiders.Contains(other))
            {
                _enterOutsiders.Add(other);
            }
        }
        else
        {
            //�����̎q�I�u�W�F�N�g�Ȃ�return
            if(other.transform.parent != null)
            {
                return;
            }

            //�����̃I�u�W�F�N�g�̃��X�g�ɖ����āA�t���[���������Ȏ�
            if (!_insiders.Contains(other) && !g_isActive)
            {
                //�����̃I�u�W�F�N�g�̃��X�g�ɒǉ�
                _insiders.Add(other);
            }
        }
    }

    //�t���[���̓�������o�����̃��\�b�h
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GoalHitBox")) { return; }

        //�O���ɏo��I�u�W�F�N�g�̃��X�g�ɂ����āA�t���[�����L���Ȃ�
        if (_exitInsiders.ContainsKey(other) && g_isActive)
        {
            //�o�Ă����������擾
            Vector2 vec = _prevExitInsiders[other];

            //���W���擾
            Transform t = other.transform;
            var pos = t.position;

            if(vec.x < 0)
            {
                if(pos.x >= _loopRangeX.min)
                {
                    vec.x = 0;
                }
            }
            else if(vec.x > 0)
            {
                if (pos.x <= _loopRangeX.max)
                {
                    vec.x = 0;
                }
            }


            if (vec.y < 0)
            {
                if (pos.y >= _loopRangeY.min)
                {
                    vec.y = 0;
                }
            }
            else if (vec.y > 0)
            {
                if (pos.y <= _loopRangeY.max)
                {
                    vec.y = 0;
                }
            }

            //�o�Ă��������Ɣ��΂̍��W�Ɉړ�
            vec *= _size;
            pos -= (Vector3)vec;
            t.position = pos;

            if (_playerInfo.g_takeUpFg)
            {
                if (t.CompareTag("Box"))
                {
                    Box box = t.GetComponent<Box>();
                    var offset = box.GetOffset();
                    offset -= vec;
                    box.SetOffset(offset);
                }
                else if (t.CompareTag("Player"))
                {
                    if (_playerInfo.g_box != null)
                    {
                        Box box = _playerInfo.g_box.GetComponent<Box>();
                        var offset = box.GetOffset();
                        offset += vec;
                        box.SetOffset(offset);
                    }
                }
            }
        }

        if (_insiders.Contains(other))
        {
            if (!g_isActive)
            {
                //�����̃I�u�W�F�N�g�̃��X�g�ɂ����āA�t���[���������Ȃ烊�X�g����폜
                _insiders.Remove(other);
                if (_exitInsiders.ContainsKey(other))
                {
                    _exitInsiders.Remove(other);
                }
            }
        }

        //�O�ɏo��I�u�W�F�N�g�Ȃ烊�X�g����폜
        if (_enterOutsiders.Contains(other))
        {
            _enterOutsiders.Remove(other);
        }
    }

    //�t���[���̎���̓����蔻��ɓ������Ƃ��̃��\�b�h
    public void OnStay(Collider2D other,Transform transform)
    {
        if (other.CompareTag("GoalHitBox")) { return; }

        //�R�s�[�I�u�W�F�N�g�Ȃ�return
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

        //���������R���C�_�[�Ȃ�return
        if(_insideColliderList.Contains(other) || _outsideColliderList.Contains(other))
        {
            return;
        }

        //�ǂ���������Ă��Ă邩���擾
        Vector2 vec = Vector2.zero;
        if (transform == _topT) { vec.y = 1; }
        if (transform == _bottomT) { vec.y = -1; }
        if (transform == _rightT) { vec.x = 1; }
        if (transform == _leftT) { vec.x = -1; }

        //�����̃I�u�W�F�N�g�̃��X�g�ɂ����āA�t���[�����L���Ȃ�
        if (_insiders.Contains(other) && g_isActive)
        {
            //�o�Ă����I�u�W�F�N�g�̃��X�g�ɒǉ�����
            if (!_exitInsiders.ContainsKey(other))
            {
                _exitInsiders.Add(other, vec);
            }
            else
            {
                var setVec = _exitInsiders[other];
                setVec += vec;
                _exitInsiders[other] = setVec;
            }
        }
        else
        {
            //Player�Ȃ�return
            if (other.CompareTag("Player")) { return; }

            Vector3 otherPos = other.transform.position;

            bool ignoreCol = false;

            //�p�̏ꍇ�Ƀ��[�v�\�Ȃ瓖���蔻�������
            if(vec.x != 0 )//������
            {
                //��̒[
                if (_loopRangeY.max - 1 < otherPos.y && otherPos.y < _loopRangeY.max)
                {
                    if (_ableToLoop[0])
                    {
                        Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), _outsideTileCol);
                        ignoreCol = true;
                    }

                }
                //���̒[
                else if (_loopRangeY.min < otherPos.y && otherPos.y < _loopRangeY.min + 1)
                {
                    if (_ableToLoop[1])
                    {
                        Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), _outsideTileCol);
                        ignoreCol = true;
                    }
                }
            }
            else if (vec.x != 0)//�㉺����
            {
                //���̒[
                if (_loopRangeX.min < otherPos.x && otherPos.x < _loopRangeY.min + 1)
                {
                    if (_ableToLoop[2])
                    {
                        Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), _outsideTileCol);
                        ignoreCol = true;
                    }

                }
                //�E�̒[
                else if (_loopRangeX.max - 1 < otherPos.x && otherPos.x < _loopRangeX.max)
                {
                    if (_ableToLoop[3])
                    {
                        Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), _outsideTileCol);
                        ignoreCol = true;
                    }
                }
            }


            //�O���̃I�u�W�F�N�g�̃��X�g�ɖ�����Βǉ�����
            if (!_outsiders.ContainsKey(other))
            {
                _outsiders.Add(other, vec);
            }
            else
            {
                _outsiders[other] = vec;
                if (!ignoreCol)//���[�v�\����Ȃ��Ȃ瓖���蔻��𕜊�
                {
                    Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), _outsideTileCol, false);
                }
            }
        }
    }

    //�t���[���̎���̓����蔻�肩��o�����̃��\�b�h
    public void OnExit(Collider2D other, Transform transform)
    {
        if (other.CompareTag("GoalHitBox")) { return; }

        //�����ɓ���I�u�W�F�N�g�̃��X�g�ɂ����āA�t���[�����L���Ȃ�
        if (_enterOutsiders.Contains(other) && g_isActive)
        {
            //�����Ă���������擾
            Vector2 vec = _outsiders[other];

            //���W���擾
            Transform t = other.transform;
            var pos = t.position;

            //�����Ă�������Ɣ��΂ɍ��W���ړ�
            vec *= _size;
            pos -= (Vector3)vec;
            t.position = pos;

            //�R�s�[�I�u�W�F�N�g���폜����
            if (_outsideCopyDic.ContainsKey(other))
            {
                Destroy(_outsideCopyDic[other].gameObject);
            }

            //�����蔻���߂�
            Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), _outsideTileCol, false);

            _outsideCopyDic.Remove(other);
        }

        //�O���̃I�u�W�F�N�g�̃��X�g�ɂ�������폜����
        if (_outsiders.ContainsKey(other))
        {
            _outsiders.Remove(other);

            //�����ɓ���I�u�W�F�N�g�̃��X�g�ɂ�������폜����
            if (_enterOutsiders.Contains(other))
            {
                _enterOutsiders.Remove(other);
            }
        }

        //�O���ɏo��I�u�W�F�N�g�̃��X�g�ɂ�������폜����
        if (_exitInsiders.ContainsKey(other))
        {
            _exitInsiders.Remove(other);
        }
    }

    public void OnEnter(Collider2D other, Transform transform)
    {
        //�t���[���̎���̓����蔻��ɓ������u�ԂɌĂԃ��\�b�h
    }

    //�v���C���[�����Ⴊ��ł邩����
    public void SetCrouching(bool isCrouching)
    {
        _isCrouching = isCrouching;
    }

    //�t���[���̃T�C�Y��Vector2Int�ŕԂ�
    public Vector2Int GetSize()
    {
        return _size;
    }

    //�����蔻��̔z���Ԃ�
    public bool[] GetHitArray(int select)
    {
        switch(select)
        {
            case 0:
                return _topHitArray;
            case 1:
                return _bottomHitArray;
            case 2:
                return _leftHitArray;
            case 3:
                return _rightHitArray;
            default:
                return null;
        }
    }

    public Dictionary<TileReplace, List<(int, int)>> GetBreakableDic()
    {
        return _breakableDic;
    }
}

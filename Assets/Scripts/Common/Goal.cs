using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*  ProjectName :FrameLoop
 *  ClassName   :Goal
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Goal���������
 *               
 *  Created     :2024/06/12
 */
public class Goal : SingletonMonoBehaviour<Goal>
{
    [SerializeField, Tooltip("�S�[���ɕK�v�ȃt���[���̎g�p��")]
    private int _minFrameCount = 1;
    [SerializeField, Tooltip("�ŏ��ɑI������{�^��")]
    private GameObject _selectButton;
    [SerializeField]
    private GameObject _clearCanvas;

    private int _buttonCount = 0;
    private int _count = 0;

    private Animator _animator;

    private BoxCollider2D _boxCollider;
    private Vector2 _offset = Vector2.zero;

    private bool _isOpened = false;
    private bool _inputW = false;
    private bool _clear = false;

    private int _frameCount = 0;

    private List<Animator> _starAnimators = new List<Animator>();
    private bool[] _starBrightArray = new bool[4] { true, true, true, true };

    private Animator _clearScreenAnimator = null;

    private PlayerInfo _playerInfo = null;
    private PlayerInput _playerInput;

    private GamepadUISelect _gamepadUISelect;

    //�Œ�񐔁{����܂ł�2�ɂ��邩
    private const int STAR_GAP = 1;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        _boxCollider = GetComponent<BoxCollider2D>();
        _offset = _boxCollider.offset;

        _playerInfo = PlayerInfo.Instance;
        _playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();

        _gamepadUISelect = GetComponent<GamepadUISelect>();

        var animatorArray = GetComponentsInChildren<Animator>();
        foreach (var animator in animatorArray)
        {
            if (animator.CompareTag("Star"))
            {
                _starAnimators.Add(animator);
                animator.SetBool("Bright", true);
            }
            else if (animator.CompareTag("ClearScreen"))
            {
                _clearScreenAnimator = animator;
            }
        }
        _clearCanvas.SetActive(false);

        var objs = GameObject.FindGameObjectsWithTag("Button");
        foreach( var obj in objs)
        {
            if (obj.GetComponent<Button>().IsToGoal())
            {
                _buttonCount++;
            }
        }
    }

    private void Update()
    {
        _isOpened = _count >= _buttonCount;
        _animator.SetBool("isOpened", _isOpened);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != _playerInfo.g_goalHitBox) { return; }

        //�N���A�ς݂Ȃ�return
        if (_clear) { return; }

        //�{�^���̐�������Ă��āA�ڒn���Ă��āA���͂�����΃S�[��
        if(_isOpened && _inputW && PlayerInfo.Instance.g_isGround)
        {
            _clear = true;
            _clearCanvas.SetActive(true);

            for(int i=0; i < _starAnimators.Count; i++)
            {
                _starAnimators[i].SetBool("Bright", _starBrightArray[i]);
            }

            _playerInput.SwitchCurrentActionMap("UI");

            if(_gamepadUISelect != null)
            {
                _gamepadUISelect.SetEnable(true);
            }

            EventSystem.current.SetSelectedGameObject(_selectButton);

            _clearScreenAnimator.SetTrigger("Scale");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //Reset frame sound
            AudioManager.instance.Stop("Frame");
        }
    }


    public void GoalStarted(InputAction.CallbackContext context)
    {
        _inputW = true;
    }

    public void GoalCanceled(InputAction.CallbackContext context)
    {
        _inputW = false;
    }

    //�t���[���ɏd�Ȃ��Ă��邩�Ń��C���[��ύX����
    public void GoalLayerCheck()
    {
        //�X�N���[�����W�ɕϊ�
        var pos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)_offset);

        //���W�Ɉʒu�Ƀ��C���΂�
        Ray ray = Camera.main.ScreenPointToRay(pos);
        LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10, mask);

        if (hit.collider != null)
        {
            gameObject.layer = LayerMask.NameToLayer("Inside");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Outside");
        }
    }

    //���C���[��߂�
    public void SetOutsideLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Outside");
    }

    //�{�^���̐������Z
    public void CountUp()
    {
        _count++;
    }

    //�{�^���̐������Z
    public void CountDown()
    {
        _count--;
    }

    public void FrameCount()
    {
        _frameCount++;
        SetStarParamater();
    }

    private void SetStarParamater()
    {
        if(_frameCount <= _minFrameCount)
        {
            for(int i=0; i < _starBrightArray.Length; i ++)
            {
                _starBrightArray[i] = true;
            }
        }
        else if(_frameCount <= _minFrameCount + STAR_GAP)
        {
            _starBrightArray[0] = true;
            _starBrightArray[1] = true;
            _starBrightArray[2] = false;
        }                       
        else                    
        {                       
            _starBrightArray[0] = true;
            _starBrightArray[1] = false;
            _starBrightArray[2] = false;
        }
    }

    public void Next()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        //�Ō��Scene�Ȃ�^�C�g���Ɉړ�����
        if (currentScene.name == "lvl 18")
        {
            SceneLoader.Instance.LoadScene("MainMenu");
        }
        else
        {
            SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Retry()
    {
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Title()
    {
        SceneLoader.Instance.LoadScene("MainMenu");
    }
}

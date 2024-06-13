using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*  ProjectName :FrameLoop
 *  ClassName   :Goal
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Goal判定をする
 *               
 *  Created     :2024/06/12
 */
public class Goal : SingletonMonoBehaviour<Goal>
{
    [SerializeField, Tooltip("ゴールに必要なフレームの使用回数")]
    private int _minFrameCount = 1;
    [SerializeField, Tooltip("最初に選択するボタン")]
    private GameObject _selectButton;

    private Canvas _clearCanvas = null;
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
    private Animator _clearScreenAnimator = null;

    private PlayerInfo _playerInfo = null;
    private PlayerInput _playerInput;

    private GamepadUISelect _gamepadUISelect;

    //最低回数＋何回までを星2つにするか
    private const int STAR_GAP = 1;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _clearCanvas = GetComponentInChildren<Canvas>();

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
        _clearCanvas.enabled = false;

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

        //クリア済みならreturn
        if (_clear) { return; }

        //ボタンの数が足りていて、接地していて、入力があればゴール
        if(_isOpened && _inputW && PlayerInfo.Instance.g_isGround)
        {
            _clear = true;
            _clearCanvas.enabled = true;
            _playerInput.SwitchCurrentActionMap("UI");

            if(_gamepadUISelect != null)
            {
                _gamepadUISelect.SetEnable(true);
            }


            EventSystem.current.SetSelectedGameObject(_selectButton);

            _clearScreenAnimator.SetTrigger("Scale");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

    //フレームに重なっているかでレイヤーを変更する
    public void GoalLayerCheck()
    {
        //スクリーン座標に変換
        var pos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)_offset);

        //座標に位置にレイを飛ばす
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

    //レイヤーを戻す
    public void SetOutsideLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Outside");
    }

    //ボタンの数を加算
    public void CountUp()
    {
        _count++;
    }

    //ボタンの数を減算
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
            foreach(var anim in _starAnimators)
            {
                anim.SetBool("Bright", true);
            }
        }
        else if(_frameCount <= _minFrameCount + STAR_GAP)
        {
            _starAnimators[0].SetBool("Bright", true);
            _starAnimators[1].SetBool("Bright", true);
            _starAnimators[2].SetBool("Bright", false);
        }
        else
        {
            _starAnimators[0].SetBool("Bright", true);
            _starAnimators[1].SetBool("Bright", false);
            _starAnimators[2].SetBool("Bright", false);
        }
    }

    public void Next()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Title()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

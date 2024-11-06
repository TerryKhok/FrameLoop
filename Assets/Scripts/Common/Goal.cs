using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private GameObject _clearCanvas_challenge;
    [SerializeField]
    private GameObject _clearCanvas_casual;

    [SerializeField]
    private TextMeshProUGUI _minText, _usedText;

    private int _buttonCount = 0;
    private int _count = 0;

    private Animator _animator;

    private BoxCollider2D _boxCollider;
    private Vector2 _offset = Vector2.zero;

    private bool _isOpened = false;
    private bool _inputW = false;
    public bool g_clear = false;

    private int _frameCount = 0;

    private List<Animator> _starAnimators = new List<Animator>();
    private bool[] _starBrightArray = new bool[4] { true, true, true, true };

    [SerializeField]
    private Animator _clearScreenAnimator_challenge = null;
    [SerializeField]
    private Animator _clearScreenAnimator_casual = null;

    private PlayerInfo _playerInfo = null;
    private PlayerInput _playerInput;

    [SerializeField]
    private GamepadUISelect _gamepadUISelect_challenge;
    [SerializeField]
    private GamepadUISelect _gamepadUISelect_casual;
    private bool _buttonSelected = false;
    //最低回数＋何回までを星2つにするか
    private const int STAR_GAP = 1;
    private int _starCount = 3;
    private int _stageIndex = -1;

    private static bool s_isChallenge = false;

    private TutorialPC _tutorialPC;

    public static bool IsChallenge
    {
        set => s_isChallenge = value;
    }

    private void Start()
    {
        AudioManager.instance.Stop("PlayerWin");

        _animator = GetComponent<Animator>();

        _boxCollider = GetComponent<BoxCollider2D>();
        _offset = _boxCollider.offset;

        _playerInfo = PlayerInfo.Instance;
        _playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();

        _stageIndex = SceneManager.GetActiveScene().buildIndex - 1;

        var animatorArray = GetComponentsInChildren<Animator>();
        foreach (var animator in animatorArray)
        {
            if (animator.CompareTag("Star"))
            {
                _starAnimators.Add(animator);
                animator.SetBool("Bright", true);
            }
        }
        _clearCanvas_challenge.SetActive(false);
        _clearCanvas_casual.SetActive(false);

        var objs = GameObject.FindGameObjectsWithTag("Button");
        foreach( var obj in objs)
        {
            if (obj.GetComponent<Button>().IsToGoal())
            {
                _buttonCount++;
            }
        }

        _tutorialPC = GetComponent<TutorialPC>();
    }

    private void Update()
    {
        _isOpened = _count >= _buttonCount;
        _animator.SetBool("isOpened", _isOpened);

        _tutorialPC.enabled = _isOpened;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != _playerInfo.g_goalHitBox) { return; }

        //クリア済みならreturn
        if (g_clear) { return; }

        //ボタンの数が足りていて、接地していて、入力があればゴール
        if(_isOpened && _inputW && PlayerInfo.Instance.g_isGround)
        {
            OnGoal();
        }
    }

    private void OnGoal()
    {
        //Debug.Log(s_isChallenge);

        g_clear = true;
        if (_gamepadUISelect_challenge != null && s_isChallenge)
        {
            _clearCanvas_challenge.SetActive(true);

            _gamepadUISelect_challenge.SetEnable(true);
            _clearScreenAnimator_challenge.SetTrigger("Scale");
        }

        AudioManager.instance.Play("PlayerWin");

        for (int i = 0; i < _starAnimators.Count; i++)
        {
            _starAnimators[i].SetBool("Bright", _starBrightArray[i]);
        }

        _minText.text = _minFrameCount.ToString();
        _usedText.text = _frameCount.ToString();

        _playerInput.SwitchCurrentActionMap("UI");

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //カーソルを表示する
        if (_playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //Reset frame sound
        AudioManager.instance.Stop("Frame");

        if (SaveManager.g_saveData != null)
        {
            SaveManager.g_saveData.g_clearFlag[_stageIndex] = true;
            SaveManager.g_saveData.g_starCount[_stageIndex] = _starCount;
        }

        if(!s_isChallenge)
        {
            Next();
        }
    }

    public void GoalStarted(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        if(value.y < 0.85f)
        {
            return;
        }

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
            for(int i=0; i < _starBrightArray.Length; i ++)
            {
                _starBrightArray[i] = true;
            }
            _starCount = 3;
        }
        else if(_frameCount <= _minFrameCount + STAR_GAP)
        {
            _starBrightArray[0] = true;
            _starBrightArray[1] = true;
            _starBrightArray[2] = false;
            _starCount = 2;
        }                       
        else                    
        {                       
            _starBrightArray[0] = true;
            _starBrightArray[1] = false;
            _starBrightArray[2] = false;
            _starCount = 1;
        }
    }

    public void Next()
    {
        if (_buttonSelected) { return; }

        Scene currentScene = SceneManager.GetActiveScene();
        //最後のSceneならタイトルに移動する
        if (currentScene.name == "lvl 27")
        {
            SceneLoader.Instance.LoadScene("AppreciateScene");
        }
        else
        {
            SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        _buttonSelected = true;
    }

    public void Retry()
    {
        if (_buttonSelected) { return; }
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);

        _buttonSelected = true;
    }

    public void Title()
    {
        if (_buttonSelected) { return; }
        SceneLoader.Instance.LoadScene("MainMenu");
        _buttonSelected = true;
    }
}

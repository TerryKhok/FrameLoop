using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnterStage : MonoBehaviour
{
    [SerializeField, Tooltip("ステージのscene名")]
    private string _sceneName = "";

    [SerializeField]
    private bool _isClear = false;
    [SerializeField]
    private int _stageIndex = -1;
    [SerializeField]
    private bool _isWorldEnter = false;
    [SerializeField]
    private int _firstStageIndex = 0, _finalStageIndex = 0;

    private bool _inputW = false;
    private FrameLoop _frameLoop;
    private PlayerInfo _playerInfo;
    private Animator _animator;
    private Vector2 _offset = Vector2.zero;

    private bool _isEnter = false, _isArrived = false;

    private TutorialPC[] _tutorialPCArray = new TutorialPC[2];

    private void Start()
    {
        _frameLoop = FrameLoop.Instance;
        _playerInfo = PlayerInfo.Instance;
        _animator = GetComponent<Animator>();

        Debug.Log($"{_playerInfo.GetPrevSceneName()}:{_sceneName}");

        if(_playerInfo.GetPrevSceneName() == _sceneName)
        {
            Vector3 pos = transform.position;
            pos.y -= 1.0f;
            _playerInfo.g_transform.position = pos;
        }

        _offset = GetComponent<BoxCollider2D>().offset;
        _tutorialPCArray = GetComponents<TutorialPC>();
        _isClear = false;
        _isArrived = false;

        if (_isWorldEnter)
        {
            _isClear = true;
            for (int i = _firstStageIndex; i <= _finalStageIndex; ++i)
            {
                _isArrived |= SaveManager.SaveDataInstance.g_arriveFlag[i];
                _isClear &= SaveManager.SaveDataInstance.g_clearFlag[i];
            }
        }
        else
        {
            if (SaveManager.SaveDataInstance.g_arriveFlag[_stageIndex])
            {
                _isArrived = true;
            }

            if (SaveManager.SaveDataInstance.g_clearFlag[_stageIndex])
            {
                _isClear = true;
            }
        }

        if(_tutorialPCArray != null)
        {
            foreach(var tutorial in _tutorialPCArray)
            {
                tutorial.SetActive(_isArrived);
            }
        }

    }

    private void Update()
    {
        if (_animator != null)
        {
            _animator.SetBool("isOpened", _isClear);
            _animator.SetBool("isArrived", _isArrived);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //-------------------------------------------------------------------
        //　デバッグ用にしゃがんでれば入れるようにする
        //-------------------------------------------------------------------
        if ((/*!_playerInfo.g_isCrouch &&*/ !_isArrived) || _isEnter) { return; }
        if (collision != _playerInfo.g_goalHitBox) { return; }

        if (_inputW && _playerInfo.g_isGround)
        {
            SceneLoader.Instance.LoadScene(_sceneName);
            _isEnter = true;
        }
    }

    public void EnterStarted(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        if(value.y < 0.85f)
        {
            return;
        }

        _inputW = true;
    }

    public void EnterCanceled(InputAction.CallbackContext context)
    {
        _inputW = false;
    }

    //フレームに重なっているかでレイヤーを変更する
    public void LayerCheck()
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
}

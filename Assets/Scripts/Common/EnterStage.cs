using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnterStage : MonoBehaviour
{
    [SerializeField, Tooltip("ステージのscene名")]
    private string _sceneName = "";

    [SerializeField]
    private bool _isOpened = false;
    [SerializeField]
    private int _stageIndex = -1;

    private bool _inputW = false;
    private FrameLoop _frameLoop;
    private PlayerInfo _playerInfo;
    private Animator _animator;
    private Vector2 _offset = Vector2.zero;

    private bool _isEnter = false;

    private TutorialPC _tutorial = null;

    private void Start()
    {
        _frameLoop = FrameLoop.Instance;
        _playerInfo = PlayerInfo.Instance;
        _animator = GetComponent<Animator>();

        if(_playerInfo.GetPrevSceneName() == _sceneName)
        {
            Vector3 pos = transform.position;
            pos.y -= 1.0f;
            _playerInfo.g_transform.position = pos;
        }

        _offset = GetComponent<BoxCollider2D>().offset;
        _tutorial = GetComponent<TutorialPC>();

        if(_stageIndex == 0)
        {
            _isOpened = true;
        }
        else if(SaveManager.g_saveData.g_clearFlag[_stageIndex - 1])
        {
            _isOpened = true;
        }
        else
        {
            _isOpened = false;
        }
        _tutorial.enabled = _isOpened;
    }

    private void Update()
    {
        if (_animator != null)
        {
            _animator.SetBool("isOpened", _isOpened);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //-------------------------------------------------------------------
        //　デバッグ用にしゃがんでれば入れるようにする
        //-------------------------------------------------------------------
        if ((!_playerInfo.g_isCrouch && !_isOpened) || _isEnter) { return; }
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

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnterStage : MonoBehaviour
{
    [SerializeField, Tooltip("ステージのscene名")]
    private string _sceneName = "";

    [SerializeField]
    private bool _isOpened = false;

    private bool _inputW = false;
    private FrameLoop _frameLoop;
    private PlayerInfo _playerInfo;
    private Animator _animator;
    private Vector2 _offset = Vector2.zero;

    private bool _isEnter = false;

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
    }

    private void Update()
    {
        if (_animator != null)
        {
            _animator.SetBool("isOpened", _isOpened);
        }

        if (_frameLoop.g_isActive)
        {
            gameObject.layer = LayerMask.NameToLayer("Inside");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Outside");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isOpened || _isEnter) { return; }
        if (collision != _playerInfo.g_goalHitBox) { return; }

        if (_inputW && _playerInfo.g_isGround)
        {
            SceneLoader.Instance.LoadScene(_sceneName);
            _isEnter = true;
        }
    }

    public void EnterStarted(InputAction.CallbackContext context)
    {
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

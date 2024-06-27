using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

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
    }

    private void Update()
    {
        _animator.SetBool("isOpened", _isOpened);

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
}

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

    private void Start()
    {
        _frameLoop = FrameLoop.Instance;
        _playerInfo = PlayerInfo.Instance;
        _animator = GetComponent<Animator>();
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
        if (!_isOpened) { return; }
        if (collision != _playerInfo.g_goalHitBox) { return; }

        if (_inputW && _playerInfo.g_isGround)
        {
            SceneManager.LoadScene(_sceneName);
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

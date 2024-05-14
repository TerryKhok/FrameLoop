using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterStage : MonoBehaviour
{
    [SerializeField, Tooltip("ステージのscene名")]
    private string _sceneName = "";

    [SerializeField]
    private bool _isOpened = false;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isOpened) { return; }

        if (collision.transform == _playerInfo.g_transform)
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}

using UnityEngine;
using Cinemachine;

public class PlayerOpening : MonoBehaviour
{
    [System.Serializable]
    public struct MoveInfo
    {
        public float time;
        public int direction;
    }

    [SerializeField]
    private MoveInfo[] _moveInfos;
    [SerializeField]
    private float[] _frameTiming;
    [SerializeField]
    private float[] _jumpTiming;
    [SerializeField]
    private float _sceneChangeTiming;
    [SerializeField]
    private float _exclamationTiming;
    [SerializeField]
    private GameObject _frameParent;
    [SerializeField]
    private GameObject _exclamation;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    private FrameLoop _frameLoop = null;
    private PlayerMove _playerMove = null;
    private PlayerJump _playerJump = null;
    private float _elapsedTime = 0;
    private int _moveIndex = 0, _frameIndex = 0, _jumpIndex = 0;
    private bool _exclamationFlag = false;

    private void Start()
    {
        _frameLoop = FrameLoop.Instance;
        _playerJump = GetComponent<PlayerJump>();
        _playerMove = GetComponent<PlayerMove>();
        _frameParent.SetActive(false);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        // åoâﬂéûä‘Ç∆î‰ärÇµÇƒÅAâﬂÇ¨ÇƒÇ¢ÇÍÇŒóvëfî‘çÜÇëùÇ‚Ç∑
        if (_moveInfos[_moveIndex].time <= _elapsedTime)
        {
            ++_moveIndex;
        }
        _playerMove.SetMove(_moveInfos[_moveIndex].direction);

        if (_jumpTiming[_jumpIndex] <= _elapsedTime)
        {
            _playerJump.SetJump();
            ++_jumpIndex;
        }

        if (_frameTiming[_frameIndex] <= _elapsedTime)
        {
            if(_frameIndex%2 == 0)
            {
                _virtualCamera.enabled = false;
            }
            else
            {
                _virtualCamera.enabled = true;
            }

            _frameLoop.SwitchFrame();
            ++_frameIndex;
        }

        if(_sceneChangeTiming <= _elapsedTime)
        {
            SceneLoader.Instance.LoadScene("lvl 1");
        }

        if (_exclamationTiming <= _elapsedTime && !_exclamationFlag)
        {
            _exclamationFlag = true;

            var pos = transform.position;

            pos += new Vector3(1.0f, 1.0f);

            var instance = Instantiate(_exclamation, pos, Quaternion.identity);
            Destroy(instance, 1.0f);
        }
    }

    public void GetFrameMachine()
    {
        _frameParent.SetActive(true);
    }
}

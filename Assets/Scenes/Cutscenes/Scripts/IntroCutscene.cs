using UnityEngine;
using Cinemachine;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField]
    private float appearanceTime;

    [System.Serializable]
    public struct MoveInfo
    {
        public float time;
        public int direction;
    }

    [System.Serializable]
    public struct Timing
    {
        public float start, end;
    }

    [SerializeField]
    private Timing[] _smileTiming;
    private int _smileIndex = 0;

    [SerializeField]
    private MoveInfo[] _moveInfos;
    [SerializeField]
    private float[] _frameTiming;
    [SerializeField]
    private float[] _jumpTiming;
    [SerializeField]
    private float _sceneChangeTiming;
    [SerializeField]
    private GameObject _frameParent;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    private FrameLoop _frameLoop = null;
    private PlayerMove _playerMove = null;
    private PlayerJump _playerJump = null;
    private float _elapsedTime = 0;
    private int _moveIndex = 0, _frameIndex = 0, _jumpIndex = 0;

    private GameObject playerSpriteObject;

    private void Start()
    {
        _frameLoop = FrameLoop.Instance;
        _playerJump = GetComponent<PlayerJump>();
        _playerMove = GetComponent<PlayerMove>();
        _frameParent.SetActive(false);

        playerSpriteObject = GameObject.Find("PlayerSprite");
        playerSpriteObject.SetActive(false);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if(_elapsedTime >= appearanceTime)
        {
            playerSpriteObject.SetActive(true);
        }

        // åoâﬂéûä‘Ç∆î‰ärÇµÇƒÅAâﬂÇ¨ÇƒÇ¢ÇÍÇŒóvëfî‘çÜÇëùÇ‚Ç∑
        if (_moveInfos[_moveIndex].time <= _elapsedTime)
        {
            ++_moveIndex;
        }
        _playerMove.SetMove(_moveInfos[_moveIndex].direction);

        if (_frameTiming[_frameIndex] <= _elapsedTime)
        {
            if (_frameIndex % 2 == 0)
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

        if (_sceneChangeTiming <= _elapsedTime)
        {
            SceneLoader.Instance.LoadScene("lvl 1");
        }

        if (_smileTiming[_smileIndex].start <= _elapsedTime)
        {
            PlayerAnimation.Instance.SetSmile(true);
            //Debug.Log("smile");
        }

        if (_smileTiming[_smileIndex].end <= _elapsedTime)
        {
            _smileIndex++;
            PlayerAnimation.Instance.SetSmile(false);
        }
    }

    public void GetFrameMachine()
    {
        _frameParent.SetActive(true);
    }
}

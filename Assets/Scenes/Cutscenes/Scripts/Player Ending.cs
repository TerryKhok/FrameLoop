using UnityEngine;

public class PlayerCutScene : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainPlayerSprite = null, _climePlayerSprite = null;

    [System.Serializable]
    public struct MoveInfo
    {
        public float time;
        public int moveDirection;
        public float targetVelocity;
        public int lookDirection;
    }

    [SerializeField]
    private MoveInfo[] _moveInfos;

    [System.Serializable]
    public struct Timing
    {
        public float start, end;
    }

    [SerializeField]
    private Timing[] _smileTiming;
    private float _smileElapsedTime = 0;
    private int _smileIndex = 0;

    [SerializeField]
    private float _climbStart = 30;
    private float _climbElapsedTime = 0;
    private bool _climbDone = false;

    private PlayerMove _playerMove = null;
    private float _elapsedTime = 0;
    private int _moveIndex = 0;

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _climePlayerSprite.SetActive(false);
    }

    private void Update()
    {
        if(_climbDone)
        {
            var currentPos = transform.position;
            currentPos.y += 1 * Time.deltaTime;
            transform.position = currentPos;
            return;
        }
        _elapsedTime += Time.deltaTime;
        _smileElapsedTime += Time.deltaTime;
        _climbElapsedTime += Time.deltaTime;
        //Debug.Log(_climbElapsedTime);

        // 経過時間と比較して、過ぎていれば要素番号を増やす
        if (_moveInfos[_moveIndex].time <= _elapsedTime)
        {
            ++_moveIndex;
            _elapsedTime = 0;
        }

        if (_smileTiming[_smileIndex].start <= _smileElapsedTime)
        {
            PlayerAnimation.Instance.SetSmile(true);
            //Debug.Log("にこ！");
        }

        if (_smileTiming[_smileIndex].end <= _smileElapsedTime)
        {
            _smileIndex++;
            _smileElapsedTime = 0;
            PlayerAnimation.Instance.SetSmile(false);
        }

        if (_climbStart <= _climbElapsedTime && !_climbDone)
        {
            _climbDone = true;
            _mainPlayerSprite.SetActive(false);
            _climePlayerSprite.SetActive(true);
            _climePlayerSprite.GetComponent<Animator>().Play("PlayerClimb");
            transform.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
            _playerMove.SetMove(0);
        }

        _playerMove.SetMove(_moveInfos[_moveIndex].moveDirection);
        _playerMove.SetRotate(_moveInfos[_moveIndex].lookDirection);
        _playerMove.SetTargetVelocity(_moveInfos[_moveIndex].targetVelocity);
    }
}
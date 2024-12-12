using UnityEngine;

public class PlayerEnding : MonoBehaviour
{
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

    [SerializeField]
    private float[] _smileTiming;
    private float _smileElapsedTime = 0;
    private int _smileIndex = 0;

    private PlayerMove _playerMove = null;
    private float _elapsedTime = 0;
    private int _moveIndex = 0;

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        _smileElapsedTime += Time.deltaTime;

        // �o�ߎ��ԂƔ�r���āA�߂��Ă���Ηv�f�ԍ��𑝂₷
        if (_moveInfos[_moveIndex].time <= _elapsedTime)
        {
            ++_moveIndex;
            _elapsedTime = 0;
        }

        if (_smileTiming[_smileIndex] <= _smileElapsedTime)
        {
            ++_smileIndex;
            _smileElapsedTime = 0;

            PlayerAnimation.Instance.PlaySmileAnimation();
            //Debug.Log("�ɂ��I");
        }

        _playerMove.SetMove(_moveInfos[_moveIndex].moveDirection);
        _playerMove.SetRotate(_moveInfos[_moveIndex].lookDirection);
        _playerMove.SetTargetVelocity(_moveInfos[_moveIndex].targetVelocity);
    }
}
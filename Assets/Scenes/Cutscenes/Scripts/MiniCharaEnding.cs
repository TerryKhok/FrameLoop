using UnityEngine;

public class MiniCharaEnding : MonoBehaviour
{
    [System.Serializable]
    public struct MoveInfo
    {
        public float time;
        public int moveDirection;
        public int lookDirection;
        public float velocity;
        public bool jump;
    }

    [SerializeField]
    private MoveInfo[] _moveInfos;

    [SerializeField]
    private float[] _smileTiming;
    private float _smileElapsedTime = 0;
    private int _smileIndex = 0;

    private Rigidbody2D _rb = null;
    private Transform _transform = null;
    private MiniCharaAnimation _anim = null;
    private float _elapsedTime = 0;
    private int _moveIndex = -1;
    private float _velocity = 0;

    private LayerMask _mask;
    private Vector2 _size = new Vector2(0.7f, 1);
    private bool _isLanding = false;

    private int _currentMoveDirection = 0, _currentLookDirection = 0;

    private void Start()
    {
        _transform = transform;

        _mask = 1 << LayerMask.NameToLayer("OPlatform");
        _mask |= 1 << LayerMask.NameToLayer("OBox");

        _rb = _transform.GetComponent<Rigidbody2D>();
        _anim = _transform.GetComponentInChildren<MiniCharaAnimation>();

        Next();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        _smileElapsedTime += Time.deltaTime;

        // 経過時間と比較して、過ぎていれば要素番号を増やす
        //Debug.Log($"{_moveInfos[_moveIndex].time},{_elapsedTime}");
        if (_moveInfos[_moveIndex].time <= _elapsedTime)
        {
            Next();
        }

        if (_smileTiming[_smileIndex] <= _smileElapsedTime)
        {
            _smileElapsedTime = 0;
            ++_smileIndex;

            _anim.PlaySmileAnimation();
            //Debug.Log("ミニニコ！");
        }

        _velocity = _moveInfos[_moveIndex].velocity;
        _currentMoveDirection = _moveInfos[_moveIndex].moveDirection;
        _currentLookDirection = _moveInfos[_moveIndex].lookDirection;
    }

    private void Next()
    {
        ++_moveIndex;
        _elapsedTime = 0;

        if (_moveInfos[_moveIndex].jump)
        {
            _rb.AddForce(Vector3.up * MiniCharaParams.JUMP_FORCE_LOW, ForceMode2D.Impulse);
            _anim.PlayJumpAnimation();

            ++_moveIndex;
            return;
        }

        _anim.SetMoveAnimation(_moveInfos[_moveIndex].moveDirection != 0);
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.BoxCast(_transform.position, _size, 0, Vector2.down, 0.05f, _mask);
        _isLanding = hit.collider != null && _rb.velocity.y <= 0;
        _anim.SetLanding(_isLanding);

        Vector2 moveDistance = new Vector2(_currentMoveDirection * _velocity * Time.fixedDeltaTime, 0);
        _rb.position += moveDistance;

        _transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, _currentLookDirection));
    }
}
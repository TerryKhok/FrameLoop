using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public static class MiniCharaParams
{
    public const float TO_LOST_VERTICAL_DISTANCE = 2.0f;
    public const float TO_MOVE_HORIZONTAL_DISTANCE = 1.3f;
    public const float TO_IDLE_HORIZONTAL_DISTANCE = 1.0f;
    public const float TO_WARP_WAIT_TIME = 1.2f;
    public const float TO_WARP_STUCK_TIME = 1.5f;

    public const float STUCK_RANGE = 0.2f;

    public const float MOVE_VELOCITY = 7.0f;
    public const float MOVE_VELOCITY_CROUCH = 5.0f;
    public const float MOVE_MIN_HORIZONTSL_DISTANCE = 0.3f;

    public const float MOVE_STOP_DISTANCE = 0.1f;

    public const float JUMP_FORCE_LOW = 7.0f;
    public const float JUMP_FORCE_MIDDLE = 10.0f;
    public const float JUMP_FORCE_HIGH = 13.0f;
}

public class MiniChara : MonoBehaviour
{
    [SerializeField]
    private GameObject _warpPrefab = null;
    [SerializeField]
    private GameObject _warpEnterParticle = null;
    [SerializeField]
    private GameObject _warpExitParticle = null;

    private StateMachineBase _stateMachine = null;

    private void Start()
    {
        Vector3 target = PlayerInfo.Instance.g_transform.position - PlayerInfo.Instance.g_transform.right * 0.5f;

        transform.position = target;

        _stateMachine = new MiniCharaStateMachine(transform);
        MiniCharaIdle.SetPlayer(PlayerInfo.Instance);
        MiniCharaMove.SetPlayer(PlayerInfo.Instance);
        MiniCharaFrame.SetFrame(FrameLoop.Instance.transform);
        MiniCharaLost.SetPlayer(PlayerInfo.Instance);
        MiniCharaWarp.SetWarpPrefab(_warpPrefab, _warpEnterParticle, _warpExitParticle);
    }

    private void Update()
    {
        _stateMachine.UpdateCurrentState();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdateCurrentState();
    }
}

public class MiniCharaStateMachine : StateMachineBase
{
    private Rigidbody2D _rigidbody;
    public Rigidbody2D rigidbody => _rigidbody;

    public Transform transform => _transform;

    private StateBase MakeIdleState()
    {
        return new MiniCharaIdle(this);
    }
    private StateBase MakeMoveState()
    {
        return new MiniCharaMove(this);
    }
    private StateBase MakeFrameState()
    {
        return new MiniCharaFrame(this);
    }
    private StateBase MakeUnframeState()
    {
        return new MiniCharaUnframe(this);
    }
    private StateBase MakeWarpState()
    {
        return new MiniCharaWarp(this);
    }
    private StateBase MakeLostState()
    {
        return new MiniCharaLost(this);
    }
    private StateBase MakeFroatState()
    {
        return new MiniCharaFroat(this);
    }

    public MiniCharaStateMachine(Transform transform) : base(transform)
    {
        _rigidbody = _transform.GetComponent<Rigidbody2D>();

        Make makeFunc = MakeIdleState;
        RegisterStates(makeFunc, "Idle");

        makeFunc = MakeMoveState;
        RegisterStates(makeFunc, "Move");

        makeFunc = MakeFrameState;
        RegisterStates(makeFunc, "Frame");

        makeFunc = MakeUnframeState;
        RegisterStates(makeFunc, "Unframe");

        makeFunc = MakeWarpState;
        RegisterStates(makeFunc, "Warp");

        makeFunc = MakeLostState;
        RegisterStates(makeFunc, "Lost");

        makeFunc = MakeFroatState;
        RegisterStates(makeFunc, "Froat");

        _currentState = new MiniCharaMove(this);
        _currentState.Enter();
    }

    ~MiniCharaStateMachine()
    {
        _currentState?.Exit();
    }

    public override void ChangeState(string stateName)
    {
        base.ChangeState(stateName);
    }
}

public class StateMachineBase
{
    protected delegate StateBase Make();

    protected StateBase _currentState = null;
    protected Transform _transform = null;
    private Dictionary<string, Make> _makeFunctionDictionary = new Dictionary<string, Make>();
    protected string _currentStateName = null, _prebStateName = null;
    public string PrebStateName => _prebStateName;

    public StateMachineBase(Transform transform)
    {
        _transform = transform;
    }

    protected void RegisterStates(Make makeFunc, string stateName)
    {
        _makeFunctionDictionary[stateName] = makeFunc;
    }

    virtual public void ChangeState(string stateName)
    {
        _currentState?.Exit();

        _prebStateName = _currentStateName;
        _currentStateName = stateName;
        _currentState = _makeFunctionDictionary[_currentStateName]();

        _currentState?.Enter();
    }

    virtual public void UpdateCurrentState()
    {
        Debug.Log(_currentState.ToString());

        if (FrameLoop.Instance.g_activeTrigger)
        {
            //Debug.Log("フレーム生成");
            MiniCharaWarp.SetWarpTarget(MiniCharaWarp.WarpTarget.Frame);
            ChangeState("Warp");
        }
        if(FrameLoop.Instance.g_resumeTrigger)
        {
            //Debug.Log("フレーム解除");
            MiniCharaWarp.SetWarpTarget(MiniCharaWarp.WarpTarget.Player);
            ChangeState("Warp");
        }
        _currentState?.Update();
    }

    virtual public void FixedUpdateCurrentState()
    {
        _currentState?.FixedUpdate();
    }
}

public class MiniCharaIdle : MiniCharaStateBase
{
    private static PlayerInfo _playerInfo = null;

    public static void SetPlayer(PlayerInfo _playerInfo)
    {
        MiniCharaIdle._playerInfo = _playerInfo;
    }

    public MiniCharaIdle(MiniCharaStateMachine stateMachine) : base(stateMachine)
    {

    }

    override public void Enter()
    {
        //_miniCharaStateMachine.transform.rotation = _playerInfo.g_transform.rotation;
        _miniCharaStateMachine.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, _playerInfo.g_lastInputX));
    }

    override public void Update()
    {
        Vector3 target = _playerInfo.g_transform.position;// - _playerInfo.g_transform.right * 0.5f;
        float horizontalDistance = target.x - _miniCharaStateMachine.transform.position.x;
        horizontalDistance = Mathf.Abs(horizontalDistance);

        //Debug.Log(distance);

        if (horizontalDistance > MiniCharaParams.TO_MOVE_HORIZONTAL_DISTANCE)
        {
            _miniCharaStateMachine.ChangeState("Move");
        }
    }

    override public void Exit()
    {
        //Debug.Log("IdleExit");
    }
}

public class MiniCharaMove : MiniCharaStateBase
{
    private static PlayerInfo _playerInfo = null;
    private bool _isLanding = true, _isHole = false, _isWall = false, _isOpen = false, _isOpen_high = false;
    private bool _isLost = false;
    private LayerMask _mask;
    private Vector2 _size = new Vector2(0.7f, 1);
    private float _stuckTime = 0;
    private Transform _transform = null;
    private float _elapsedTime = 0;
    private MiniCharaAnimation _animation;
    private float _wallDistance = 0;

    public static void SetPlayer(PlayerInfo playerInfo)
    {
        _playerInfo = playerInfo;
    }

    public MiniCharaMove(MiniCharaStateMachine stateMachine) : base(stateMachine)
    {

        //_mask |= 1 << LayerMask.NameToLayer("Outside");
    }

    override public void Enter()
    {
        //Debug.Log("MoveEnter");
        _mask = 1 << LayerMask.NameToLayer("OPlatform");
        _mask |= 1 << LayerMask.NameToLayer("OBox");
        _transform = _miniCharaStateMachine.transform;

        _animation = _transform.GetComponentInChildren<MiniCharaAnimation>();
        _animation.SetMoveAnimation(true);
    }

    public override void Update()
    {
        Vector3 target = _playerInfo.g_transform.position;// - _playerInfo.g_transform.right * 0.5f;
        float horizontalDistance = target.x - _miniCharaStateMachine.transform.position.x;
        horizontalDistance = Mathf.Abs(horizontalDistance);

        //Debug.Log($"横位置：{horizontalDistance}");
        float verticalDistance = target.y - _miniCharaStateMachine.transform.position.y;
        verticalDistance = Mathf.Abs(verticalDistance);

        if (horizontalDistance < MiniCharaParams.TO_IDLE_HORIZONTAL_DISTANCE)
        {
            //Debug.Log($"縦位置：{verticalDistance}");

            if (verticalDistance < MiniCharaParams.TO_LOST_VERTICAL_DISTANCE)
            {
                _miniCharaStateMachine.ChangeState("Idle");
            }
            else if (PlayerInfo.Instance.g_isGround)
            {
                _isLost = true;
            }
        }

        if(_stuckTime >= MiniCharaParams.TO_WARP_STUCK_TIME)
        {
            _miniCharaStateMachine.ChangeState("Warp");
        }


        if (verticalDistance < MiniCharaParams.TO_LOST_VERTICAL_DISTANCE)
        {
            _isLost = false;
            _elapsedTime = 0;
        }

        if (_isLost)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= MiniCharaParams.TO_WARP_WAIT_TIME)
            {
                _miniCharaStateMachine.ChangeState("Warp");
            }
        }
        else
        {
            _isLost = false;
        }
    }

    override public void FixedUpdate()
    {
        var currentVelocity = _miniCharaStateMachine.rigidbody.velocity;
        currentVelocity.y = 0;
        _miniCharaStateMachine.rigidbody.velocity -= currentVelocity;

        Vector3 currentPosition = _transform.position;

        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, _size, 0, Vector2.down, 0.05f, _mask);
        _isLanding = hit.collider != null && _miniCharaStateMachine.rigidbody.velocity.y <= 0;

        _animation.SetLanding(_isLanding);

        hit = Physics2D.Raycast(currentPosition + _transform.right, Vector2.down, 0.7f, _mask);
        _isHole = hit.collider == null;

        //Debug.DrawRay(origin + minichara.right, Vector2.down * 0.55f, Color.green, 0.1f);

        hit = Physics2D.Raycast(currentPosition, _transform.right, 0.6f, _mask);
        _isWall = hit.collider != null;
        _wallDistance = hit.distance;

        hit = Physics2D.Raycast(currentPosition + Vector3.up, _transform.right, 0.6f, _mask);
        _isOpen = hit.collider == null;

        hit = Physics2D.Raycast(currentPosition + Vector3.up * 2, _transform.right, 0.6f, _mask);
        _isOpen_high = hit.collider == null;

        Debug.DrawRay(currentPosition, _transform.right * 0.6f, Color.red, 0.1f);
        Debug.DrawRay(currentPosition + Vector3.up, _transform.right * 0.6f, Color.green, 0.1f);
        Debug.DrawRay(currentPosition + Vector3.up * 2, _transform.right * 0.6f, Color.blue, 0.1f);

        //Debug.Log($"着地：{_isLanding}、穴：{_isHole}、壁：{_isWall}、開いてる：{_isOpen}、ハイてる：{_isOpen_high}");

        Vector3 target = _playerInfo.g_transform.position;// - _playerInfo.g_transform.right * 0.5f;
        float distance = target.x - currentPosition.x;
        int direction = MathF.Sign(distance);

        if (Mathf.Abs(distance) >= MiniCharaParams.MOVE_MIN_HORIZONTSL_DISTANCE)
        {
            if (_wallDistance > MiniCharaParams.MOVE_STOP_DISTANCE || _isWall == false)
            {
                float velocity = MiniCharaParams.MOVE_VELOCITY;
                if (_playerInfo.g_isCrouch || _playerInfo.g_takeUpFg)
                {
                    velocity = MiniCharaParams.MOVE_VELOCITY_CROUCH;
                }

                Vector2 moveDistance = new Vector2(direction * velocity * Time.fixedDeltaTime, 0);
                _miniCharaStateMachine.rigidbody.position += moveDistance;

                _transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, direction));
            }
        }

        if (_isLanding)
        {
            if (_isHole || (_isOpen && _isWall))
            {
                _miniCharaStateMachine.rigidbody.AddForce(Vector3.up * MiniCharaParams.JUMP_FORCE_MIDDLE, ForceMode2D.Impulse);
                _animation.PlayJumpAnimation();
                AudioManager.instance.Stop("Chibi character walking");
                AudioManager.instance.Play("Chibi character jumping");
            }
            else if (_isOpen_high && _isWall)
            {
                _miniCharaStateMachine.rigidbody.AddForce(Vector3.up * MiniCharaParams.JUMP_FORCE_HIGH, ForceMode2D.Impulse);
                _animation.PlayJumpAnimation();
                AudioManager.instance.Stop("Chibi character walking");
                AudioManager.instance.Play("Chibi character jumping");
                //Debug.Log("ハイジャンプ");
            }
            //else
            //{
            //    _miniCharaStateMachine.rigidbody.AddForce(Vector3.up * MiniCharaParams.JUMP_FORCE_LOW, ForceMode2D.Impulse);
            //    //Debug.Log("ミニジャンプ");
            //}
        }

        if (_isWall)
        {
            _stuckTime += Time.fixedDeltaTime;
        }
        else
        {
            _stuckTime = 0;
        }
        //Debug.Log($"{_stuckTime}秒ハマってます:{gap}しか移動してません");
    }

    override public void Exit()
    {
        _animation.SetMoveAnimation(false);
        _animation.SetLanding(true);
    }
}

public class MiniCharaFrame : MiniCharaStateBase
{
    private static Transform _frame;
    private MiniCharaAnimation _animation;
    private VisualEffect _effect;
    private bool _isStop = false;

    public static void SetFrame(Transform frameTransform)
    {
        _frame = frameTransform;
    }

    public MiniCharaFrame(MiniCharaStateMachine stateMachine) : base(stateMachine)
    {

    }

    override public void Enter()
    {
        if (_frame == null)
        {
            _frame = FrameLoop.Instance.transform;
        }

        _miniCharaStateMachine.rigidbody.velocity = Vector2.zero;

        if (_miniCharaStateMachine.PrebStateName == "Warp")
        {
            _miniCharaStateMachine.rigidbody.AddForce(Vector3.up * MiniCharaParams.JUMP_FORCE_MIDDLE, ForceMode2D.Impulse);
        }

        _animation = _miniCharaStateMachine.transform.GetComponentInChildren<MiniCharaAnimation>();
        _effect = _miniCharaStateMachine.transform.GetComponentInChildren<VisualEffect>();
        _effect.enabled = true;

        _miniCharaStateMachine.transform.GetComponent<BoxCollider2D>().isTrigger = true;
        _miniCharaStateMachine.transform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 150;
    }

    override public void Update()
    {
        if (_isStop)
        {
            return;
        }

        Vector3 gap = _frame.transform.position - _miniCharaStateMachine.transform.position;

        if (Mathf.Abs(gap.y) < 0.1f && _miniCharaStateMachine.rigidbody.velocity.y < 0)
        {
            _miniCharaStateMachine.rigidbody.velocity = Vector3.zero;
            _miniCharaStateMachine.rigidbody.gravityScale = 0.0f;

            _isStop = true;
        }
    }

    public override void FixedUpdate()
    {
        //if (_isDone)
        //{
        //    return;
        //}

        //_miniCharaStateMachine.transform.position = _frame.position;
        //_isDone = true;

        //Vector3 setPos = (_miniCharaStateMachine.transform.position * 3.0f + _frame.transform.position) / 4.0f;

        //Vector3 distance = setPos - _miniCharaStateMachine.transform.position;

        //if(distance.magnitude < 0.05f)
        //{
        //    _isDone = true;
        //}

        //int direction = MathF.Sign(distance.x);

        //_miniCharaStateMachine.transform.position = setPos;
        //_miniCharaStateMachine.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, direction));
    }

    override public void Exit()
    {
        _animation.StopFrameAnimation();
        _effect.enabled = false;

        _miniCharaStateMachine.rigidbody.gravityScale = 1.0f;

        _miniCharaStateMachine.transform.GetComponent<BoxCollider2D>().isTrigger = false;
        _miniCharaStateMachine.transform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
    }
}

public class MiniCharaUnframe : MiniCharaStateBase
{
    private static PlayerInfo _playerInfo;

    public MiniCharaUnframe(MiniCharaStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        if (_playerInfo == null)
        {
            _playerInfo = PlayerInfo.Instance;
        }
    }

    public override void Update()
    {
        _miniCharaStateMachine.transform.position = _playerInfo.g_transform.position;
        _miniCharaStateMachine.ChangeState("Idle");

        //Vector3 target = _playerInfo.g_transform.position;// - _playerInfo.g_transform.right * 0.5f;
        //float distance = target.x - _miniCharaStateMachine.transform.position.x;
        //distance = Mathf.Abs(distance);

        //if (distance < 1.0f)
        //{
        //    _miniCharaStateMachine.ChangeState("Idle");
        //}
    }

    public override void FixedUpdate()
    {
        //Vector3 target = _playerInfo.g_transform.position;// - _playerInfo.g_transform.right * 0.5f;

        //Vector3 setPos = (_miniCharaStateMachine.transform.position * 3.0f + target) / 4.0f;

        //Vector2 gap = setPos - _miniCharaStateMachine.transform.position;
        //int direction = MathF.Sign(gap.x);

        //_miniCharaStateMachine.rigidbody.position += gap;
        //_miniCharaStateMachine.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, direction));
    }

    public override void Exit()
    {
        _miniCharaStateMachine.rigidbody.gravityScale = 1.0f;

        _miniCharaStateMachine.transform.GetComponent<BoxCollider2D>().isTrigger = false;
        _miniCharaStateMachine.transform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
    }
}

public class MiniCharaWarp : MiniCharaStateBase
{
    private static GameObject _warpPrefab = null, _warpEnterParticle = null, _warpExitParticle = null;
    private static WarpTarget _warpTarget = WarpTarget.Player;
    private Vector3 _warpPosition = Vector3.zero, _gatePosition = Vector3.zero;
    private bool _warped = false, _madeGate = false;

    public enum WarpTarget { Player, Frame }

    public static void SetWarpPrefab(GameObject warpPrefab, GameObject warpEnter, GameObject warpExit)
    {
        _warpPrefab = warpPrefab;
        _warpEnterParticle = warpEnter;
        _warpExitParticle = warpExit;
    }

    public static void SetWarpTarget(WarpTarget target)
    {
        _warpTarget = target;
    }

    public MiniCharaWarp(MiniCharaStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //Debug.Log("ワープしました");
        Transform minichara = _miniCharaStateMachine.transform;

        switch (_warpTarget)
        {
            case WarpTarget.Player:
                {
                    _gatePosition = minichara.position;
                    _gatePosition.y -= 1.0f;

                    break;
                }
            case WarpTarget.Frame:
                {
                    minichara.GetComponentInChildren<MiniCharaAnimation>().PlayFrameAnimation();
                    minichara.transform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 150;
                    _warpPosition = FrameLoop.Instance.transform.position;

                    LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

                    RaycastHit2D hit = Physics2D.Raycast(minichara.position, Vector3.zero, 0.1f, mask);
                    if(hit.collider !=  null)
                    {
                        //Debug.Log("toプカプカ");
                        _miniCharaStateMachine.ChangeState("Froat");
                    }

                    break;
                }
        }

        _miniCharaStateMachine.rigidbody.velocity = Vector2.zero;
        _miniCharaStateMachine.rigidbody.AddForce(Vector3.up * MiniCharaParams.JUMP_FORCE_MIDDLE, ForceMode2D.Impulse);
        minichara.transform.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public override void Update()
    {
        //Debug.Log("わーぷあっぷでーと");
        Transform minichara = _miniCharaStateMachine.transform;

        if(_madeGate == false)
        {
            if (_miniCharaStateMachine.rigidbody.velocity.y < 0)
            {
                _gatePosition = minichara.position;
                _gatePosition.y -= 1.0f;
                var warpgate = GameObject.Instantiate(_warpPrefab, _gatePosition, Quaternion.identity);
                GameObject.Destroy(warpgate, 0.5f);

                CoroutineHandler.StartStaticCoroutine(MakeParticle(_warpEnterParticle, _gatePosition, 1.5f));

                _madeGate = true;
            }
        }

        if (_madeGate && _warped == false)
        {
            Vector3 gateGap = minichara.position - _gatePosition;
            if (gateGap.y < -1.2f)
            {
                _warped = true;

                if(_warpTarget == WarpTarget.Player)
                {
                    _warpPosition = PlayerInfo.Instance.g_transform.position;
                    _warpPosition -= PlayerInfo.Instance.g_transform.right;

                    LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform") | 1 << LayerMask.NameToLayer("Obox");
                    Debug.DrawRay(_warpPosition, Vector3.forward * 10.0f, Color.red, 5.0f);
                    RaycastHit2D hit = Physics2D.Raycast(_warpPosition, Vector2.zero, 0.1f, mask);
                    if (hit.collider != null)
                    {
                        _warpPosition = PlayerInfo.Instance.g_transform.position;
                        _warpPosition += Vector3.up;
                        Debug.DrawRay(_warpPosition, Vector3.forward * 10.0f, Color.red, 5.0f);

                        hit = Physics2D.Raycast(_warpPosition, Vector2.zero, 0.1f, mask);
                        if (hit.collider != null)
                        {
                            _warpPosition = PlayerInfo.Instance.g_transform.position;
                        }
                    }
                }

                var warpgate = GameObject.Instantiate(_warpPrefab, _warpPosition, Quaternion.identity);
                GameObject.Destroy(warpgate, 0.5f);

                CoroutineHandler.StartStaticCoroutine(MakeParticle(_warpExitParticle, _warpPosition, 0.0f));

                minichara.position = _warpPosition;
                _miniCharaStateMachine.rigidbody.velocity = Vector2.zero;

                if (_warpTarget == WarpTarget.Frame)
                {
                    _miniCharaStateMachine.ChangeState("Frame");
                }
                else if (_warpTarget == WarpTarget.Player)
                {
                    _miniCharaStateMachine.rigidbody.AddForce(Vector3.up * MiniCharaParams.JUMP_FORCE_MIDDLE, ForceMode2D.Impulse);
                }
            }
        }

        if (_warpTarget == WarpTarget.Player && _warped)
        {
            Vector3 warpGap = _warpPosition - minichara.position;

            if (Mathf.Abs(warpGap.y) < 0.1f && _miniCharaStateMachine.rigidbody.velocity.y < 0)
            {
                _miniCharaStateMachine.ChangeState("Idle");
            }
            else
            {
                Vector3 currentPosition = minichara.position;
                currentPosition.y -= 0.5f;
                LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform");
                mask |= 1 << LayerMask.NameToLayer("OBox");

                RaycastHit2D hit = Physics2D.BoxCast(currentPosition, new Vector2(0.7f,1.0f), 0, Vector2.down, 0.03f, mask);
                bool isLanding = hit.collider != null && _miniCharaStateMachine.rigidbody.velocity.y <= 0;

                var direction = PlayerInfo.Instance.g_transform.position - minichara.position;
                Debug.Log(direction.y);

                if(isLanding && direction.y > 0)
                {
                    _miniCharaStateMachine.ChangeState("Idle");

                }
            }
        }
    }

    private IEnumerator MakeParticle(GameObject particle, Vector3 position, float gapY)
    {
        yield return new WaitForSeconds(0.2f);

        Vector3 gap = new Vector3(0.0f, gapY, 0.0f);
        Quaternion quaternion = Quaternion.LookRotation(Vector3.up);
        var instance = GameObject.Instantiate(particle, position + gap, quaternion);
        instance.GetComponent<ParticleSystemRenderer>().maskInteraction = SpriteMaskInteraction.None;
        GameObject.Destroy(instance, 0.5f);
    }

    public override void Exit()
    {
        _miniCharaStateMachine.transform.GetComponent<BoxCollider2D>().isTrigger = false;
    }
}

public class MiniCharaLost : MiniCharaStateBase
{
    private float _elapsedTime = 0;
    private static PlayerInfo _playerInfo;

    public MiniCharaLost(MiniCharaStateMachine miniCharaStateMachine) : base(miniCharaStateMachine)
    {
        _miniCharaStateMachine = miniCharaStateMachine;
    }

    public static void SetPlayer(PlayerInfo playerInfo)
    {
        _playerInfo = playerInfo;
    }

    public override void Update()
    {
        Vector3 target = _playerInfo.g_transform.position;// - _playerInfo.g_transform.right * 0.5f;
        float verticalDistance = target.y - _miniCharaStateMachine.transform.position.y;
        verticalDistance = Mathf.Abs(verticalDistance);

        //Debug.Log($"縦位置：{verticalDistance}");

        if (verticalDistance < MiniCharaParams.TO_LOST_VERTICAL_DISTANCE)
        {
            _miniCharaStateMachine.ChangeState("Idle");
        }

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= MiniCharaParams.TO_WARP_WAIT_TIME)
        {
            _miniCharaStateMachine.ChangeState("Warp");
        }
    }
}

public class MiniCharaFroat : MiniCharaStateBase
{
    private static Transform _frame;
    private Vector3 _target = Vector3.zero;

    public MiniCharaFroat(MiniCharaStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if(_frame == null)
        {
            _frame = FrameLoop.Instance.transform;
        }
        _target = _frame.position;
        //Debug.Log("プカプカ");
    }

    public override void FixedUpdate()
    {
        //Debug.Log("ぷかぷかあっぷでーと");
        Vector3 setPos = (_miniCharaStateMachine.transform.position * 2.0f + _target) / 3.0f;

        Vector2 gap = setPos - _miniCharaStateMachine.transform.position;
        int direction = MathF.Sign(gap.x);

        _miniCharaStateMachine.rigidbody.position += gap;
        _miniCharaStateMachine.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, direction));

        if(gap.sqrMagnitude < 0.003f)
        {
            _miniCharaStateMachine.ChangeState("Frame");
        }
    }
}

public class StateBase
{
    virtual public void Enter() { }
    virtual public void Update() { }
    virtual public void FixedUpdate() { }
    virtual public void Exit() { }
}

public class MiniCharaStateBase : StateBase
{
    protected MiniCharaStateMachine _miniCharaStateMachine = null;

    protected MiniCharaStateBase(MiniCharaStateMachine miniCharaStateMachine)
    {
        _miniCharaStateMachine = miniCharaStateMachine;
    }
}
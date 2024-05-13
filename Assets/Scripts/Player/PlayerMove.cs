using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤーの移動を行うクラス
public class PlayerMove : MonoBehaviour
{
    [SerializeField,Tooltip("プレイヤーの速度上限(m/s)")]
    private float _targetVelocity = 5.0f;

    [SerializeField, Tooltip("しゃがみ中の移動速度")]
    private float _crouchVelocity = 3.0f;

    private Rigidbody2D _rb = null;
    private Transform _transform;
    private Vector2 _currentInput = Vector2.zero;
    private PlayerInfo _playerInfo;
    private PlayerAnimation _playerAnimation;

    //box.csで使われている
    public bool _isMoving = false;

    private bool _se = false;

    private void Start()
    {
        //PlayerInfoクラスから変数を受け取る
        _playerInfo = PlayerInfo.Instance;
        _rb = _playerInfo.g_rb;
        _transform = _playerInfo.g_transform;

        _playerAnimation = PlayerAnimation.Instance;

        _playerAnimation.SetCrouchSpeed(_crouchVelocity / _targetVelocity);
    }

    private void Update()
    {
        //Debug.Log(_rb.velocity);
        _playerAnimation.SetMoveX((int)_currentInput.x);

        //Walkアニメーションを再生
        _playerAnimation.SetWalkAnimation(_isMoving && _playerInfo.g_isGround);

        _playerInfo.g_currentInputX = (int)_currentInput.x;

        if(_isMoving && _playerInfo.g_isGround)
        {
            if (!_se)
            {
                //足跡の音
                AudioManager.instance.Play("Walk");
                _se = true;
            }
        }
        else
        {
            if (_se)
            {
                //足跡の音
                AudioManager.instance.Stop("Walk");
                _se = false;
            }
        }
    }

    private void FixedUpdate()
    {
        move();
        rotate();
    }

    //InputSystemのコールバックを受け取るメソッド
    public void MovePerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("ismoving true");

        //WASD、LeftStick、Dpadの入力をVector2として受け取る
        var input = context.ReadValue<Vector2>();
        //Y軸の入力を無効化する
        _currentInput = Vector2.Scale(input, new Vector2(1, 0)).normalized;

        _isMoving = _currentInput.x != 0;
    }

    public void MoveCanceled(InputAction.CallbackContext context)
    {
        _isMoving = false;
        //Debug.Log("ismoving false");

        _currentInput = Vector2.zero;
    }

    private void rotate()
    {
        //入力が無ければリターン
        if(_currentInput == Vector2.zero) { return; }
        if (_playerInfo.g_takeUpFg) { return; }

        if(_currentInput.x < 0)
        {
            _transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            _transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void move()
    {
        if(_currentInput.x == _playerInfo.g_wall)
        {
            return;
        }

        var velocity = _targetVelocity;

        if(_playerInfo.g_takeUpFg || _playerInfo.g_isCrouch) 
        {
            velocity = _crouchVelocity;
        }

        //移動
        var currentPos = _rb.position;
        currentPos += _currentInput * velocity * Time.fixedDeltaTime;
        _rb.position = currentPos;
    }

    public void SetMove(int x)
    {
        _currentInput = new Vector2(x, 0);

        _isMoving = _currentInput.x != 0;
    }
}

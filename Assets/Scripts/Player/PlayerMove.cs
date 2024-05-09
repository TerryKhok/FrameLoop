using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤーの移動を行うクラス
public class PlayerMove : MonoBehaviour
{
    [SerializeField,Tooltip("プレイヤーの速度上限(m/s)")]
    private float _targetVelocity = 5.0f;
    private Rigidbody2D _rb = null;
    private Transform _transform;
    private Vector2 _currentInput = Vector2.zero;
    private PlayerInfo _playerInfo;
    private PlayerAnimation _playerAnimation;

    //box.csで使われている
    public bool _isMoving = false;

    private void Start()
    {
        //PlayerInfoクラスから変数を受け取る
        _playerInfo = PlayerInfo.Instance;
        _rb = _playerInfo.g_rb;
        _transform = _playerInfo.g_transform;

        _playerAnimation = PlayerAnimation.Instance;
    }

    private void Update()
    {
        //Debug.Log(_rb.velocity);

        //Walkアニメーションを再生
        _playerAnimation.SetWalkAnimation(_isMoving);
    }

    private void FixedUpdate()
    {
        move();
        rotate();
    }

    //InputSystemのコールバックを受け取るメソッド
    public void MovePerformed(InputAction.CallbackContext context)
    {
        //足跡の音
        AudioManager.instance.Play("Walk");
        _isMoving = true;
        Debug.Log("ismoving true");

        //WASD、LeftStick、Dpadの入力をVector2として受け取る
        var input = context.ReadValue<Vector2>();
        //Y軸の入力を無効化する
        _currentInput = Vector2.Scale(input, new Vector2(1, 0)).normalized;
    }

    public void MoveCanceled(InputAction.CallbackContext context)
    {
        //足跡の音
        AudioManager.instance.Stop("Walk");
        _isMoving = false;
        Debug.Log("ismoving false");

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

        //移動
        var currentPos = _rb.position;
        currentPos += _currentInput * _targetVelocity * Time.fixedDeltaTime;
        _rb.position = currentPos;
    }
}

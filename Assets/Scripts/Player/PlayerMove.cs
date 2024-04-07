using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤーの移動を行うクラス
public class PlayerMove : MonoBehaviour
{
    [SerializeField,Tooltip("プレイヤーの速度上限(m/s)")]
    private float _targetVelocity = 5.0f;
    [SerializeField,Tooltip("プレイヤーに加える加速度(m/s^2)")]
    private float _moveForce = 1.0f;

    private Rigidbody _rb = null;
    private Transform _transform;
    private Vector2 _currentInput = Vector2.zero;

    private void Start()
    {
        //PlayerInfoクラスから変数を受け取る
        _rb = PlayerInfo.Instance.g_rb;
        _transform = PlayerInfo.Instance.g_transform;
    }

    private void Update()
    {
        Debug.Log(_rb.velocity);
    }

    private void FixedUpdate()
    {
        move();
        rotate();
    }

    //InputSystemのコールバックを受け取るメソッド
    public void MoveInput(InputAction.CallbackContext context)
    {
        //WASD、LeftStick、Dpadの入力をVector2として受け取る
        var input = context.ReadValue<Vector2>();
        //Y軸の入力を無効化する
        _currentInput = Vector2.Scale(input, new Vector2(1, 0)).normalized;
    }

    private void rotate()
    {
        //入力が無ければリターン
        if(_currentInput == Vector2.zero) { return; }

        //移動方向を向かせる
        var rotate = Quaternion.LookRotation(_currentInput);
        _transform.rotation = rotate;
    }

    private void move()
    {
        //移動
        var currentPos = _rb.position;
        currentPos += (Vector3)_currentInput * _targetVelocity * Time.fixedDeltaTime;
        _rb.position = currentPos;

        //プレイヤーが等速になるように力を加える
        //var force = (_targetVelocity - Mathf.Abs(_rb.velocity.x)) * _moveForce;
        //_rb.AddForce(_currentInput*force, ForceMode.Acceleration);

        //入力が無くてプレイヤーが動いているor入力と移動方向が逆の時に速度を打ち消す
        if (_currentInput.x == 0
           || Mathf.Sign(_currentInput.x) != Mathf.Sign(_rb.velocity.x))
        {
            var currentVelocity = _rb.velocity;
            currentVelocity.y = 0;
            currentVelocity.x *= -1;

            //空中は力を弱める
            //if (!PlayerInfo.Instance.g_isGround) { currentVelocity.x *= 0.1f; }
            _rb.AddForce(currentVelocity, ForceMode.VelocityChange);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
        _rb = PlayerInfo.Instance.g_rb;
        _transform = PlayerInfo.Instance.g_transform;
    }

    private void FixedUpdate()
    {
        move();
        rotate();
    }

    public void MoveInput(InputAction.CallbackContext context)
    {
        _currentInput = context.ReadValue<Vector2>();
    }

    private void rotate()
    {
        if(_currentInput == Vector2.zero) { return; }

        var rotate = Quaternion.LookRotation(_currentInput);
        _transform.rotation = rotate;
    }

    private void move()
    {
        //プレイヤーが等速になるように力を加える
        var force = (_targetVelocity - Mathf.Abs(_rb.velocity.x)) * _moveForce;
        _rb.AddForce(_currentInput*force, ForceMode.Acceleration);

        //入力が無くてプレイヤーが動いているor入力と移動方向が逆の時に速度を打ち消す
        if(_currentInput.x == 0
           || Mathf.Sign(_currentInput.x) != Mathf.Sign(_rb.velocity.x))
        {
            var currentVelocity = _rb.velocity;
            currentVelocity.y = 0;
            currentVelocity.x *= -1;

            //空中は力を弱める
            if (!PlayerInfo.Instance.g_isGround) { currentVelocity.x *= 0.5f; }
            _rb.AddForce(currentVelocity, ForceMode.VelocityChange);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D _rb = null;
    private bool _jumpFg = false;
    [SerializeField]
    private float _jumpHeight = 2f;
    private float _jumpVelocity = 0;
    //private float _maxHeight = 0, _minHeight = 0;
    private void Start()
    {
        _rb = PlayerInfo.Instance.g_rb;
        _jumpVelocity = Mathf.Sqrt(2 * _jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        //_maxHeight = transform.position.y;
        //_minHeight = transform.position.y;
    }

    private void FixedUpdate()
    {
    //    if(_minHeight > transform.position.y)
    //    {
    //        _minHeight = transform.position.y;
    //    }

    //    if(_maxHeight < transform.position.y)
    //    {
    //        _maxHeight = transform.position.y;
    //    }
    //    Debug.Log($"Max{_maxHeight} Min{_minHeight} Offset{_maxHeight-_minHeight}");
        jump();
    }

    public void JumpStarted(InputAction.CallbackContext context)
    {
        _jumpFg = true;
    }
    
    public void JumpCanceled(InputAction.CallbackContext context)
    {
        _jumpFg = false;
    }

    private void jump()
    {
        if (!_jumpFg) { return; }
        //Debug.Log($"ƒWƒƒƒ“ƒv{_jumpFg}’…’n{PlayerInfo.Instance.g_isGround}");
        if (PlayerInfo.Instance.g_isGround)
        {
            var currentVelocity = _rb.velocity;
            currentVelocity.y = 0;
            _rb.velocity = currentVelocity;
            _rb.AddForce(Vector3.up * _jumpVelocity * _rb.mass, ForceMode2D.Impulse);
        }
        _jumpFg = false;
    }
}

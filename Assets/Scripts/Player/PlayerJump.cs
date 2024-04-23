using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D _rb = null;
    private bool _pressedJump = false, _releasedJump = false;
    [SerializeField,Tooltip("ジャンプの高さ")]
    private float _jumpHeightMax = 2f;
    [SerializeField,Tooltip("ジャンプの最低の高さ")]
    private float _jumpHeightMin = 1f;
    [SerializeField,Tooltip("ジャンプの速度")]
    private float _jumpVelocity = 5f;

    private float _minJumpTime = 0, _maxJumpTime = 0;
    private float _elapsedTime = 0;
    private float _gravityScale = 1;
    private bool _isJumping = false;

    private PlayerInfo _playerInfo = null;
    private void Start()
    {
        _playerInfo = PlayerInfo.Instance;
        _rb = _playerInfo.g_rb;
        _gravityScale = _rb.gravityScale;
        var jumpUnitHeight = (_jumpVelocity * _jumpVelocity) / (2 * Mathf.Abs(Physics2D.gravity.y) * _gravityScale);
        _minJumpTime = (_jumpHeightMin - jumpUnitHeight) / _jumpVelocity;
        _maxJumpTime = (_jumpHeightMax - jumpUnitHeight) / _jumpVelocity;
    }

    private void FixedUpdate()
    {
        if (_isJumping)
        {
            _elapsedTime += Time.fixedDeltaTime;
        }

        if (_pressedJump)
        {
            jump();
        }

        if(_releasedJump)
        {
            jumpCancel();
        }

        if(_elapsedTime >= _maxJumpTime)
        {
            _releasedJump = true;
        }
    }

    public void JumpStarted(InputAction.CallbackContext context)
    {
        _elapsedTime = 0;
        _pressedJump = true;
        _isJumping = true;
    }
    
    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (_isJumping)
        {
            _releasedJump = true;
        }
    }

    private void jump()
    {
        if (_playerInfo.g_isGround)
        {
            var currentVelocity = _rb.velocity;
            currentVelocity.y = 0;
            _rb.gravityScale = 0;
            _rb.velocity = currentVelocity;
            _rb.AddForce(Vector3.up * _jumpVelocity * _rb.mass, ForceMode2D.Impulse);
        }
        _pressedJump = false;
    }

    private void jumpCancel()
    {
        if(_elapsedTime <= _minJumpTime) { return; }
        _rb.gravityScale = _gravityScale;
        _elapsedTime = 0;
        _isJumping = false;
        _releasedJump = false;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerJump
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :プレイヤーのジャンプ
 *               ジャンプキーが押されている間は重力を無効にしてジャンプの高さを変更している
 *               
 *  Created     :2024/04/27
 */
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

    [SerializeField]
    private AudioManager _audioManager = null;

    private float _minJumpTime = 0, _maxJumpTime = 0;
    private float _elapsedTime = 0;
    private float _gravityScale = 1;
    private bool _isJumping = false;

    private PlayerInfo _playerInfo = null;
    private PlayerAnimation _playerAnimation = null;

    private void Start()
    {
        _playerInfo = PlayerInfo.Instance;
        _playerAnimation = PlayerAnimation.Instance;
        _rb = _playerInfo.g_rb;
        _gravityScale = _rb.gravityScale;

        //_jumpVelocityで真上に跳んだ時の高さを計算する
        var jumpUnitHeight = (_jumpVelocity * _jumpVelocity) / (2 * Mathf.Abs(Physics2D.gravity.y) * _gravityScale);

        //_jumpVelocityでジャンプの最低高度まで到達するのにかかる時間を計算する
        _minJumpTime = (_jumpHeightMin - jumpUnitHeight) / _jumpVelocity;
        //同じく最高高度までの時間を計算する
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

        //経過時間が最大時間より長かったらジャンプを終了
        if(_elapsedTime >= _maxJumpTime)
        {
            _releasedJump = true;
        }
    }

    //ジャンプキーを押したときの処理
    public void JumpStarted(InputAction.CallbackContext context)
    {
        _elapsedTime = 0;
        _pressedJump = true;
        _isJumping = true;
    }
    
    //ジャンプキーを離したときの処理
    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (_isJumping)
        {
            _releasedJump = true;
        }
    }

    //ジャンプ処理
    private void jump()
    {
        if (_playerInfo.g_isGround)
        {
            //Jumpアニメーションを再生
            _playerAnimation.PlayJumpAnimation();

            var currentVelocity = _rb.velocity;
            currentVelocity.y = 0;

            //重力を無効にして上に力を加える
            _rb.gravityScale = 0;
            _rb.velocity = currentVelocity;
            _rb.AddForce(Vector3.up * _jumpVelocity * _rb.mass, ForceMode2D.Impulse);

            if (_isJumping) { _audioManager.Play("Jump"); }
        }
        _pressedJump = false;
    }

    //ジャンプの終了処理
    private void jumpCancel()
    {
        //経過時間が最低時間より短かったらreturn
        if(_elapsedTime <= _minJumpTime) { return; }

        //重力を元に戻す
        _rb.gravityScale = _gravityScale;

        _elapsedTime = 0;
        _isJumping = false;
        _releasedJump = false;
    }
}

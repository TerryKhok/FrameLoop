using UnityEngine;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerJump
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�v���C���[�̃W�����v
 *               �W�����v�L�[��������Ă���Ԃ͏d�͂𖳌��ɂ��ăW�����v�̍�����ύX���Ă���
 *               
 *  Created     :2024/04/27
 */
public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D _rb = null;
    private bool _pressedJump = false, _releasedJump = false;
    [SerializeField,Tooltip("�W�����v�̍���")]
    private float _jumpHeightMax = 2f;
    [SerializeField,Tooltip("�W�����v�̍Œ�̍���")]
    private float _jumpHeightMin = 1f;
    [SerializeField,Tooltip("�W�����v�̑��x")]
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

        //_jumpVelocity�Ő^��ɒ��񂾎��̍������v�Z����
        var jumpUnitHeight = (_jumpVelocity * _jumpVelocity) / (2 * Mathf.Abs(Physics2D.gravity.y) * _gravityScale);

        //_jumpVelocity�ŃW�����v�̍Œፂ�x�܂œ��B����̂ɂ����鎞�Ԃ��v�Z����
        _minJumpTime = (_jumpHeightMin - jumpUnitHeight) / _jumpVelocity;
        //�������ō����x�܂ł̎��Ԃ��v�Z����
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

        //�o�ߎ��Ԃ��ő厞�Ԃ�蒷��������W�����v���I��
        if(_elapsedTime >= _maxJumpTime)
        {
            _releasedJump = true;
        }
    }

    //�W�����v�L�[���������Ƃ��̏���
    public void JumpStarted(InputAction.CallbackContext context)
    {
        _elapsedTime = 0;
        _pressedJump = true;
        _isJumping = true;
    }
    
    //�W�����v�L�[�𗣂����Ƃ��̏���
    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (_isJumping)
        {
            _releasedJump = true;
        }
    }

    //�W�����v����
    private void jump()
    {
        if (_playerInfo.g_isGround)
        {
            //Jump�A�j���[�V�������Đ�
            _playerAnimation.PlayJumpAnimation();

            var currentVelocity = _rb.velocity;
            currentVelocity.y = 0;

            //�d�͂𖳌��ɂ��ď�ɗ͂�������
            _rb.gravityScale = 0;
            _rb.velocity = currentVelocity;
            _rb.AddForce(Vector3.up * _jumpVelocity * _rb.mass, ForceMode2D.Impulse);

            if (_isJumping) { _audioManager.Play("Jump"); }
        }
        _pressedJump = false;
    }

    //�W�����v�̏I������
    private void jumpCancel()
    {
        //�o�ߎ��Ԃ��Œ᎞�Ԃ��Z��������return
        if(_elapsedTime <= _minJumpTime) { return; }

        //�d�͂����ɖ߂�
        _rb.gravityScale = _gravityScale;

        _elapsedTime = 0;
        _isJumping = false;
        _releasedJump = false;
    }
}

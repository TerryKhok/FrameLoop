using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

    //[SerializeField]
    //private AudioManager _audioManager = null;

    private float _minJumpTime = 0, _maxJumpTime = 0;
    private float _elapsedTime = 0;
    private float _gravityScale = 1;
    private bool _isJumping = false;

    private PlayerInfo _playerInfo = null;
    private PlayerAnimation _playerAnimation = null;
    private WaitForSeconds _wait = new WaitForSeconds(0.04f);

    private GameObject jumpDust = null; //vfx
    private GameObject landDust = null;
    private float animationTime = 0.4f; //�W�����v�ƃ����h�͈ꏏ
    private bool hasLanded = true;
    private bool playLandedAnim = false;

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

        jumpDust = GameObject.Find("VFX_Jump");
        landDust = GameObject.Find("VFX_Land");
    }

    public bool GetJumpBool()
    {
        //playermove�ɂ��g���Ă���
        Debug.Log("getjumpbool = " + _releasedJump);
        return _releasedJump;
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

        if(!hasLanded)
        {
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
            GameObject g = Instantiate(landDust, playerPos, Quaternion.identity);
            Destroy(g, animationTime);
            hasLanded = true;
        }
    }

    //�W�����v�L�[���������Ƃ��̏���
    public void JumpStarted(InputAction.CallbackContext context)
    {
        _pressedJump = true;
    }
    
    //�W�����v�L�[�𗣂����Ƃ��̏���
    public void JumpCanceled(InputAction.CallbackContext context)
    {
        if (_isJumping)
        {
            _releasedJump = true;
        }
    }

    private IEnumerator JumpDelay()
    {
        yield return _wait;

        _elapsedTime = 0;

        var currentVelocity = _rb.velocity;
        currentVelocity.y = 0;

        //�d�͂𖳌��ɂ��ď�ɗ͂�������
        _rb.gravityScale = 0;
        _rb.velocity = currentVelocity;
        _rb.AddForce(Vector3.up * _jumpVelocity * _rb.mass, ForceMode2D.Impulse);

        _isJumping = true;

        if (_isJumping) 
        {
            AudioManager.instance.Play("Jump");
        }
    }

    //�W�����v����
    private void jump()
    {
        if (_playerInfo.g_isGround)
        {
            //Jump�A�j���[�V�������Đ�
            _playerAnimation.PlayJumpAnimation();

            AudioManager.instance.Stop("Walk");

            StartCoroutine(JumpDelay());

            Vector2 playerPos = new Vector2(transform.position.x - 0.27f, transform.position.y - 0.3f);
            GameObject g = Instantiate(jumpDust, new Vector2(playerPos.x+.2f,playerPos.y-.5f), Quaternion.identity);
            Destroy(g, animationTime);
        }            
        _pressedJump = false;
    }

    //�W�����v�̏I������
    private void jumpCancel()
    {
        //�o�ߎ��Ԃ��Œ᎞�Ԃ��Z��������return
        if(_elapsedTime <= _minJumpTime) { return; }

        StopAllCoroutines();

        //�d�͂����ɖ߂�
        _rb.gravityScale = _gravityScale;

        _elapsedTime = 0;
        _isJumping = false;
        _releasedJump = false;

        //hasLanded = false;
    }

    public void SetJump()
    {
        _pressedJump = true;
    }
}

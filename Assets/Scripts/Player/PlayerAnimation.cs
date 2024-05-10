using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerAnimation
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Playerのアニメーションを再生する
 *               
 *  Created     :2024/05/03
 */
public class PlayerAnimation : SingletonMonoBehaviour<PlayerAnimation>
{
    private Animator _animator = null;
    private PlayerInfo _playerInfo = null;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerInfo = PlayerInfo.Instance;
    }

    public void PlayJumpAnimation()
    {
        _animator.SetTrigger("Jump");
    }

    public void SetWalkAnimation(bool isWalking)
    {
        _animator.SetBool("Walk", isWalking);
    }

    public void SetCrouchAnimation(bool isCrouching)
    {
        _animator.SetBool("Crouch", isCrouching);
    }

    public void SetHoldAnimation(bool isHolding)
    {
        _animator.SetBool("Hold", isHolding);
    }

    public void SetMoveX(int moveX)
    {
        _animator.SetInteger("MoveX", moveX);
    }

    private void Update()
    {
        _animator.SetBool("IsGround", _playerInfo.g_isGround);

        _animator.SetFloat("GroundDistance", _playerInfo.g_groundDistance);
    }
}

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
    private PlayerInfo _playerInfo = null;

    [SerializeField]
    private Animator _headAnim = null;
    [SerializeField]
    private Animator _bodyAnim = null;

    private void Start()
    {
        _playerInfo = PlayerInfo.Instance;
    }

    public void PlayJumpAnimation()
    {
        _bodyAnim.SetTrigger("Jump");
    }

    public void SetWalkAnimation(bool isWalking)
    {
        _bodyAnim.SetBool("Walk", isWalking);
        _headAnim.SetBool("Walk", isWalking);
    }

    public void SetCrouchAnimation(bool isCrouching)
    {
        if (PauseMenu.IsPaused) {  return; }

        _bodyAnim.SetBool("Crouch", isCrouching);
    }

    public void SetHoldAnimation(bool isHolding)
    {
        _bodyAnim.SetBool("Hold", isHolding);
    }

    public void PlayFrameAnimation()
    {
        _headAnim.SetTrigger("Frame");
    }

    public void SetMoveX(int moveX)
    {
        int direction = _playerInfo.g_boxDirection * moveX;

        if (direction == 0) { return; }

        _bodyAnim.SetInteger("MoveX", direction);
    }

    public void SetCrouchSpeed(float speed)
    {
        _bodyAnim.SetFloat("CrouchSpeed", speed);
    }

    private void LateUpdate()
    {
        _bodyAnim.SetBool("IsGround", _playerInfo.g_isGround);

        _bodyAnim.SetFloat("GroundDistance", _playerInfo.g_groundDistance);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerCrouch
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Playerをしゃがませる
 *               
 *  Created     :2024/04/27
 */
public class PlayerCrouch : MonoBehaviour
{
    private bool _isCrouching = false;
    private PlayerAnimation _playerAnimation = null;
    private PlayerInfo _playerInfo = null;

    private void Start()
    {
        _playerAnimation = PlayerAnimation.Instance;
        _playerInfo = PlayerInfo.Instance;
    }

    private void Update()
    {
        _playerAnimation.SetCrouchAnimation(_isCrouching);
        _playerInfo.g_isCrouch = _isCrouching;

        crouch();

        FrameLoop.Instance.SetCrouching(_isCrouching);
    }

    public void CrouchStarted(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        if (value.y > -0.85f)
        {
            return;
        }

        _isCrouching = true;
    }

    public void CrouchCanceled(InputAction.CallbackContext context)
    {
        _isCrouching = false;
    }

    private void crouch()
    {
        if (!_isCrouching) { return; }

        //しゃがんでいる間に行いたい処理
    }
}

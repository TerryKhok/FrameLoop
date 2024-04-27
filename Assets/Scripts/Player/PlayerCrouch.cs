using UnityEngine;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerCrouch
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Player�����Ⴊ�܂���
 *               
 *  Created     :2024/04/27
 */
public class PlayerCrouch : MonoBehaviour
{
    private bool _isCrouching = false;

    private void Update()
    {
        crouch();

        FrameLoop.Instance.SetCrouching(_isCrouching);
    }

    public void CrouchStarted(InputAction.CallbackContext context)
    {
        _isCrouching = true;
    }

    public void CrouchCanceled(InputAction.CallbackContext context)
    {
        _isCrouching = false;
    }

    private void crouch()
    {
        if (!_isCrouching) { return; }

        //���Ⴊ��ł���Ԃɍs����������
    }
}

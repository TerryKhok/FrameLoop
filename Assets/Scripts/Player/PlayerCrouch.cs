using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : MonoBehaviour
{
    private bool _isCrouching = false;

    private void Update()
    {
        crouch();
        FrameLoop.Instance.SetCrouching(_isCrouching);
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        _isCrouching = context.performed;
    }

    private void crouch()
    {
        if (!_isCrouching) { return; }
    }
}

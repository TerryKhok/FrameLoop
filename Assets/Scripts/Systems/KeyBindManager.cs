using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

public class KeyBindManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    private GameObject _mask;
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private void Start()
    {
        _mask = GameObject.FindGameObjectWithTag("Mask");
        _mask.SetActive(false);
        _playerInput = GetComponent<PlayerInput>();
    }

    // ���o�C���h���J�n���郁�\�b�h
    public void StartRebinding(string actionName, int index)
    {
        _mask.SetActive(true);

        // �A�N�V�������擾
        InputAction action = _playerInput.actions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"{actionName}��������܂���");
            return;
        }

        action.Disable();

        // ���o�C���h�̐ݒ�
        _rebindingOperation = action.PerformInteractiveRebinding(index)
            .WithControlsExcluding("<Keyboard>/escape")
            .WithExpectedControlType("Button")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                // ���o�C���h������������A�N�V������L����
                action.Enable();
                operation.Dispose();
                _mask.SetActive(false);
            })
            .Start();
    }

    // �f�t�H���g�̃��o�C���h���폜���郁�\�b�h
    public void RemoveBindingOverrides(string actionName)
    {
        InputAction action = _playerInput.actions.FindAction(actionName);
        if (action != null)
        {
            action.RemoveAllBindingOverrides();
        }
    }

    public PlayerInput GetPlayerInput()
    {
        return _playerInput;
    }
}
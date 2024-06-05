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

    // リバインドを開始するメソッド
    public void StartRebinding(string actionName, int index)
    {
        _mask.SetActive(true);

        // アクションを取得
        InputAction action = _playerInput.actions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"{actionName}が見つかりません");
            return;
        }

        action.Disable();

        // リバインドの設定
        _rebindingOperation = action.PerformInteractiveRebinding(index)
            .WithControlsExcluding("<Keyboard>/escape")
            .WithExpectedControlType("Button")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                // リバインドが完了したらアクションを有効化
                action.Enable();
                operation.Dispose();
                _mask.SetActive(false);
            })
            .Start();
    }

    // デフォルトのリバインドを削除するメソッド
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
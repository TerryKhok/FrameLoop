using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using System.Linq;

public class KeyBindManager : MonoBehaviour
{
#if false
    private PlayerInput _playerInput;
    private GameObject _mask;
    private InputAction _action;
    private TextMeshProUGUI _setText;
    private bool isWaitingForInput = false;
    private int bindingIndexToOverride = -1;

    void Awake()
    {
        _mask = GameObject.FindGameObjectWithTag("Mask");
        _mask.SetActive(false);
        _playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (isWaitingForInput)
        {
            // キーボード入力をキャプチャ
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                foreach (var keyControl in keyboard.allKeys)
                {
                    if (keyControl.wasPressedThisFrame)
                    {
                        string controlPath = keyControl.path;
                        ApplyBindingOverride(controlPath);
                        isWaitingForInput = false;
                        return;
                    }
                }
            }

            // ゲームパッド入力をキャプチャ
            var gamepads = Gamepad.all;
            foreach (var gamepad in gamepads)
            {
                foreach (var control in gamepad.allControls)
                {
                    if (control is ButtonControl buttonControl && buttonControl.wasPressedThisFrame)
                    {
                        string controlPath = control.path;
                        ApplyBindingOverride(controlPath);
                        isWaitingForInput = false;
                        return;
                    }
                }
            }

            // マウス入力をキャプチャ
            var mouse = Mouse.current;
            if (mouse != null)
            {
                foreach (var buttonControl in mouse.allControls)
                {
                    if (buttonControl is ButtonControl mouseButton && mouseButton.wasPressedThisFrame)
                    {
                        string controlPath = mouseButton.path;
                        ApplyBindingOverride(controlPath);
                        isWaitingForInput = false;
                        return;
                    }
                }
            }
        }
    }

    public void StartListeningForInput(int index, string acitonName, TextMeshProUGUI text)
    {
        bindingIndexToOverride = index;
        _action = _playerInput.actions.FindAction(acitonName);

        _setText = text;

        isWaitingForInput = true;

        _mask.SetActive(true);
    }

    private void ApplyBindingOverride(string controlPath)
    {
        if (bindingIndexToOverride >= 0)
        {
            _action.ApplyBindingOverride(bindingIndexToOverride, controlPath);
            Debug.Log($"Binding overridden to: {controlPath}");
        }

        _setText.text = _action.bindings[bindingIndexToOverride].ToDisplayString().ToUpper();

        _mask.SetActive(false);
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
#endif
}
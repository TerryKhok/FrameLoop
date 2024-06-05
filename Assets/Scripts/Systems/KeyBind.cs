using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBind : MonoBehaviour
{
    private KeyBindManager _manager = null;

    [SerializeField]
    private string _actionName;
    private InputAction _action;
    [SerializeField]
    private TextMeshProUGUI _binding;
    [SerializeField]
    private int _index = 0;

    private void Start()
    {
        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<KeyBindManager>();
        _action = _manager.GetPlayerInput().actions.FindAction(_actionName);
        _binding.text = _action.bindings[0].ToDisplayString();
    }

    private void Update()
    {
        _binding.text = _action.GetBindingDisplayString(_index, InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    // UIボタンから呼び出されるメソッド
    public void OnChangeKeyButtonPressed()
    {
        _manager.StartRebinding(_actionName,_index);
    }

    // デフォルトのリバインドを削除するボタン
    public void OnResetKeyButtonPressed()
    {
        _manager.RemoveBindingOverrides(_actionName);
    }
}

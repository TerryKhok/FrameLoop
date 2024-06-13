using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :KeyBind
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :キーバインドの設定
 *               
 *  Created     :2024/06/13
 */
public class KeyBind : MonoBehaviour
{
    private KeyBindManager _manager = null;

    [SerializeField]
    private string _actionName;
    private InputAction _action;
    [SerializeField]
    private string _compositeBindingName;
    [SerializeField]
    private string _bindingName;
    [SerializeField]
    private TextMeshProUGUI _bindingText;
    [SerializeField]
    private int _index = 0;

    private void Start()
    {
        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<KeyBindManager>();
        _action = _manager.GetPlayerInput().actions.FindAction(_actionName);

        if (_compositeBindingName != "")
        {
            bool partOfComposite = false;
            for (int i = 0; i < _action.bindings.Count; i++)
            {
                if (_action.bindings[i].name == _compositeBindingName)
                {
                    partOfComposite = true;
                }

                if (partOfComposite && _action.bindings[i].name == _bindingName)
                {

                    _index = i;
                    break;
                }
            }
        }

        _bindingText.text = _action.bindings[_index].ToDisplayString().ToUpper();
    }

    // UIボタンから呼び出されるメソッド
    public void OnChangeKeyButtonPressed()
    {
        _manager.StartListeningForInput(_index, _actionName, _bindingText);
    }

    // デフォルトのリバインドを削除するボタン
    public void OnResetKeyButtonPressed()
    {
        _manager.RemoveBindingOverrides(_actionName);
    }
}

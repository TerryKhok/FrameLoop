using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :GamepadUIManager
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :GamepadUISelect���Ǘ�����
 *               
 *  Created     :2024/06/13
 */
public class GamepadUIManager : MonoBehaviour
{
    private EventSystem _eventSystem;
    private PlayerInput _playerInput;

    private List<bool> _enableList = new List<bool>();
    private GameObject _selectObject = null;

    private void Start()
    {
        _eventSystem = GetComponent<EventSystem>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void LateUpdate()
    {
        Debug.Log(_eventSystem.currentSelectedGameObject);
        Debug.Log(_playerInput.currentControlScheme);
        Select();
        _selectObject = null;
    }

    private void Select()
    {
        if (_playerInput.currentControlScheme == "Gamepad")
        {
            if (_eventSystem.currentSelectedGameObject == null)
            {
                //���I���Ȃ�ݒ肷��
                _eventSystem.SetSelectedGameObject(_selectObject);
            }
            else if(_selectObject == null)
            {
                Debug.Log("null");
                _eventSystem.SetSelectedGameObject(null);
            }

        }
        else
        {
            Debug.Log("gamepadNull");
            _eventSystem.SetSelectedGameObject(null);
        }
    }

    public void SetSelectObject(GameObject selectObject)
    {
        _selectObject = selectObject;
    }
}

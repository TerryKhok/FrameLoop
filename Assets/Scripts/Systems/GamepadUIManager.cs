using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :GamepadUIManager
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :GamepadUISelectÇä«óùÇ∑ÇÈ
 *               
 *  Created     :2024/06/13
 */
public class GamepadUIManager : MonoBehaviour
{
    private EventSystem _eventSystem;
    private PlayerInput _playerInput;

    private GameObject _selectObject = null;

    private void Start()
    {
        _eventSystem = GetComponent<EventSystem>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void LateUpdate()
    {
        Select();
        _selectObject = null;
    }

    private void Select()
    {
        if (_playerInput.currentControlScheme == "Gamepad")
        {
            if (_eventSystem.currentSelectedGameObject == null)
            {
                //ñ¢ëIëÇ»ÇÁê›íËÇ∑ÇÈ
                _eventSystem.SetSelectedGameObject(_selectObject);
            }
            else if(_selectObject == null)
            {
                _eventSystem.SetSelectedGameObject(null);
            }

        }
        else
        {
            _eventSystem.SetSelectedGameObject(null);
        }
    }

    public void SetSelectObject(GameObject selectObject)
    {
        if(selectObject == null)
        {
            _eventSystem.SetSelectedGameObject(null);
            return;
        }
        _selectObject = selectObject;
    }
}

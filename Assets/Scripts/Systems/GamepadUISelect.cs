using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GamepadUISelect : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelect;
    private EventSystem _eventSystem;

    private void Start()
    {
        _eventSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EventSystem>();
    }

    private void Update()
    {
        if (Gamepad.current != null)
        {
            _eventSystem.SetSelectedGameObject(_firstSelect);
        }
        else
        {
            _eventSystem.SetSelectedGameObject(null);
        }
    }
}

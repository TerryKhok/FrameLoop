using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :GamepadUISelect
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :‚Ç‚ÌUI‚ð‚Í‚¶‚ß‚É‘I‘ð‚·‚é‚©‚ðŠÇ—
 *               
 *  Created     :2024/06/13
 */
public class GamepadUISelect : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelect;
    private GamepadUIManager _gamepadUIManager;
    [SerializeField]
    private bool _enableOnAwake = false;

    private bool _enable = false;

    public bool Enable => _enable;

    public GameObject FirstSelect
    {
        set => _firstSelect = value;
        get => _firstSelect;
    }

    private void Start()
    {
        _gamepadUIManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GamepadUIManager>();
        _enable |= _enableOnAwake;
    }

    private void Update()
    {
        if (_enable)
        {
            _gamepadUIManager.SetSelectObject(_firstSelect);
        }
    }

    public void SetEnable(bool enable)
    {
        _enable = enable;
    }

    public void SetSelectEmpty()
    {
        _gamepadUIManager.SetSelectObject(null);
    }
}

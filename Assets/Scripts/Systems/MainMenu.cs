using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :MainMenu
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :メインメニューの表示切替
 *               
 *  Created     :2024/06/13
 */
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainMenu;
    [SerializeField]
    private GamepadUISelect _gamepadUISelect;
    [SerializeField]
    private GameObject _selectUIObject;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetEnable(bool enable)
    {
        if (_mainMenu != null)
        {
            _mainMenu.SetActive(enable);
            _gamepadUISelect.SetEnable(enable);
        }
    }

    public void StartFromBegining()
    {
        _selectUIObject.SetActive(true);
    }

    public void ModeSelect(bool isChallenge)
    {
        Goal.IsChallenge = isChallenge;
        SceneLoader.Instance.LoadScene("lvl 1");
        _selectUIObject.SetActive(false);
    }
}

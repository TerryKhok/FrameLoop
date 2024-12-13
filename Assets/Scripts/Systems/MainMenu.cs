using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private GamepadUISelect _gamepadUISelect_main;
    [SerializeField]
    private GameObject _selectUIObject;
    [SerializeField]
    private GameObject _firstSelectObj;

    [SerializeField]
    private GameObject transitionObject;
    [SerializeField]
    private float transitionTime;

    private Animator fadeAnimator;
    private bool isTransition = false;
    private float elapsedTime = 0f;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        fadeAnimator = transitionObject.GetComponent<Animator>();
        isTransition = false;
        fadeAnimator.Play("fade in");
    }

    public void SetEnable(bool enable)
    {
        if (_mainMenu != null)
        {
            _mainMenu.SetActive(enable);
            _gamepadUISelect_main.SetEnable(enable);
        }
    }

    public void StartFromBegining()
    {
        _selectUIObject.SetActive(true);

        _gamepadUISelect_main.SetSelectEmpty();

        _gamepadUISelect_main.FirstSelect = _firstSelectObj;
    }

    public void ModeSelect(bool isChallenge)
    {
        Goal.IsChallenge = isChallenge;
        _selectUIObject.SetActive(false);

        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        fadeAnimator.Play("fade out");
        isTransition = true;

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = fadeAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("fade out") && stateInfo.normalizedTime >= 1.0f;
        });

        SceneManager.LoadScene("OpeningNew");
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }
}

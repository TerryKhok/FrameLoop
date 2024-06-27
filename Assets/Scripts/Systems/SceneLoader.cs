using System;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    [SerializeField] PauseMenu _pauseMenu;
    private InputManager _inputManager;
    private CircleWipeController _circleWipeController;

    private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

    private void Start()
    {
        _inputManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();
        _inputManager._Retry.performed += Retry;
        _circleWipeController = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<CircleWipeController>();
    }

    //[SerializeField]
    //AudioManager _audioManager = null;

    public void LoadScene(string sceneName)
    {
        // FindObjectOfType<AudioManager>().Play("ButtonPressed");
        // FindObjectOfType<AudioManager>().Stop("BGM2");
        // FindObjectOfType<AudioManager>().Play("BGM1");
        //if (sceneName != "StageSelection")
        //{
        //    _audioManager.Play("Main BGM");
        //    DontDestroyOnLoad(_audioManager);
        //}
        StartCoroutine(LoadSceneAsync(sceneName));
        //Debug.Log("Scene loaded");
        if (_pauseMenu != null) _pauseMenu.SetPause(false);
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneAsync(index));
        //Debug.Log("Scene loaded");
        if (_pauseMenu != null) _pauseMenu.SetPause(false);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        float duration = 0, elapsedTime = 0;

        if (_circleWipeController != null)
        {
            duration = _circleWipeController.GetDuration();
            _circleWipeController.BeginTransition(true);
        }

        var async = SceneManager.LoadSceneAsync(sceneName);

        async.allowSceneActivation = false;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        float duration = 0, elapsedTime = 0;

        if (_circleWipeController != null)
        {
            duration = _circleWipeController.GetDuration();
            _circleWipeController.BeginTransition(true);
        }

        var async = SceneManager.LoadSceneAsync(index);

        async.allowSceneActivation = false;
        
        while(elapsedTime <= duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        async.allowSceneActivation = true;

        while(!async.isDone)
        {
            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    public void ContinueGame()
    {
        //Read Save and load Stage from last session
    }

    public void OpenSettings()
    {

    }

    private void OnDestroy()
    {
        _inputManager._Retry.performed -= Retry;
    }

    public void Retry(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        // FindObjectOfType<AudioManager>().Play("ButtonPressed");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

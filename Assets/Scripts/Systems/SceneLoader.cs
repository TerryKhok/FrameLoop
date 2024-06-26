using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    [SerializeField] PauseMenu _pauseMenu;
    private InputManager _inputManager;
    private CircleWipeController _circleWipeController;

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
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsync(sceneName));
        //Debug.Log("Scene loaded");
        if (_pauseMenu != null) _pauseMenu.SetPause(false);
    }

    public void LoadScene(int index)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsync(index));
        //Debug.Log("Scene loaded");
        if (_pauseMenu != null) _pauseMenu.SetPause(false);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        float duration = 0;

        if (_circleWipeController != null)
        {
            duration = _circleWipeController.GetDuration();
            _circleWipeController.BeginTransition(true);
        }

        var async = SceneManager.LoadSceneAsync(sceneName);

        async.allowSceneActivation = false;
        yield return new WaitForSeconds(duration);
        async.allowSceneActivation = true;
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        float duration = 0;

        if (_circleWipeController != null)
        {
            duration = _circleWipeController.GetDuration();
            _circleWipeController.BeginTransition(true);
        }

        var async = SceneManager.LoadSceneAsync(index);

        async.allowSceneActivation = false;
        yield return new WaitForSeconds(duration);
        async.allowSceneActivation = true;
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

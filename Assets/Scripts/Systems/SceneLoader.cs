using System;
using System.Collections;
using System.Net.Http.Headers;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    [SerializeField] PauseMenu _pauseMenu;
    [SerializeField]
    private bool _playable = true;
    private InputManager _inputManager;
    private CircleWipeController _circleWipeController;
    private float _progress = 0;

    private bool retryAnimStart = false;
    private static Coroutine _coroutine;
    private bool _isLoading = false;
    public bool IsSceneLoading
    {
        get { return (_coroutine != null || _isLoading); }
    }

    private string _sceneName;

    private void Start()
    {
        Time.timeScale = 1.0f;
        _inputManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();

        _sceneName = SceneManager.GetActiveScene().name;

        if (_playable)
        {
            if (!(_sceneName == "World1" || _sceneName == "World2" || _sceneName == "World3" || _sceneName == "World4" || _sceneName == "WorldSelect"))
            {
                _inputManager._Retry.performed += Retry;
                _inputManager._Next.performed += Next;
            }
            else if (_sceneName != "lvl 27")
            {
                _inputManager._Retry.performed += Retry;
            }

        }
        else
        {
            _inputManager._Retry.performed += Skip;
        }
        _circleWipeController = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<CircleWipeController>();
        retryAnimStart = false;
        _coroutine = null;
    }

    private void Update()
    {
        if (_sceneName == "World1" || _sceneName == "World2" || _sceneName == "World3" || _sceneName == "World4" || _sceneName == "WorldSelect")
        {
            return;
        }

        _progress = _inputManager._Retry.GetTimeoutCompletionPercentage();
        var temp = _inputManager._Next.GetTimeoutCompletionPercentage();
        if (_progress < temp)
        {
            if (_sceneName != "lvl 27")
            {
                _progress = temp;
            }
        }
        _circleWipeController.SetProgress(_progress);
        if(_progress > 0)
        {
            if(!retryAnimStart)
            {
                AudioManager.instance.Play("StageResetSlow");
                retryAnimStart = true;
            }
        }
        else
        {
            AudioManager.instance.Stop("StageResetSlow");
            retryAnimStart = false;
        }
    }

    //[SerializeField]
    //AudioManager _audioManager = null;

    public void LoadScene(string sceneName)
    {
        if(_coroutine != null) 
        {
            return;
        }

        // FindObjectOfType<AudioManager>().Play("ButtonPressed");
        // FindObjectOfType<AudioManager>().Stop("BGM2");
        // FindObjectOfType<AudioManager>().Play("BGM1");
        //if (sceneName != "StageSelection")
        //{
        //    _audioManager.Play("Main BGM");
        //    DontDestroyOnLoad(_audioManager);
        //}
        _coroutine = StartCoroutine(LoadSceneAsync(sceneName));
        //Debug.Log("Scene loaded");
        if (_pauseMenu != null) _pauseMenu.SetPause(false);
    }

    public void LoadScene(int index)
    {
        if (_coroutine != null)
        {
            return;
        }

        _coroutine = StartCoroutine(LoadSceneAsync(index));
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
        if (_playable)
        {

            if (!( _sceneName == "World1" || _sceneName == "World2" || _sceneName == "World3" || _sceneName == "World4" || _sceneName == "WorldSelect"))
            {
                _inputManager._Retry.performed -= Retry;
                _inputManager._Next.performed -= Next;
            }
            else if(_sceneName != "lvl 27")
            {
                _inputManager._Retry.performed -= Retry;
            }
        }
        else
        {
            _inputManager._Retry.performed -= Skip;
        }
    }

    public void Retry(InputAction.CallbackContext context)
    {
        _isLoading = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Next(InputAction.CallbackContext context)
    {
        _isLoading = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Skip(InputAction.CallbackContext context)
    {
        _isLoading = true;

        if(SceneManager.GetActiveScene().name == ("OpeningNew"))
        {
            SceneManager.LoadScene("lvl 1");
        }
        else
        {
            SceneManager.LoadScene("AppreciateScene");
        }
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

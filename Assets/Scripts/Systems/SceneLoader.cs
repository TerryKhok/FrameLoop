using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    [SerializeField] PauseMenu _pauseMenu;
    private InputManager _inputManager;

    private void Start()
    {
        _inputManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InputManager>();
        _inputManager._Retry.performed += Retry;
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
        SceneManager.LoadScene(sceneName);
        //Debug.Log("Scene loaded");
        if (_pauseMenu != null) _pauseMenu.SetPause(false);
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

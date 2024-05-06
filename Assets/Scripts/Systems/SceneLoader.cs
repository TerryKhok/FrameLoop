using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] PauseMenu _pauseMenu;

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

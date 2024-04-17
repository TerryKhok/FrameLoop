using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] public static bool IsPaused = false;

    public GameObject pauseMenuUI;

    private void Start()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    void Pause()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Retry()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");
        Time.timeScale = 1f;
        IsPaused = false;
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetPause(bool isPaused){
        IsPaused = isPaused;
    }
}

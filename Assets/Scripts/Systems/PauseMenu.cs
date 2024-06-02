using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : SingletonMonoBehaviour<PauseMenu>
{
    [SerializeField] public static bool IsPaused = false;

    public GameObject pauseMenuUI;

    private PlayerInput playerInput;

    private void Start()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();
    }

    public void OnPause(InputAction.CallbackContext context)
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

    public void Resume()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");

        playerInput.SwitchCurrentActionMap("Player");

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    void Pause()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");

        playerInput.SwitchCurrentActionMap("UI");

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

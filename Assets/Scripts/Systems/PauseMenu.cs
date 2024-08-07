using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : SingletonMonoBehaviour<PauseMenu>
{
    [SerializeField] public static bool IsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionMenuUI;

    [SerializeField] private GamepadUISelect gamepadUISelect = null;

    private PlayerInput playerInput;

    [SerializeField]
    private AudioMixerSnapshot paused, unpaused;

    private void Start()
    {
        unpaused.TransitionTo(0f);

        IsPaused = false;
        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if(IsPaused)
        {
            paused.TransitionTo(0f);
        }
        else
        {
            unpaused.TransitionTo(0f);
        }
    }

    //public void OnPause(InputAction.CallbackContext context)
    //{
    //    if (IsPaused)
    //    {
    //        Resume();
    //    }
    //    else
    //    {
    //        Pause();
    //    }
    //}

    public void OnResume(InputAction.CallbackContext context)
    {
        Resume();
    }

    private void Resume()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");

        if (Goal.instance != null)
        {
            // クリア済みなら実行しない
            if (Goal.instance.g_clear)
            {
                return;
            }
        }

        playerInput.SwitchCurrentActionMap("Player");

        if(pauseMenuUI == null) { return; }

        gamepadUISelect.SetEnable(false);
        pauseMenuUI.SetActive(false);
        optionMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Pause();
    }

    private void Pause()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");

        if(Goal.instance != null)
        {
            // クリア済みならポーズを実行しない
            if(Goal.instance.g_clear)
            {
                return;
            }
        }

        playerInput.SwitchCurrentActionMap("UI");

        gamepadUISelect.SetEnable(true);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Retry()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonPressed");
        Time.timeScale = 1f;
        IsPaused = false;
        Resume();
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetPause(bool isPaused){
        IsPaused = isPaused;
    }

    public void SetEnable(bool enable)
    {
        pauseMenuUI.SetActive(enable);
    }
}

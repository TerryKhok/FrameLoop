using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

/*  ProjectName :FrameLoop
 *  ClassName   :OptionMenu
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :ÉIÉvÉVÉáÉìÇÃUIä«óùÇ∆âπó ê›íË
 *               
 *  Created     :2024/06/13
 */

public class OptionMenu : MonoBehaviour
{
    [SerializeField]
    private GamepadUISelect _gamepadUISelect;
    [SerializeField]
    private GameObject OptionUI;
    [SerializeField]
    private GameObject _firstSelect;
    [SerializeField]
    private GameObject _returnSelect;
    [SerializeField]
    private AudioMixer _audioMixer;
    [SerializeField]
    private TextMeshProUGUI _masterText;
    [SerializeField]
    private Slider _masterSlider;
    [SerializeField]
    private TextMeshProUGUI _seText;
    [SerializeField]
    private Slider _seSlider;
    [SerializeField]
    private TextMeshProUGUI _bgmText;
    [SerializeField]
    private Slider _bgmSlider;

    private const int MASTER_BASE = 0, SE_BASE = -1, BGM_BASE = -12;
    private const int PAUSED_MASTER_BASE = -6, PAUSED_SE_BASE = -7, PAUSED_BGM_BASE = -18;
    private int masterBase = 0, seBase = 0, bgmBase = 0;
    private static bool _isInit = false;

#if false
    [SerializeField]
    private GameObject _keyboardRebind;
    [SerializeField]
    private GameObject _gamepadRebind;
#endif

    private EventSystem _eventSystem;

    private void Start()
    {
        OptionUI.SetActive(false);

        _eventSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EventSystem>();
        //if (!_isInit)
        //{
        //    SetMasterVolume(5);
        //    SetSEVolume(5);
        //    SetBGMVolume(5);

        //    _isInit = true;
        //}

        //masterBase = MASTER_BASE;
        //seBase = SE_BASE;
        //bgmBase = BGM_BASE;

        //GetMasterVolume();
        //GetSEVolume();
        //GetBGMVolume();

    }
#if false
    private void Update()
    {
        if (PauseMenu.IsPaused)
        {
            masterBase = PAUSED_MASTER_BASE;
            seBase = PAUSED_SE_BASE;
            bgmBase = PAUSED_BGM_BASE;
        }
        else
        {
            masterBase = MASTER_BASE;
            seBase = SE_BASE;
            bgmBase = BGM_BASE;
        }
    }
#endif

    public void SetMasterVolume(float rawValue)
    {
        var value = rawValue/10;
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f + MASTER_BASE, -80f, 0f);

        _audioMixer.SetFloat("VolumeMaster", volume);
        _masterText.text = rawValue.ToString();
    }

    public void SetSEVolume(float rawValue)
    {
        var value = rawValue / 10;
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f + SE_BASE, -80f, 0f);

        _audioMixer.SetFloat("VolumeSE", volume);
        _seText.text = rawValue.ToString();
    }

    public void SetBGMVolume(float rawValue)
    {
        var value = rawValue / 10;
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f + BGM_BASE, -80f, 0f);

        _audioMixer.SetFloat("VolumeBGM", volume);
        _bgmText.text = rawValue.ToString();
    }

    private void GetMasterVolume()
    {
        float volume = 0;
        _audioMixer.GetFloat("VolumeMaster", out volume);

        var value = Mathf.Pow(10, (volume - MASTER_BASE) / 20.0f);
        int setValue = Mathf.RoundToInt(value * 10);

        _masterText.text = setValue.ToString();
        _masterSlider.value = setValue;
    }

    private void GetSEVolume()
    {
        float volume = 0;
        _audioMixer.GetFloat("VolumeSE", out volume);

        var value = Mathf.Pow(10, (volume - SE_BASE) / 20.0f);
        int setValue = Mathf.RoundToInt(value * 10);

        _seText.text = setValue.ToString();
        _seSlider.value = setValue;
    }

    private void GetBGMVolume()
    {
        float volume = 0;
        _audioMixer.GetFloat("VolumeBGM", out volume);

        var value = Mathf.Pow(10, (volume - BGM_BASE) / 20.0f);
        int setValue = Mathf.RoundToInt(value * 10);

        _bgmText.text = setValue.ToString();
        _bgmSlider.value = setValue;
    }

    public void Open()
    {
        OptionUI.SetActive(true);
        
        _gamepadUISelect.SetEnable(true);

        if (Gamepad.current != null)
        {
            _eventSystem.SetSelectedGameObject(_firstSelect);
        }

        bool gamepadConectting = Gamepad.current != null;

        //_gamepadRebind.SetActive(gamepadConectting);
        //_keyboardRebind.SetActive(!gamepadConectting);
    }

    public void Close()
    {
        OptionUI.SetActive(false);

        _gamepadUISelect.SetEnable(false);

        if (Gamepad.current != null)
        {
            _eventSystem.SetSelectedGameObject(_returnSelect);
        }

        //_gamepadRebind.SetActive(false);
        //_keyboardRebind.SetActive(false);
    }
}

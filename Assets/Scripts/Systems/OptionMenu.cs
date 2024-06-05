using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class OptionMenu : MonoBehaviour
{
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
    private TextMeshProUGUI _seText;
    [SerializeField]
    private TextMeshProUGUI _bgmText;
    [SerializeField]
    private GameObject _keyboardRebind;
    [SerializeField]
    private GameObject _gamepadRebind;

    private EventSystem _eventSystem;

    private void Start()
    {
        OptionUI.SetActive(false);

        _eventSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EventSystem>();

    }

    public void SetMasterVolume(float rawValue)
    {
        var value = rawValue/10;
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        _audioMixer.SetFloat("VolumeMaster", volume);
        _masterText.text = rawValue.ToString();
    }

    public void SetSEVolume(float rawValue)
    {
        var value = rawValue / 10;
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        _audioMixer.SetFloat("VolumeSE", volume);
        _seText.text = rawValue.ToString();
    }

    public void SetBGMVolume(float rawValue)
    {
        var value = rawValue / 10;
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);

        _audioMixer.SetFloat("VolumeBGM", volume);
        _bgmText.text = rawValue.ToString();
    }

    public void Open()
    {
        OptionUI.SetActive(true);

        _eventSystem.SetSelectedGameObject(_firstSelect);

        bool gamepadConectting = Gamepad.current != null;

        _gamepadRebind.SetActive(gamepadConectting);
        _keyboardRebind.SetActive(!gamepadConectting);
    }

    public void Close()
    {
        OptionUI.SetActive(false);

        _eventSystem.SetSelectedGameObject(_returnSelect);

        _gamepadRebind.SetActive(false);
        _keyboardRebind.SetActive(false);
    }
}

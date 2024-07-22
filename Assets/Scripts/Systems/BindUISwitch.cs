using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindUISwitch : MonoBehaviour
{
    [SerializeField]
    private Sprite _gamepadSprite;

    private Sprite _keyboardSprite;
    private Image _image;
    private static PlayerInput _playerInput;
    private bool _isInit = false;

    private void Start()
    {
        _image = GetComponent<Image>();
        _keyboardSprite = _image.sprite;

        if(!_isInit)
        {
            _playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();
            _isInit = true;
        }
    }

    private void Update()
    {
        if (_image == null)
        {
            return;
        }

        if(_playerInput.currentControlScheme == "Gamepad")
        {
            _image.sprite = _gamepadSprite;
        }
        else
        {
            _image.sprite = _keyboardSprite;
        }
    }
}

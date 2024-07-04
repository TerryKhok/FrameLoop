using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindUISwitch : MonoBehaviour
{
    [SerializeField]
    private Sprite _gamepadSprite;

    private Sprite _keyboardSprite;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        _keyboardSprite = _image.sprite;
    }

    private void Update()
    {
        if (_image == null)
        {
            return;
        }

        if(Gamepad.current != null)
        {
            _image.sprite = _gamepadSprite;
        }
        else
        {
            _image.sprite = _keyboardSprite;
        }
    }
}

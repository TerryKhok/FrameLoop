using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EnterKeySwitcher : MonoBehaviour
{
    [SerializeField]
    private Sprite _gamePadSprite = null;
    [SerializeField]
    private Sprite _keyboardSprite = null;

    private PlayerInput _playerInput;
    private Image _image;

    private void Start()
    {
        _image = transform.GetComponent<Image>();
        _playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();

        if (Gamepad.current != null)
        {
            _image.sprite = _gamePadSprite;
        }
        else
        {
            _image.sprite = _keyboardSprite;
        }
    }

    private void Update()
    {
        if (_playerInput.currentControlScheme == "Gamepad")
        {
            _image.sprite = _gamePadSprite;
        }
        else
        {
            _image.sprite = _keyboardSprite;
        }
    }
}


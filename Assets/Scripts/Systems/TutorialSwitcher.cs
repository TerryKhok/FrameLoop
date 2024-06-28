using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialSwitcher : MonoBehaviour
{
    [SerializeField]
    private Sprite _gamePadSprite = null;
    [SerializeField]
    private Sprite _keyboardSprite = null;

    private PlayerInput _playerInput;
    private SpriteRenderer _renderer;

    private void Start()
    {
        _renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if(_playerInput.currentControlScheme == "Gamepad")
        {
            _renderer.sprite = _gamePadSprite;
        }
        else
        {
            _renderer.sprite = _keyboardSprite;
        }
    }
}

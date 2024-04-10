using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _player = null;
    [SerializeField]
    private GameObject _frame = null;

    private PlayerInput _playerInput = null;
    private PlayerMove _playerMove = null;
    private PlayerJump _playerJump = null;
    private PlayerCrouch _playerCrouch = null;
    private PlayerTakeUp _playerTakeUp = null;
    private FrameLoop _frameLoop = null;

    private InputAction _Move, _Jump, _FrameEnable, _Crouch, _TakeUp;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerMove = _player.GetComponent<PlayerMove>();
        _playerJump = _player.GetComponent <PlayerJump>();
        _playerCrouch = _player.GetComponent<PlayerCrouch>();
        _playerTakeUp = _player.GetComponent<PlayerTakeUp>();
        _frameLoop = _frame.GetComponent<FrameLoop>();

        _Move = _playerInput.actions["Move"];
        _Jump = _playerInput.actions["Jump"];
        _FrameEnable = _playerInput.actions["FrameEnable"];
        _Crouch = _playerInput.actions["Crouch"];
        _TakeUp = _playerInput.actions["TakeUp"];

        _playerInput.SwitchCurrentActionMap("Player");
        //_playerInput.SwitchCurrentControlScheme("Gamepad");
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        if(_playerInput == null) { return; }

        _Move.performed += _playerMove.MovePerformed;
        _Move.canceled += _playerMove.MoveCanceled;
        _Jump.started += _playerJump.JumpStarted;
        _Jump.canceled += _playerJump.JumpCanceled;
        _Crouch.started += _playerCrouch.CrouchStarted;
        _Crouch.canceled += _playerCrouch.CrouchCanceled;
        _TakeUp.started += _playerTakeUp.TakeUpStarted;
        _TakeUp.canceled += _playerTakeUp.TakeUpCanceled;
        _FrameEnable.started += _frameLoop.FrameStarted;
        _FrameEnable.canceled += _frameLoop.FrameCanceled;
    }

    private void OnDisable()
    {
        if (_playerInput == null) { return; }

        _Move.performed -= _playerMove.MovePerformed;
        _Move.canceled -= _playerMove.MoveCanceled;
        _Jump.started -= _playerJump.JumpStarted;
        _Jump.canceled -= _playerJump.JumpCanceled;
        _Crouch.started -= _playerCrouch.CrouchStarted;
        _Crouch.canceled -= _playerCrouch.CrouchCanceled;
        _TakeUp.started -= _playerTakeUp.TakeUpStarted;
        _TakeUp.canceled -= _playerTakeUp.TakeUpCanceled;
        _FrameEnable.started -= _frameLoop.FrameStarted;
        _FrameEnable.canceled -= _frameLoop.FrameCanceled;
    }
}

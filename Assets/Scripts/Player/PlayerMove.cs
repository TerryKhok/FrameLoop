using UnityEngine;
using UnityEngine.InputSystem;

//�v���C���[�̈ړ����s���N���X
public class PlayerMove : MonoBehaviour
{
    [SerializeField,Tooltip("�v���C���[�̑��x���(m/s)")]
    private float _targetVelocity = 5.0f;
    private Rigidbody2D _rb = null;
    private Transform _transform;
    private Vector2 _currentInput = Vector2.zero;
    private PlayerInfo _playerInfo;
    private PlayerAnimation _playerAnimation;

    //box.cs�Ŏg���Ă���
    public bool _isMoving = false;

    private void Start()
    {
        //PlayerInfo�N���X����ϐ����󂯎��
        _playerInfo = PlayerInfo.Instance;
        _rb = _playerInfo.g_rb;
        _transform = _playerInfo.g_transform;

        _playerAnimation = PlayerAnimation.Instance;
    }

    private void Update()
    {
        //Debug.Log(_rb.velocity);

        //Walk�A�j���[�V�������Đ�
        _playerAnimation.SetWalkAnimation(_isMoving);
    }

    private void FixedUpdate()
    {
        move();
        rotate();
    }

    //InputSystem�̃R�[���o�b�N���󂯎�郁�\�b�h
    public void MovePerformed(InputAction.CallbackContext context)
    {
        //���Ղ̉�
        AudioManager.instance.Play("Walk");
        _isMoving = true;
        Debug.Log("ismoving true");

        //WASD�ALeftStick�ADpad�̓��͂�Vector2�Ƃ��Ď󂯎��
        var input = context.ReadValue<Vector2>();
        //Y���̓��͂𖳌�������
        _currentInput = Vector2.Scale(input, new Vector2(1, 0)).normalized;
    }

    public void MoveCanceled(InputAction.CallbackContext context)
    {
        //���Ղ̉�
        AudioManager.instance.Stop("Walk");
        _isMoving = false;
        Debug.Log("ismoving false");

        _currentInput = Vector2.zero;
    }

    private void rotate()
    {
        //���͂�������΃��^�[��
        if(_currentInput == Vector2.zero) { return; }
        if (_playerInfo.g_takeUpFg) { return; }

        if(_currentInput.x < 0)
        {
            _transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            _transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void move()
    {
        if(_currentInput.x == _playerInfo.g_wall)
        {
            return;
        }

        //�ړ�
        var currentPos = _rb.position;
        currentPos += _currentInput * _targetVelocity * Time.fixedDeltaTime;
        _rb.position = currentPos;
    }
}

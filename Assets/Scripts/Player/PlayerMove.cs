using UnityEngine;
using UnityEngine.InputSystem;

//�v���C���[�̈ړ����s���N���X
public class PlayerMove : MonoBehaviour
{
    [SerializeField,Tooltip("�v���C���[�̑��x���(m/s)")]
    private float _targetVelocity = 5.0f;

    [SerializeField, Tooltip("���Ⴊ�ݒ��̈ړ����x")]
    private float _crouchVelocity = 3.0f;

    private Rigidbody2D _rb = null;
    private Transform _transform;
    private Vector2 _currentInput = Vector2.zero;
    private PlayerInfo _playerInfo;
    private PlayerAnimation _playerAnimation;

    //box.cs�Ŏg���Ă���
    public bool _isMoving = false;

    private bool _se = false;

    private void Start()
    {
        //PlayerInfo�N���X����ϐ����󂯎��
        _playerInfo = PlayerInfo.Instance;
        _rb = _playerInfo.g_rb;
        _transform = _playerInfo.g_transform;

        _playerAnimation = PlayerAnimation.Instance;

        _playerAnimation.SetCrouchSpeed(_crouchVelocity / _targetVelocity);
    }

    private void Update()
    {
        //Debug.Log(_rb.velocity);
        _playerAnimation.SetMoveX((int)_currentInput.x);

        //Walk�A�j���[�V�������Đ�
        _playerAnimation.SetWalkAnimation(_isMoving && _playerInfo.g_isGround);

        _playerInfo.g_currentInputX = (int)_currentInput.x;

        if(_isMoving && _playerInfo.g_isGround)
        {
            if (!_se)
            {
                //���Ղ̉�
                AudioManager.instance.Play("Walk");
                _se = true;
            }
        }
        else
        {
            if (_se)
            {
                //���Ղ̉�
                AudioManager.instance.Stop("Walk");
                _se = false;
            }
        }
    }

    private void FixedUpdate()
    {
        move();
        rotate();
    }

    //InputSystem�̃R�[���o�b�N���󂯎�郁�\�b�h
    public void MovePerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("ismoving true");

        //WASD�ALeftStick�ADpad�̓��͂�Vector2�Ƃ��Ď󂯎��
        var input = context.ReadValue<Vector2>();
        //Y���̓��͂𖳌�������
        _currentInput = Vector2.Scale(input, new Vector2(1, 0)).normalized;

        _isMoving = _currentInput.x != 0;
    }

    public void MoveCanceled(InputAction.CallbackContext context)
    {
        _isMoving = false;
        //Debug.Log("ismoving false");

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

        if(_playerInfo.g_walkCancel)
        {
            return;
        }

        var velocity = _targetVelocity;

        if(_playerInfo.g_takeUpFg || _playerInfo.g_isCrouch) 
        {
            velocity = _crouchVelocity;
        }

        if (FrameLoop.Instance != null)
        {
            //�i�s�������ǂȂ�return����
            Vector3 pos = _transform.position;
            pos += (Vector3)_currentInput * 0.5f;
            pos -= _transform.up * 0.25f;
            Ray ray = new Ray(pos, _currentInput);
            RaycastHit2D hit;
            LayerMask mask;

            //�t���[�����L�����ǂ�����LayerMask��Layer��ύX
            if (FrameLoop.Instance.g_isActive)
            {
                mask = _playerInfo.g_insideMask;
            }
            else
            {
                mask = _playerInfo.g_outsideMask;
            }

            hit = Physics2D.Raycast(ray.origin, ray.direction, 0.05f, mask);

            if (hit.collider != null)
            {
                if(_playerInfo.g_box == null)
                {
                    return;
                }

                //�����Ă��锠�ȊO���ړ������ɂ�������return����
                Box box = _playerInfo.g_box.GetComponent<Box>();
                if(!box.ContainsCopyList(hit.transform))
                {
                    return;
                }
            }
        }

        //�ړ�
        var currentPos = _rb.position;
        currentPos += _currentInput * velocity * Time.fixedDeltaTime;
        _rb.position = currentPos;
    }

    public void SetMove(int x)
    {
        _currentInput = new Vector2(x, 0);

        _isMoving = _currentInput.x != 0;
    }
}

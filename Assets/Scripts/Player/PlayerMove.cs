using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//�v���C���[�̈ړ����s���N���X
public class PlayerMove : MonoBehaviour
{
    [SerializeField, Tooltip("�v���C���[�̑��x���(m/s)")]
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

    private GameObject walkRightDust = null;
    private GameObject walkLeftDust = null;
    private SpriteRenderer _rightDustRenderer, _leftDustRenderer;
    private float animationTime = 0.4f;
    private int walkDir; //0 = right, 1 = left
    [SerializeField]
    private ParticleSystem walkDustParticles;
    private ParticleSystemRenderer walkParticlesRnderer;

    private void Start()
    {
        //PlayerInfo�N���X����ϐ����󂯎��
        _playerInfo = PlayerInfo.Instance;
        _rb = _playerInfo.g_rb;
        _transform = _playerInfo.g_transform;

        _playerAnimation = PlayerAnimation.Instance;

        _playerAnimation.SetCrouchSpeed(_crouchVelocity / _targetVelocity);

        AudioManager.instance.Stop("Walk");

        walkRightDust = GameObject.Find("VFX_WalkRight");
        _rightDustRenderer = walkRightDust.GetComponent<SpriteRenderer>();
        walkLeftDust = GameObject.Find("VFX_WalkLeft");
        _leftDustRenderer = walkLeftDust.GetComponent<SpriteRenderer>();

        walkParticlesRnderer = walkDustParticles.GetComponent<ParticleSystemRenderer>();
    }

    private void Update()
    {
        var currentVelocity = _rb.velocity;
        currentVelocity.y = 0;
        _rb.velocity -= currentVelocity;

        //Debug.Log(_rb.velocity);
        _playerAnimation.SetMoveX((int)_currentInput.x);

        //Walk�A�j���[�V�������Đ�
        _playerAnimation.SetWalkAnimation(_isMoving && _playerInfo.g_isGround);

        _playerInfo.g_currentInputX = (int)_currentInput.x;

        if (FrameLoop.Instance != null)
        {
            if (FrameLoop.Instance.g_isActive)
            {
                walkParticlesRnderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            else
            {
                walkParticlesRnderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
        }

        if(_playerInfo.g_isGround && _isMoving)
        {
            walkDustParticles.Play();
        }
    }

    private void FixedUpdate()
    {
        move();
        rotate();

        if (_isMoving && _playerInfo.g_isGround)
        {
            if (!_se)
            {
                //���Ղ̉�
                AudioManager.instance.Play("Walk");
                _se = true;

                //vfx�A�j���[�V������
                if (walkDir == 0)
                {
                    if (FrameLoop.Instance != null)
                    {
                        if (FrameLoop.Instance.g_isActive)
                        {
                            _rightDustRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                        }
                        else
                        {
                            _rightDustRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                        }
                    }

                    Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.3f);
                    GameObject g = Instantiate(walkRightDust, playerPos, Quaternion.identity);
                    Destroy(g, animationTime);

                    var copyList = _playerInfo.GetCopyList();
                    foreach (var copy in copyList)
                    {
                        playerPos = new Vector2(copy.position.x, copy.position.y - 0.3f);
                        g = Instantiate(walkRightDust, playerPos,Quaternion.identity);
                        Destroy(g, animationTime);
                    }
                }
                else
                {
                    if (FrameLoop.Instance != null)
                    {
                        if (FrameLoop.Instance.g_isActive)
                        {
                            _leftDustRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                        }
                        else
                        {
                            _leftDustRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                        }
                    }

                    Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.3f);
                    GameObject g = Instantiate(walkLeftDust, playerPos, Quaternion.identity);
                    Destroy(g, animationTime);

                    var copyList = _playerInfo.GetCopyList();
                    foreach (var copy in copyList)
                    {
                        playerPos = new Vector2(copy.position.x, copy.position.y - 0.3f);
                        g = Instantiate(walkLeftDust, playerPos, Quaternion.identity);
                        Destroy(g, animationTime);
                    }
                }
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
        if (_currentInput == Vector2.zero) { return; }
        if (_playerInfo.g_takeUpFg) { return; }

        if (_currentInput.x < 0)
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
        if (_currentInput.x == _playerInfo.g_wall)
        {
            return;
        }

        if (_playerInfo.g_walkCancel)
        {
            _playerInfo.g_walkCancel = false;
            return;
        }

        var velocity = _targetVelocity;

        if (_playerInfo.g_takeUpFg || _playerInfo.g_isCrouch)
        {
            velocity = _crouchVelocity;
        }

        _playerInfo.g_walkCancel = true;

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
                if (_playerInfo.g_box == null)
                {
                    return;
                }

                //�����Ă��锠�ȊO���ړ������ɂ�������return����
                Box box = _playerInfo.g_box.GetComponent<Box>();
                if (!box.ContainsCopyList(hit.transform))
                {
                    return;
                }
            }
        }
        _playerInfo.g_walkCancel = false;

        //�ړ�
        var currentPos = _rb.position;
        currentPos += _currentInput * velocity * Time.fixedDeltaTime;
        if (currentPos.x - _rb.position.x > 0.0f) //if right
        {
            walkDir = 0;
        }
        else //if left
        {
            walkDir = 1;
        }
        _rb.position = currentPos;
    }

    public void SetMove(int x)
    {
        _currentInput = new Vector2(x, 0);

        _isMoving = _currentInput.x != 0;
    }

    public void SetRotate(int x)
    {
        if(x == 0)
        {
            return; 
        }

        if (x < 0)
        {
            _transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            _transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void SetTargetVelocity(float target)
    {
        _targetVelocity = target;
        _playerAnimation.SetWalkSpeed(target / 7.0f);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerTakeUp
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :����͂�
 *               
 *  Created     :2024/04/27
 */
public class PlayerTakeUp : MonoBehaviour
{
    private Transform _transform = null;
    private PlayerInfo _playerInfo = null;
    private FrameLoop _frameLoop = null;
    private BoxCollider2D _boxCollider = null;
    private IBox _box = null;
    [SerializeField,Tooltip("�؂�ւ�")]
    private bool _toggle = false;

    private int _count = 0;
    private LayerMask _insideMask = 0, _outsideMask = 0;

    private PlayerAnimation _playerAnimation = null;

    private void Start()
    {
        _playerInfo = PlayerInfo.Instance;
        _frameLoop = FrameLoop.Instance;
        _playerAnimation = PlayerAnimation.Instance;
        _transform = _playerInfo.g_transform;
        _boxCollider = _playerInfo.g_collider;

        _insideMask = 1 << LayerMask.NameToLayer("IBox");
        _outsideMask = 1 << LayerMask.NameToLayer("OBox");
    }

    private void Update()
    {
        takeUp();
        _playerAnimation.SetHoldAnimation(_playerInfo.g_takeUpFg);
    }

    //�������邩�̔���ƒ͂�
    public void TakeUpStarted(InputAction.CallbackContext context)
    {
        //�؂�ւ�����̏���
        if(_toggle && _playerInfo.g_takeUpFg)
        {
            //���𗣂�
            _box.Hold(null);
            return;
        }

        //�v���C���[�̐i�s�����ւ�Ray
        Ray ray = new Ray(_transform.position, _transform.right);
        RaycastHit2D hit;
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);

        LayerMask mask = 0;
        if (_frameLoop.g_isActive)
        {
            mask = _insideMask;
        }
        else
        {
            mask = _outsideMask;
        }

        //�v���C���[�̐i�s�����ɔ������邩����
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 1f,mask);
        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Box"))
            {
                //IBox���p������Component���擾
                _box = hit.transform.GetComponent<IBox>();

                //����͂�
                _box.Hold(_transform);
                _playerInfo.g_boxDirection = (int)ray.direction.x;
            }
        }
    }

    //�������𗣂�
    public void TakeUpCanceled(InputAction.CallbackContext context)
    {
        //�؂�ւ�����̏ꍇ��return
        if (_toggle) { return; }

        _box.Hold(null);
    }

    //����͂�ł���Ԃ̏���
    private void takeUp()
    {
        //����͂�łȂ�������return
        if (!_playerInfo.g_takeUpFg) { return; }

        _playerInfo.g_takeUpFg = true;

        //�i�s�����ւ�Ray
        Ray ray = new Ray(_transform.position, _transform.right);
        RaycastHit2D hit;

        Vector3 size = new Vector3(0.42f, 0.5f, 0.5f);
        float length = 1 + _boxCollider.size.x/2;


        //�t���[�����L�����ǂ�����LayerMask��ύX
        LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform");
        if (_frameLoop.g_isActive)
        {
            mask = 1 << LayerMask.NameToLayer("IPlatform");
        }

        //�ǂ����邩�𔻒�
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, length, mask);

        if(hit.collider != null)
        {
            //�ǂ���������ǂ̏����X�V
            _playerInfo.g_wall = _transform.right.normalized.x;
        }
        else
        {
            //�ǂ�����������ǂ̏������Z�b�g
            _playerInfo.g_wall = 0;
        }
    }
}

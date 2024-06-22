using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Canon
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�e�𔭎˂���
 *               
 *  Created     :2024/04/27
 */
public class Canon : MonoBehaviour
{
    [SerializeField,Tooltip("���˂���Prefab")]
    private GameObject _bulletPrefab = null;
    [SerializeField, Tag,Tooltip("�j��\��Tag")]
    private List<string> _breakTag = new List<string>();
    [SerializeField,Tooltip("���˕���")]
    private Vector2 _direction = Vector2.zero;
    [SerializeField,Tooltip("���ˑ��x")]
    private float _velocity = 1f;
    [SerializeField,Tooltip("�˒�")]
    private float _range = 1f;
    [SerializeField,Tooltip("���ˊԊu(s)")]
    private float _interval = 1f;
    [SerializeField,Tooltip("���߂���L����")]
    private bool _enabledOnAwake = true;

    private GameObject _instance = null;
    private Bullet _bullet = null;
    private float _elapsedTime = 0f;
    private Transform _transform = null;
    private Vector3 _position = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    private bool _enable = false;
    private bool _frameInside = false;

    private GameObject _colUpObj, _colLowObj;

    private void Start()
    {
        _colUpObj = transform.GetChild(0).GetChild(0).gameObject;
        _colLowObj = transform.GetChild(0).GetChild(1).gameObject;

        _enable = _enabledOnAwake;

        //���ˈʒu�͎q�I�u�W�F�N�g�̈ʒu���Q�Ƃ���
        _transform = transform.GetChild(0);

        //�e�̌����𔭎˕����Ɍ�������
        _rotation = Quaternion.FromToRotation(Vector3.right,_direction);

        //���ˈʒu�𔭎˕����Ɉ�苗�����炷
        _position = _transform.position;
        _position += (Vector3)_direction.normalized;

        //�ŏ�����N�����Ă����甭�˂���
        if (!_enable) { return; }
        InstantiateBullet();
    }

    private void Update()
    {
        if (!_enable) {  return; }

        _elapsedTime += Time.deltaTime;

        //��莞�Ԗ��ɔ��˂���
        if(_interval < _elapsedTime)
        {
            InstantiateBullet();
        }
    }

    //�e�𔭎˂���
    private void InstantiateBullet()
    {
        _instance = Instantiate(_bulletPrefab, _position, _rotation);
        _bullet = _instance.GetComponent<Bullet>();
        _bullet.SetValues(_direction, _velocity, _range, _breakTag);

        //�����ʒu���t���[�����ŁA�t���[�����L���ȏꍇ�̏���
        if(FrameLoop.Instance.g_isActive && _frameInside)
        {
            //Frame��Insider��Bullet��ǉ�
            FrameLoop.Instance.AddInsiders(_instance.GetComponent<Collider2D>());
        }

        //�o�ߎ��Ԃ����Z�b�g
        _elapsedTime = 0f;
    }

    //�L�����ǂ����������ŕύX����
    public void SetEnable(bool enable)
    {
        _enable = enable;
    }

    //�L�����������𔽓]������
    public void SwitchEnable()
    {
        _enable = !_enable;
    }

    //�t���[���ɏd�Ȃ��Ă��邩�Ń��C���[��ύX����
    public void CanonLayerCheck()
    {
        Vector3 offset = new Vector3(0, 0.25f, 0);
        GameObject obj = _colUpObj;

        for (int i = 0; i < 2; i++)
        {
            //�X�N���[�����W�ɕϊ�
            var pos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);

            //���W�Ɉʒu�Ƀ��C���΂�
            Ray ray = Camera.main.ScreenPointToRay(pos);
            LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1.0f, mask);

            if (hit.collider != null)
            {
                obj.layer = LayerMask.NameToLayer("Inside");
                //���ˈʒu���t���[���̓������𔻒�
                if(i == 0)
                {
                    _frameInside = true;
                }
            }
            else
            {
                obj.layer = LayerMask.NameToLayer("Outside");
                //���ˈʒu���t���[���̓������𔻒�
                if (i == 0)
                {
                    _frameInside = false;
                }
            }

            obj = _colLowObj;
            offset.y -= 0.5f;
        }
    }
}

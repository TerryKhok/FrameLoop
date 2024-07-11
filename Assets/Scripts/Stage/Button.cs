using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/*  ProjectName :FrameLoop
 *  ClassName   :Button
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�{�^���̏�Ԃɂ���ăC�x���g�����s����
 *               
 *  Created     :2024/04/27
 */
[RequireComponent(typeof(BoxCollider2D))]
public class Button : MonoBehaviour
{
    private BoxCollider2D _collider = null;
    [SerializeField,Tooltip("Goal�ɕK�v��")]
    private bool _toGoal = false;
    [SerializeField, Tag ,Header("�{�^���������\��Tag")]
    private List<string> _tagList = new List<string>();
    [SerializeField,Tooltip("�����ꂽ�Ƃ��Ɉ�x���s���郁�\�b�h")]
    private UnityEvent _onClick = null;
    [SerializeField,Tooltip("������Ă���Ԏ��s���郁�\�b�h")]
    private UnityEvent _onHold = null;
    [SerializeField,Tooltip("�������Ƃ��Ɏ��s���郁�\�b�h")]
    private UnityEvent _onRelease = null;
    [SerializeField, Tooltip("������Ă����Ԃ�sprite")]
    private Sprite _pushed;

    private Sprite _unpushed;

    private SpriteRenderer _renderer;
    private int _hitCount = 0;
    private bool _isPressed = false, _prevPressed = false;

    private bool pressedSoundFlag = false;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;

        _renderer = GetComponent<SpriteRenderer>();
        _unpushed = _renderer.sprite;
    }

    private void Update()
    {
        //�O�t���[���ł̏�Ԃ�ۑ�
        _prevPressed = _isPressed;

        //�d�Ȃ��Ă���I�u�W�F�N�g�����邩�ŏ�Ԃ𔻒�
        _isPressed = _hitCount > 0;


        if(_isPressed)
        {
            if (!pressedSoundFlag)
            {
                AudioManager.instance.Play("Button");
                AudioManager.instance.Play("DoorOpen");
                pressedSoundFlag = true;
            }

            _renderer.sprite = _pushed;

            if(_prevPressed)
            {
                //Debug.Log("hold");

                //������Ă���Ԏ��s���鏈��
                _onHold.Invoke();
            }
            else
            {
                //Debug.Log("click");

                //�S�[���ɕK�v�ȃ{�^���Ȃ�J�E���g�A�b�v
                if (_toGoal)
                {
                    Goal.Instance.CountUp();
                }

                //�����ꂽ���Ɉ�x���s���鏈��
                _onClick.Invoke();
            }
        }
        else
        {
            _renderer.sprite= _unpushed;

            if (pressedSoundFlag)
            {
                AudioManager.instance.Play("DoorClose");
                pressedSoundFlag = false;
            }

            if (_prevPressed)
            {
                //Debug.Log("release");

                //�S�[���ɕK�v�ȃ{�^���Ȃ�J�E���g�_�E��
                if (_toGoal)
                {
                    Goal.Instance.CountDown();
                }

                //�����ꂽ���Ɉ�x���s���鏈��
                _onRelease.Invoke();
            }
        }
    }

    //�t���[���ɏd�Ȃ��Ă��邩�Ń��C���[��ύX����
    public void ButtonLayerCheck()
    {
        //�X�N���[�����W�ɕϊ�
        var pos = Camera.main.WorldToScreenPoint(transform.position);

        //���W�Ɉʒu�Ƀ��C���΂�
        Ray ray = Camera.main.ScreenPointToRay(pos);
        LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10, mask);

        if (hit.collider != null)
        {
            gameObject.layer = LayerMask.NameToLayer("Inside");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Outside");
        }
    }

    //���C���[��߂�
    public void SetOutsideLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Outside");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //�{�^����������^�O�̃I�u�W�F�N�g�Ȃ�J�E���g�𑝂₷
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _hitCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //�{�^����������^�O�̃I�u�W�F�N�g�Ȃ�J�E���g�����炷
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _hitCount--;
    }

    public bool IsToGoal()
    {
        return _toGoal;
    }
}

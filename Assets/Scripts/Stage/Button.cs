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

    private int _hitCount = 0;
    private bool _isPressed = false, _prevPressed = false;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    private void Update()
    {
        //�O�t���[���ł̏�Ԃ�ۑ�
        _prevPressed = _isPressed;

        //�d�Ȃ��Ă���I�u�W�F�N�g�����邩�ŏ�Ԃ𔻒�
        _isPressed = _hitCount > 0;

        if(_isPressed)
        {
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
}

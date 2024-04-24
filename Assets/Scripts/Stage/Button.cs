using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        _prevPressed = _isPressed;
        _isPressed = _hitCount > 0;

        if(_isPressed)
        {
            if(_prevPressed)
            {
                //Debug.Log("hold");
                _onHold.Invoke();
            }
            else
            {
                //Debug.Log("click");
                if (_toGoal)
                {
                    Goal.Instance.CountUp();
                }
                _onClick.Invoke();
            }
        }
        else
        {
            if (_prevPressed)
            {
                //Debug.Log("release");
                if (_toGoal)
                {
                    Goal.Instance.CountDown();
                }
                _onRelease.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _hitCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _hitCount--;
    }
}

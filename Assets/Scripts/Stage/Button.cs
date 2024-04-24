using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Button : MonoBehaviour
{
    private BoxCollider2D _collider = null;
    [SerializeField,Tooltip("Goalに必要か")]
    private bool _toGoal = false;
    [SerializeField, Tag ,Header("ボタンを押下可能なTag")]
    private List<string> _tagList = new List<string>();
    [SerializeField,Tooltip("押されたときに一度実行するメソッド")]
    private UnityEvent _onClick = null;
    [SerializeField,Tooltip("押されている間実行するメソッド")]
    private UnityEvent _onHold = null;
    [SerializeField,Tooltip("離したときに実行するメソッド")]
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

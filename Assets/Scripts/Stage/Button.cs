using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Button : MonoBehaviour
{
    private BoxCollider2D _collider = null;
    [SerializeField, Tag ,Header("ボタンを押下可能なTag")]
    private List<string> _tagList = new List<string>();
    [SerializeField,Tooltip("押されたときに一度実行するメソッド")]
    private UnityEvent _onClick = null;
    [SerializeField,Tooltip("押されている間実行するメソッド(FixedUpdate)")]
    private UnityEvent _onHold = null;
    [SerializeField,Tooltip("離したときに実行するメソッド")]
    private UnityEvent _onRelease = null;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _onClick.Invoke();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }

        _onHold.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }

        _onRelease.Invoke();
    }
}

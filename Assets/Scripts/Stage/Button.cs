using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Button : MonoBehaviour
{
    private BoxCollider _collider = null;
    [SerializeField, Tag]
    private List<string> _tagList = new List<string>();
    [SerializeField]
    private UnityEvent _onClick = null;
    [SerializeField]
    private UnityEvent _onHold = null;
    [SerializeField]
    private UnityEvent _onRelease = null;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _onClick.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }

        _onHold.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_tagList.Contains(other.tag))
        {
            return;
        }

        _onRelease.Invoke();
    }
}

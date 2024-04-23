using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChildOnTrigger : MonoBehaviour
{
    private IParentOnTrigger _parentOnTrigger = null;
    private Transform _transform = null;

    private void OnEnable()
    {
        _transform = transform;
        _parentOnTrigger = _transform.parent.GetComponent<IParentOnTrigger>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(_parentOnTrigger == null) { return; }
        _parentOnTrigger.OnEnter(collision,_transform);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(_parentOnTrigger == null) { return; }
        _parentOnTrigger.OnStay(collision, _transform);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(_parentOnTrigger == null) { return; }
        _parentOnTrigger.OnExit(collision, _transform);
    }
}

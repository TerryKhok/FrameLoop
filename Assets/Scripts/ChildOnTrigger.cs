using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChildOnTrigger : MonoBehaviour
{
    private IParentOnTrigger _parentOnTrigger = null;

    private void OnEnable()
    {
        _parentOnTrigger = transform.parent.GetComponent<IParentOnTrigger>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        _parentOnTrigger.OnEnter(collision);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        _parentOnTrigger.OnStay(collision);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        _parentOnTrigger.OnExit(collision);
    }
}

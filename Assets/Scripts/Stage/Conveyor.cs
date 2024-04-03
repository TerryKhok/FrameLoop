using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Conveyor : MonoBehaviour
{
    [SerializeField]
    private float _velocity = 1f;
    [SerializeField, Tag]
    private List<string> _tagList = new List<string>();
    [SerializeField]
    private bool _inverse = false;
    private BoxCollider _boxCollider = null;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        var size = _boxCollider.size;
        size.y += 0.3f;
        _boxCollider.size = size;

        var center = _boxCollider.center;
        center.y += 0.15f;
        _boxCollider.center = center;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        var rb = other.GetComponent<Rigidbody>();

        if(rb == null) {  return; }

        var pos = rb.position;
        if (_inverse) { pos.x -= _velocity*Time.fixedDeltaTime; }
        else { pos.x += _velocity*Time.fixedDeltaTime; }

        rb.position = pos;
    }
}

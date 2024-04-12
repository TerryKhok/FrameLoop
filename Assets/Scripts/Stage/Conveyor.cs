using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Conveyor : MonoBehaviour
{
    [SerializeField,Tooltip("‰ñ“]‘¬“x(m/s)")]
    private float _velocity = 1f;
    [SerializeField, Tag, Tooltip("‰e‹¿‚ð—^‚¦‚éTag")]
    private List<string> _tagList = new List<string>() { "Player" };
    [SerializeField,Tooltip("‹t‰ñ“]‚³‚¹‚é")]
    private bool _inverse = false;
    private BoxCollider2D _boxCollider = null;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;
        var size = _boxCollider.size;
        size.y += 0.3f;
        _boxCollider.size = size;

        var offset = _boxCollider.offset;
        offset.y += 0.15f;
        _boxCollider.offset = offset;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        var rb = other.GetComponent<Rigidbody2D>();

        if(rb == null) {  return; }

        var pos = rb.position;
        if (_inverse) { pos.x -= _velocity*Time.fixedDeltaTime; }
        else { pos.x += _velocity*Time.fixedDeltaTime; }

        rb.position = pos;
    }

    public void SetInverse(bool inverse)
    {
        _inverse = inverse;
    }

    public void FripDirection()
    {
        _inverse = !_inverse;
    }
}

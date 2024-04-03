using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private Rigidbody _rb = null;
    private Vector2 _direction = Vector2.zero;
    private float _velocity = 1f;
    private float _lifeSpan = 1f;
    private float _elapsedTime = 0f;
    private List<string> _tagList = new List<string>();

    static private List<string> _destroyTag = new List<string>() { "Wall" };

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        _rb.velocity = _direction.normalized * _velocity;

        if (_lifeSpan < _elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(_destroyTag.Contains(other.tag))
        {
            Destroy(this.gameObject);
            return;
        }

        if(!_tagList.Contains(other.tag))
        {
            return;
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
            rb.AddForce(_direction * _velocity, ForceMode.Force);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Box")) { return; }
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionX;
        }
    }

    public void SetValues(Vector2 direction, float velocity, float lifeSpan, List<string> tag)
    {
        _direction = direction;
        _velocity = velocity;
        _lifeSpan = lifeSpan;
        _tagList = new List<string>(tag);
    }
}
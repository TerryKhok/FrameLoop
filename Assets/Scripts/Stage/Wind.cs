using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private Rigidbody _rb = null;
    private Vector2 _direction = Vector2.zero;
    private float _velocity = 1f;
    private float _lifeSpan = 1f;
    private float _elapsedTime = 0f;
    private float _decayOfDistance = 0f;

    static private List<string> _destroyTag = new List<string>() { "Wall" };

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        _rb.velocity = _direction * _velocity;
        _decayOfDistance = 1 - (_elapsedTime / _lifeSpan);

        if (_lifeSpan < _elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(_direction * (_velocity * _decayOfDistance), ForceMode.Force);
        }

        foreach(string tag in _destroyTag)
        {
            if (other.CompareTag(tag))
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetValues(Vector2 direction, float velocity, float lifeSpan)
    {
        _direction = direction;
        _velocity = velocity;
        _lifeSpan = lifeSpan;
    }
}
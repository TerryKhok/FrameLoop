using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rb = null;
    private Vector2 _direction = Vector2.zero;
    private float _velocity = 1f;
    private float _lifeSpan = 1f;
    private float _elapsedTime = 0f;
    private List<string> _tagList = new List<string>();

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        _rb.velocity = _direction.normalized * _velocity;

        if(_lifeSpan < _elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_tagList.Contains(collision.transform.tag))
        {
            Destroy(collision.gameObject);
            return;
        }
        Destroy(this.gameObject);
    }

    public void SetValues(Vector2 direction, float velocity, float range, List<string> tagList)
    {
        _direction = direction;
        _velocity = velocity;
        _lifeSpan = range/velocity;
        _tagList = new List<string>(tagList);
    }
}

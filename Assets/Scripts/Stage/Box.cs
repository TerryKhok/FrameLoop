using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private float _height = 0f;
    private Transform _transform;
    [SerializeField]
    private float _width = 1f;
    [SerializeField]
    private float _breakHeight = 5f;
    [SerializeField,Tag]
    private List<string> _tagList = new List<string>();

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_height < _transform.position.y)
        {
            _height = _transform.position.y;
        }

        Ray ray = new Ray(_transform.position, Vector3.down);
        RaycastHit2D hit;
        Vector2 size = new Vector2(_width/2, 0.5f);
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 1, 1 << 6);
        if(hit.collider != null)
        {
            if(hit.distance > 0.3f) { return; }
            if(hit.transform.CompareTag("Breakable"))
            {
                if(_height - _transform.position.y >= _breakHeight)
                {
                    Destroy(hit.transform.gameObject);
                }
            }
            else
            {
                _height = _transform.position.y;
            }
        }
    }
}

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
        RaycastHit hit;
        Vector3 size = new Vector3(_width/2, 0.5f, 0.5f);
        if(Physics.BoxCast(
            ray.origin,
            size,
            ray.direction,
            out hit,
            Quaternion.identity,
            1f,
            1 << 6,
            QueryTriggerInteraction.Ignore)
        )
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

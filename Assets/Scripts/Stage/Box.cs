using UnityEngine;

public class Box : MonoBehaviour
{
    private float _prevHeight = 0f;
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_prevHeight < _transform.position.y)
        {
            _prevHeight = _transform.position.y;
        }

        Ray ray = new Ray(_transform.position, Vector3.down);
        RaycastHit hit;
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
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
                if(_prevHeight - _transform.position.y >= 5)
                {
                    Destroy(hit.transform.gameObject);
                }
                else
                {
                    _prevHeight = _transform.position.y;
                }
            }
            else
            {
                _prevHeight = _transform.position.y;
            }
        }
    }
}

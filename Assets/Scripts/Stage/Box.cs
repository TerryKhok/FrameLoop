using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private float _height = 0f;
    private Transform _transform, _playerTransform;
    [SerializeField,Tooltip("î†ÇÃâ°ïù")]
    private float _width = 1f;
    [SerializeField,Tooltip("îjâÛÇ∑ÇÈÇÃÇ…ïKóvÇ»çÇÇ≥")]
    private float _breakHeight = 5f;
    [SerializeField,Tag,Tooltip("îjâÛâ¬î\Ç»Tag")]
    private List<string> _tagList = new List<string>() { "Breakable"};

    private Rigidbody2D _rb = null;

    private void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    private void Update()
    {
        platformBreak();
    }

    private void FixedUpdate()
    {
        isHold();
    }

    private void platformBreak()
    {
        if (_height < _transform.position.y)
        {
            _height = _transform.position.y;
        }

        Ray ray = new Ray(_transform.position, Vector3.down);
        RaycastHit2D hit;
        Vector2 size = new Vector2(_width / 2, 0.5f);
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 1, 1 << 6);
        if (hit.collider != null)
        {
            if (hit.distance > 0.3f) { return; }
            if (_tagList.Contains(hit.transform.tag))
            {
                if (_height - _transform.position.y >= _breakHeight)
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

    private void isHold()
    {
        if(_playerTransform == null) { return; }

        var pos = _rb.position;
        pos.x = _playerTransform.position.x;
        pos += (Vector2)_playerTransform.right * 1;
        _rb.position = pos;
    }

    public void Hold(Transform t)
    {
        _playerTransform = t;
    }
}

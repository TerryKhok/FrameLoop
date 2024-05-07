using System.Runtime.CompilerServices;
using UnityEngine;

public class InstanceCollider : MonoBehaviour
{
    private Transform _transform = null;
    private Camera _camera = null;
    private BoxCollider2D _collider = null;

    private void OnEnable()
    {
        _camera = Camera.main;
        _transform = transform;
        _collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        var screenPos = _camera.WorldToScreenPoint(_transform.position);
        Ray ray = _camera.ScreenPointToRay(screenPos);
        LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10, mask);

        _collider.isTrigger = hit.collider != null;
    }
}

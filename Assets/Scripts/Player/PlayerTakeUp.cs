using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTakeUp : MonoBehaviour
{
    private bool _takeUpFg = false;
    private Transform _transform = null;
    private Collider _collider = null;

    private Rigidbody _box = null;

    private void Start()
    {
        _transform = PlayerInfo.Instance.g_transform; 
        _collider = PlayerInfo.Instance.g_collider;
    }

    private void Update()
    {
        takeUp();
        adjustPos();
    }

    public void TakeUp(InputAction.CallbackContext context)
    {
        _takeUpFg |= context.performed;
    }

    private void takeUp()
    {
        if (!_takeUpFg) { return; }

        _takeUpFg = false;
        Ray ray = new Ray(_transform.position,_transform.forward);
        RaycastHit hit;
        Vector3 size = new Vector3(0.3f, 0.5f, 0.5f);

        if (_box == null)
        {
            if (Physics.BoxCast(
                ray.origin,
                size,
                ray.direction,
                out hit,
                Quaternion.identity,
                0.5f,
                ~(1 << 3),
                QueryTriggerInteraction.Ignore)
            )
            {
                if (hit.transform.CompareTag("Box"))
                {
                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    if (rb == null) { return; }

                    _box = rb;
                    rb.isKinematic = true;
                    _box.transform.SetParent(_transform);
                    _box.transform.localPosition = new Vector3(0, 0, 1);
                    Physics.IgnoreCollision(_box.GetComponent<Collider>(),_collider,true);
                }
            }
        }
        else
        {
            if (Physics.BoxCast(
                ray.origin,
                size,
                ray.direction,
                out hit,
                Quaternion.identity,
                1f,
                ~(1 << 3 | 1 << 7),
                QueryTriggerInteraction.Ignore)
            ){ Debug.Log(hit.transform.name); return; }
            else
            {
                _box.transform.SetParent(null);
                _box.isKinematic = false;
                _box.AddForce(_transform.forward, ForceMode.VelocityChange);
                Physics.IgnoreCollision(_box.GetComponent<Collider>(), _collider, false);
                _box = null;
            }
        }
    }

    private void adjustPos()
    {
        if(_box == null) { return; }
        if (FrameLoop.Instance.g_isActive) { return; }
        _box.transform.localPosition = new Vector3(0, 0, 1);
    }
}

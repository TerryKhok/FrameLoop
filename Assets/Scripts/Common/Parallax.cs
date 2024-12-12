using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform _cameraTransform;
    private Transform _transform;
    [SerializeField]
    private float _strength = 0.0f;

    private Vector3 _prevCamPos = Vector3.zero , _currentCamPos = Vector3.zero;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _transform = transform;
        _currentCamPos = _cameraTransform.position;
    }

    private void Update()
    {
        _prevCamPos = _currentCamPos;

        _currentCamPos = _cameraTransform.position;

        var vec = (_currentCamPos - _prevCamPos) * _strength;

        var currentPos = _transform.position;
        currentPos += vec;

        _transform.position = currentPos;
    }

    private void OnEnable()
    {
        _cameraTransform = Camera.main.transform;

        _prevCamPos = _cameraTransform.position;

        _currentCamPos = _cameraTransform.position;
    }
}

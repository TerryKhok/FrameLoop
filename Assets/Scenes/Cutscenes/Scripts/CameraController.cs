using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [System.Serializable]
    public struct CameraInfo
    {
        public float size;
        public float rotateZ;
        public Vector2 targetPosition;
        public float time;
        public bool shake;
    }

    [SerializeField]
    private List<CameraInfo> _cameraInfo = new List<CameraInfo>();

    private Vector3 _prevPos = Vector3.zero;
    private Vector3 _positionGap = Vector3.zero;
    private float _prevSize = 0.0f;
    private float _sizeGap = 0.0f;
    private float _prevRotationZ = 0.0f;
    private float _rotateGap = 0.0f;

    private Transform _transform;
    private Camera _camera;
    private int _index = -1;
    private float _elapsedTime = 0.0f;
    private bool _isShaking = false;

    private void Start()
    {
        _transform = transform;
        _camera = _transform.GetComponent<Camera>();

        Next();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_cameraInfo[_index].time <= _elapsedTime)
        {
            Next();
        }

        if (_isShaking) { return; }

        float timeRatio = 0;
        if (_cameraInfo[_index].time != 0)
        {
            timeRatio = _elapsedTime / _cameraInfo[_index].time;
        }
        else
        {
            timeRatio = 1.0f;
        }
        _transform.position = _prevPos + _positionGap * timeRatio;
        _camera.orthographicSize = _prevSize + _sizeGap * timeRatio;
        _transform.eulerAngles = new Vector3(0,0,_prevRotationZ + _rotateGap * timeRatio);
    }

    private void FixedUpdate()
    {
        if (_isShaking)
        {
            float randX = UnityEngine.Random.Range(-1.0f, 1.0f);
            float randY = UnityEngine.Random.Range(-1.0f, 1.0f);
            Vector3 rand = new Vector3(randX, randY, 0);
            _transform.position = _prevPos + Vector3.Scale(_positionGap, rand);
            _camera.orthographicSize = _prevSize + _sizeGap * randX;
            _transform.eulerAngles = new Vector3(0, 0, _prevRotationZ + _rotateGap * randX);
            return;
        }
    }

    private void Next()
    {
        _elapsedTime = 0.0f;
        _index++;

        _prevPos = _transform.position;

        var target = new Vector3(_cameraInfo[_index].targetPosition.x, _cameraInfo[_index].targetPosition.y, -10);
        _positionGap = target - _prevPos;
        _prevSize = _camera.orthographicSize;
        _sizeGap = _cameraInfo[_index].size - _prevSize;

        _isShaking = _cameraInfo[_index].shake;
    }
}

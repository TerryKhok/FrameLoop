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
        public Vector2 shakeRange;
        public float time;
        public bool shake;
        public float shakeHz;
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

    private Vector3 _factPos = Vector3.zero;
    private float _factSize = 0.0f;
    private float _factRotationZ = 0.0f;

    private float _shakeElapsed, _shakeTime;

    private void Start()
    {
        _transform = transform;
        _camera = _transform.GetComponent<Camera>();

        _factPos = _transform.position;
        _factRotationZ = _transform.eulerAngles.z;
        _factSize = _camera.orthographicSize;

        Next();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_cameraInfo[_index].time <= _elapsedTime)
        {
            Next();
        }

        float timeRatio = 0;
        if (_cameraInfo[_index].time != 0)
        {
            timeRatio = _elapsedTime / _cameraInfo[_index].time;
        }
        else
        {
            timeRatio = 1.0f;
        }
        _factPos = _prevPos + _positionGap * timeRatio;
        _factSize = _prevSize + _sizeGap * timeRatio;
        _factRotationZ = _prevRotationZ + _rotateGap * timeRatio;

        _transform.position = _factPos;
        _transform.eulerAngles = new Vector3(0, 0, _factRotationZ);
        _camera.orthographicSize = _factSize;

        if (_isShaking) 
        {
            _shakeElapsed += Time.deltaTime;

            if(_shakeElapsed >= _shakeTime)
            {
                _shakeElapsed = 0;

                float randX = UnityEngine.Random.Range(-1.0f, 1.0f);
                float randY = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 rand = new Vector3(randX, randY, 0);
                _transform.position = _factPos + Vector3.Scale(_cameraInfo[_index].shakeRange, rand);
            }
        }

    }

    private void Next()
    {
        _elapsedTime = 0.0f;
        _index++;

        _transform.position = _factPos;
        _prevPos = _factPos;

        var target = new Vector3(_cameraInfo[_index].targetPosition.x, _cameraInfo[_index].targetPosition.y, -10);
        _positionGap = target - _prevPos;
        _prevSize = _factSize;
        _sizeGap = _cameraInfo[_index].size - _prevSize;
        _prevRotationZ = _factRotationZ;
        _rotateGap = _cameraInfo[_index].rotateZ - _prevRotationZ;

        _isShaking = _cameraInfo[_index].shake;
        _shakeElapsed = 0.0f;
        _shakeTime = 1.0f / _cameraInfo[_index].shakeHz;
    }
}

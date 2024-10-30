using System;
using UnityEngine;
using UnityEngine.UI;

public class CircleWipeController : MonoBehaviour
{
    [SerializeField]
    private Material _circleWipeMaterial;
    [SerializeField]
    private float _duration = 1.0f;
    [SerializeField]
    private Image _circleWipeImage;

    private Transform _playerTransform = null;
    private Camera _camera = null;
    private float _time = 0f;
    private bool _isTransitioning = false, _isSceneTransition = false;

    public float g_cutoff = 0.0f;

    private void Start()
    {
        SetFloatEaseInOut(_circleWipeMaterial,"_cutoff", 0);
        _playerTransform = PlayerInfo.Instance.g_transform;
        _camera = Camera.main;

        BeginTransition();
    }

    private void Update()
    {
        if (_isTransitioning)
        {
            _circleWipeImage.enabled = true;
            Vector2 playerViewportPos = Vector2.zero;
            if(_playerTransform != null )
            {
                playerViewportPos = _camera.WorldToViewportPoint(_playerTransform.position);
            }
            _circleWipeMaterial.SetVector("_center", playerViewportPos);

            if (_isSceneTransition)
            {
                _time -= Time.unscaledDeltaTime;
                g_cutoff = Mathf.Clamp01(_time / _duration);
                SetFloatEaseInOut(_circleWipeMaterial, "_cutoff", g_cutoff);

                if (_time < 0)
                {
                    SetFloatEaseInOut(_circleWipeMaterial, "_cutoff", 0);
                    _isTransitioning = false;
                }
            }
            else
            {
                _time += Time.unscaledDeltaTime;
                g_cutoff = Mathf.Clamp01(_time / _duration);
                if(g_cutoff < 0)
                {
                    g_cutoff = 0;
                }
                SetFloatEaseInOut(_circleWipeMaterial, "_cutoff", g_cutoff);

                if (_time > _duration)
                {
                    _circleWipeImage.enabled = false;
                    _isTransitioning = false;
                }
            }
        }
    }

    public void BeginTransition(bool sceneTransition = false)
    {
        _isSceneTransition = sceneTransition;

        if (sceneTransition)
        {
            _time = _duration;
            _isTransitioning = true;
        }
        else
        {
            _time = -0.4f;
            _isTransitioning = true;
        }
    }

    public void SetProgress(float progress)
    {
        if (_isTransitioning) { return; }

        Vector2 playerViewportPos = Vector2.zero;
        if (_playerTransform != null)
        {
            playerViewportPos = _camera.WorldToViewportPoint(_playerTransform.position);
        }
        _circleWipeMaterial.SetVector("_center", playerViewportPos);

        if (progress > 0)
        {
            _time = _duration - progress * (1 / _duration);
            _circleWipeImage.enabled = true;
        }
        else
        {
            _time += _duration * 2 * Time.deltaTime;
        }

        float cutoff = Mathf.Clamp01(_time / _duration);

        SetFloatEaseInOut(_circleWipeMaterial, "_cutoff", cutoff, 12, 1);

        if (_time > _duration)
        {
            _circleWipeImage.enabled = false;
            _isTransitioning = false;
        }
    }

    public float GetDuration()
    {
        return _duration;
    }

    private void SetFloatEaseInOut(Material mat, string property, float t, float easeInFactor = 2.0f, float easeOutFactor = 2.0f)
    {
        float factor = (1-t) * easeInFactor + t * easeOutFactor;

        float set = Mathf.Pow(t, factor) * (3 - 2 * t);
        mat.SetFloat(property, set);
    }
}
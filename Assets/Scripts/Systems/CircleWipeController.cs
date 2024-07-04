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

    private void Start()
    {
        _circleWipeMaterial.SetFloat("_cutoff", 0);
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
                float cutoff = Mathf.Clamp01(_time / _duration);
                _circleWipeMaterial.SetFloat("_cutoff", cutoff);

                if (_time < 0)
                {
                    _circleWipeMaterial.SetFloat("_cutoff", 0);
                    _isTransitioning = false;
                }
            }
            else
            {
                _time += Time.unscaledDeltaTime;
                float cutoff = Mathf.Clamp01(_time / _duration);
                if(cutoff < 0)
                {
                    cutoff = 0;
                }
                _circleWipeMaterial.SetFloat("_cutoff", cutoff);

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

        _circleWipeMaterial.SetFloat("_cutoff", cutoff);
    }

    public float GetDuration()
    {
        return _duration;
    }
}
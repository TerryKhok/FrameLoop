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
    private float _time = 0f;
    private bool _isTransitioning = false, _isSceneTransition = false;

    private void Start()
    {
        _circleWipeMaterial.SetFloat("_cutoff", 0);

        BeginTransition();
    }

    private void Update()
    {
        if (_isTransitioning)
        {
            _circleWipeImage.enabled = true;
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

        Vector2 playerPos = PlayerInfo.Instance.g_transform.position;
        Vector2 playerViewPortPos = Camera.main.WorldToViewportPoint(playerPos);

        _circleWipeMaterial.SetVector("_center", playerViewPortPos);

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

    public float GetDuration()
    {
        return _duration;
    }
}
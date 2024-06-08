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
    private bool _isTransitioning = false;

    private void Start()
    {
        _circleWipeMaterial.SetFloat("_cutoff", 0);

        Vector2 playerPos =  PlayerInfo.Instance.g_transform.position;
        Vector2 playerViewPortPos = Camera.main.WorldToViewportPoint(playerPos);

        _circleWipeMaterial.SetVector("_center", playerViewPortPos);

        BeginTransition();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            BeginTransition();
        }

        if (_isTransitioning)
        {
            _time += Time.deltaTime;
            float cutoff = Mathf.Clamp01(_time / _duration);
            _circleWipeMaterial.SetFloat("_cutoff", cutoff);

            if(_time > _duration)
            {
                _circleWipeImage.enabled = false;
            }
        }
    }

    public void BeginTransition()
    {
        _time = 0f;
        _isTransitioning = true;
    }
}
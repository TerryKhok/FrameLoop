using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [System.Serializable]
    public struct Timing
    {
        public float start;
        public float fadeTime;
    }

    [SerializeField]
    private Image _fadeImage = null;

    [SerializeField]
    private List<Timing> _timingList = new List<Timing>();

    private float _elapsedTime = 0;
    private int _index = 0;
    private bool _fading = false;

    private void Update()
    {
        if(_index >= _timingList.Count)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;
        Debug.Log(_elapsedTime);

        if(_elapsedTime >= _timingList[_index].start)
        {
            _fading = true;
        }

        if(_elapsedTime >= (_timingList[_index].start + _timingList[_index].fadeTime))
        {
            _fading = false;
            _elapsedTime = 0;
            _index++;
        }

        Fade();
    }

    private void Fade()
    {
        if(_fading == false)
        {
            return;
        }

        float currentElementElapsed = _elapsedTime - _timingList[_index].start;
        float timeRatio = currentElementElapsed / _timingList[_index].fadeTime;

        float concentration = 1 - Mathf.Abs(timeRatio - 0.5f) * 2;
        concentration *= 1.5f;

        float a = 255.0f * concentration;
        a = Mathf.Clamp(a, 0, 255);

        Color32 color = _fadeImage.color;
        color.a = (byte)a;
        _fadeImage.color = color;
    }
}

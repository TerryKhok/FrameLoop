using UnityEngine;
using UnityEngine.UI;

public class ColorSync : MonoBehaviour
{
    private Image _parentImage = null;
    private Color _color;

    private Image _childImage = null;

    private void Start()
    {
        _parentImage = GetComponent<Image>();
        _color = _parentImage.color;

        _childImage = transform.GetChild(0).GetComponent<Image>();
        _childImage.color = _color;
    }

    private void Update()
    {
        _color = _parentImage.color;
        _childImage.color = _color;
    }
}

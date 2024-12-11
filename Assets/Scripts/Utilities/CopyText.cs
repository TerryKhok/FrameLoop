using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyText : MonoBehaviour
{
    [SerializeField]
    private string _text = null;

    [SerializeField]
    private List<TextMeshProUGUI> _textmeshList = new List<TextMeshProUGUI>();

    private void Start()
    {
        foreach(var t in _textmeshList)
        {
            t.text = _text;
        }
    }
}

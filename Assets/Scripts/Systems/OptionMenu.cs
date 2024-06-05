using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject OptionUI;

    private void Start()
    {
        OptionUI.SetActive(false);
    }

    public void Open()
    {
        OptionUI.SetActive(true);
    }

    public void Close()
    {
        OptionUI.SetActive(false);
    }
}

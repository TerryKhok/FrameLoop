using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GetStageName : MonoBehaviour
{
    TMP_Text _stageName;
    // Start is called before the first frame update
    void Start()
    {
        _stageName = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _stageName.text = SceneManager.GetActiveScene().name;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppreciateScene : MonoBehaviour
{
    private float _elapsedTime = 0;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if(_elapsedTime > 5)
        {
            if(_animator == null) { return; }
            _animator.SetBool("fade", true);
        }
    }

    public void ReturnMain()
    {
        SceneManager.LoadScene(0);
    }
}

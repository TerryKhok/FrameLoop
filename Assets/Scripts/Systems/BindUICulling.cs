using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BindUICulling : MonoBehaviour
{
    [SerializeField]
    private bool _autoCulling = true;

    private const int _indexMove = 1;
    private const int _indexFrame = 2;
    private const int _indexCrouch = 4;
    private const int _indexHold = 8;

    private void Start()
    {
        if (!_autoCulling) { return; }

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        bool cullingFlag = false;

        for(int i=0; i < transform.childCount; ++i)
        {
            //カリングフラグが立っていないときはカリングの判定を行う
            if (!cullingFlag)
            {
                if ((sceneIndex < _indexMove && 2 < i) ||
                    (sceneIndex < _indexFrame && 4 < i) ||
                    (sceneIndex < _indexCrouch && 5 < i) ||
                    (sceneIndex < _indexHold && 6 < i))
                {
                    cullingFlag = true;
                }
            }

            if(cullingFlag)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}

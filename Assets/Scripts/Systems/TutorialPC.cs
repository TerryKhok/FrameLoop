using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :TutorialPC
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :チュートリアルのUIを表示する
 *               
 *  Created     :2024/06/26
 */
public class TutorialPC : MonoBehaviour
{
    [SerializeField]
    private RectTransform _tutorialRect = null;
    private Animator _animator = null;
    [SerializeField]
    private Vector3 _offset = Vector3.zero;

    private void Start()
    {
        _tutorialRect.position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + _offset);
        _animator = _tutorialRect.GetComponent<Animator>();
    }

    public void SetActive(bool active)
    {
        _tutorialRect.gameObject.SetActive(active);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(_animator == null || PlayerInfo.Instance == null || this.enabled == false)
        {
            return;
        }

        if (collision == PlayerInfo.Instance.g_goalHitBox)
        {
            _animator.SetBool("IsOpen", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_animator == null || PlayerInfo.Instance == null || this.enabled == false)
        {
            return;
        }

        if (collision == PlayerInfo.Instance.g_goalHitBox)
        {
            _animator.SetBool("IsOpen", false);
        }
    }

    private void Open()
    {

    }
}

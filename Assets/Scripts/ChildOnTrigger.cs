using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :ChildOnTrigger
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :親オブジェクトのIParentOnTriggerのメソッドを実行する
 *               
 *  Created     :2024/04/27
 */
public class ChildOnTrigger : MonoBehaviour
{
    private IParentOnTrigger _parentOnTrigger = null;
    private Transform _transform = null;

    private void OnEnable()
    {
        _transform = transform;

        //IParentOnTriggerを親オブジェクトから取得
        _parentOnTrigger = _transform.GetComponentInParent<IParentOnTrigger>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(_parentOnTrigger == null) { return; }
        _parentOnTrigger.OnEnter(other,_transform);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if(_parentOnTrigger == null) { return; }
        _parentOnTrigger.OnStay(other, _transform);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(_parentOnTrigger == null) { return; }
        _parentOnTrigger.OnExit(other, _transform);
    }
}

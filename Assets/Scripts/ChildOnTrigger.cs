using System;
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
    //[SerializeField]
    //private bool _useCustomTrigger = false;

    private IParentOnTrigger _parentOnTrigger = null;
    private Transform _transform = null;

    private BoxCollider2D _hitBoxCollider = null;

    private void OnEnable()
    {
        _transform = transform;

        //IParentOnTriggerを親オブジェクトから取得
        _parentOnTrigger = _transform.GetComponentInParent<IParentOnTrigger>();
    }
#if false
    private void Start()
    {
        if(_useCustomTrigger)
        {
            _hitBoxCollider = GetComponent<BoxCollider2D>();
            if( _hitBoxCollider == null )
            {
                Debug.LogError("useCustomTriggerがtrueの場合、ColliderはBoxCollider2Dである必要があります");
            }
        }
    }

    private void FixedUpdate()
    {
        if (_useCustomTrigger)
        {
            CheckOverlap();
        }
    }

    private void CheckOverlap()
    {
        Vector2 point = transform.position + (Vector3)_hitBoxCollider.offset;  // ボックスの中心をオブジェクトの位置に設定
        Vector2 size = transform.localScale * _hitBoxCollider.size; // ボックスのサイズ
        int layerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);

        Collider2D[] results = Physics2D.OverlapBoxAll(point, size, 0, layerMask);

        foreach (Collider2D collider in results)
        {
            Rigidbody2D rb = null;

            if (!collider.TryGetComponent<Rigidbody2D>(out rb))
            {
                return;
            }

            if (rb.bodyType == RigidbodyType2D.Static)
            {
                return;
            }

            if (_parentOnTrigger == null) { return; }
            _parentOnTrigger.OnStay(collider, _transform);
        }
    }

#endif
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (_parentOnTrigger == null) { return; }
        
        _parentOnTrigger.OnEnter(other,_transform);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (_parentOnTrigger == null) { return; }

        _parentOnTrigger.OnStay(other, _transform);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(_parentOnTrigger == null) { return; }

        _parentOnTrigger.OnExit(other, _transform);
    }
}

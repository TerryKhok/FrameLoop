using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Conveyor
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :乗っているオブジェクトを等速で移動させる
 *               
 *  Created     :2024/04/27
 */
[RequireComponent(typeof(BoxCollider2D))]
public class Conveyor : MonoBehaviour
{
    [SerializeField,Tooltip("回転速度(m/s)")]
    private float _velocity = 1f;
    [SerializeField, Tag, Tooltip("影響を与えるTag")]
    private List<string> _tagList = new List<string>() { "Player" };
    [SerializeField,Tooltip("逆回転させる")]
    private bool _inverse = false;
    private BoxCollider2D _boxCollider = null;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;

        //影響範囲を一定の値分縦に大きくする
        var size = _boxCollider.size;
        size.y += 0.3f;
        _boxCollider.size = size;

        //影響範囲を一定の値分上にずらす
        var offset = _boxCollider.offset;
        offset.y += 0.15f;
        _boxCollider.offset = offset;
    }

    //影響を与えるTagのオブジェクトが触れたら等速で移動させる
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        var rb = other.GetComponent<Rigidbody2D>();
        if(rb == null) {  return; }

        //Rigidbodyの座標を移動
        var pos = rb.position;
        if (_inverse) { pos.x -= _velocity*Time.fixedDeltaTime; }
        else { pos.x += _velocity*Time.fixedDeltaTime; }

        rb.position = pos;
    }

    //引数によって回転方向を定める
    //true  :右回転
    //false :左回転
    public void SetInverse(bool inverse)
    {
        _inverse = inverse;
    }

    //回転方向を逆にする
    public void FripDirection()
    {
        _inverse = !_inverse;
    }
}

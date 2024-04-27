using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Conveyor
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :����Ă���I�u�W�F�N�g�𓙑��ňړ�������
 *               
 *  Created     :2024/04/27
 */
[RequireComponent(typeof(BoxCollider2D))]
public class Conveyor : MonoBehaviour
{
    [SerializeField,Tooltip("��]���x(m/s)")]
    private float _velocity = 1f;
    [SerializeField, Tag, Tooltip("�e����^����Tag")]
    private List<string> _tagList = new List<string>() { "Player" };
    [SerializeField,Tooltip("�t��]������")]
    private bool _inverse = false;
    private BoxCollider2D _boxCollider = null;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;

        //�e���͈͂����̒l���c�ɑ傫������
        var size = _boxCollider.size;
        size.y += 0.3f;
        _boxCollider.size = size;

        //�e���͈͂����̒l����ɂ��炷
        var offset = _boxCollider.offset;
        offset.y += 0.15f;
        _boxCollider.offset = offset;
    }

    //�e����^����Tag�̃I�u�W�F�N�g���G�ꂽ�瓙���ňړ�������
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        var rb = other.GetComponent<Rigidbody2D>();
        if(rb == null) {  return; }

        //Rigidbody�̍��W���ړ�
        var pos = rb.position;
        if (_inverse) { pos.x -= _velocity*Time.fixedDeltaTime; }
        else { pos.x += _velocity*Time.fixedDeltaTime; }

        rb.position = pos;
    }

    //�����ɂ���ĉ�]�������߂�
    //true  :�E��]
    //false :����]
    public void SetInverse(bool inverse)
    {
        _inverse = inverse;
    }

    //��]�������t�ɂ���
    public void FripDirection()
    {
        _inverse = !_inverse;
    }
}

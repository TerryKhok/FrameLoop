using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Conveyor
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :æ‚Á‚Ä‚¢‚éƒIƒuƒWƒFƒNƒg‚ğ“™‘¬‚ÅˆÚ“®‚³‚¹‚é
 *               
 *  Created     :2024/04/27
 */
[RequireComponent(typeof(BoxCollider2D))]
public class Conveyor : MonoBehaviour
{
    [SerializeField,Tooltip("‰ñ“]‘¬“x(m/s)")]
    private float _velocity = 1f;
    [SerializeField, Tag, Tooltip("‰e‹¿‚ğ—^‚¦‚éTag")]
    private List<string> _tagList = new List<string>() { "Player" };
    [SerializeField,Tooltip("‹t‰ñ“]‚³‚¹‚é")]
    private bool _inverse = false;
    private BoxCollider2D _boxCollider = null;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;

        //‰e‹¿”ÍˆÍ‚ğˆê’è‚Ì’l•ªc‚É‘å‚«‚­‚·‚é
        var size = _boxCollider.size;
        size.y += 0.3f;
        _boxCollider.size = size;

        //‰e‹¿”ÍˆÍ‚ğˆê’è‚Ì’l•ªã‚É‚¸‚ç‚·
        var offset = _boxCollider.offset;
        offset.y += 0.15f;
        _boxCollider.offset = offset;
    }

    //‰e‹¿‚ğ—^‚¦‚éTag‚ÌƒIƒuƒWƒFƒNƒg‚ªG‚ê‚½‚ç“™‘¬‚ÅˆÚ“®‚³‚¹‚é
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_tagList.Contains(other.tag)) { return; }

        var rb = other.GetComponent<Rigidbody2D>();
        if(rb == null) {  return; }

        //Rigidbody‚ÌÀ•W‚ğˆÚ“®
        var pos = rb.position;
        if (_inverse) { pos.x -= _velocity*Time.fixedDeltaTime; }
        else { pos.x += _velocity*Time.fixedDeltaTime; }

        rb.position = pos;
    }

    //ˆø”‚É‚æ‚Á‚Ä‰ñ“]•ûŒü‚ğ’è‚ß‚é
    //true  :‰E‰ñ“]
    //false :¶‰ñ“]
    public void SetInverse(bool inverse)
    {
        _inverse = inverse;
    }

    //‰ñ“]•ûŒü‚ğ‹t‚É‚·‚é
    public void FripDirection()
    {
        _inverse = !_inverse;
    }
}

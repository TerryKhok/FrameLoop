using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour
{
    private enum Type
    {
        water,lava,acid
    }
    [SerializeField,Tooltip("���͉��̈Ӗ����Ȃ�")]
    private Type _type;
    [SerializeField,Tag,Tooltip("�j��\��Tag")]
    private List<string> _tagList = new List<string>() { "Player"};

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_tagList.Contains(collision.tag))
        {
            Destroy(collision.gameObject);
        }
    }
}

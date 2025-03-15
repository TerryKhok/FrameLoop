using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour, IParentOnTrigger
{
    private enum Type
    {
        water,lava,acid
    }
    [SerializeField,Tooltip("今は何の意味もない")]
    private Type _type;
    [SerializeField,Tag,Tooltip("破壊可能なTag")]
    private List<string> _tagList = new List<string>() { "Player"};

    public void OnEnter(Collider2D other, Transform transform)
    {
        if (_tagList.Contains(other.tag))
        {
            AudioManager.instance.Play("PlayerDie");
            Destroy(other.gameObject);
        }
    }
    public void OnStay(Collider2D other, Transform transform)
    {

    }
    public void OnExit(Collider2D other, Transform transform)
    {

    }
}

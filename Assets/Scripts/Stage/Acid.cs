using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour, IParentOnTrigger
{
    private enum Type
    {
        water,lava,acid
    }
    [SerializeField,Tooltip("ç°ÇÕâΩÇÃà”ñ°Ç‡Ç»Ç¢")]
    private Type _type;
    [SerializeField,Tag,Tooltip("îjâÛâ¬î\Ç»Tag")]
    private List<string> _tagList = new List<string>() { "Player"};

    public void OnEnter(Collider2D other, Transform transform)
    {
        if (_tagList.Contains(other.tag))
        {
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

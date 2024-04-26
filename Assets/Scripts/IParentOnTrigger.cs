using UnityEngine;

public interface IParentOnTrigger
{
    public void OnEnter(Collider2D other, Transform transform);
    public void OnStay(Collider2D other, Transform transform);
    public void OnExit(Collider2D other, Transform transform);
}

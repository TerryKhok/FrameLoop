using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParentOnTrigger
{
    public void OnEnter(Collider2D collision, Transform transform);
    public void OnStay(Collider2D collision, Transform transform);
    public void OnExit(Collider2D collision, Transform transform);
}

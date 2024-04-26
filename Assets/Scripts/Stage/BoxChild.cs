using UnityEngine;

public class BoxChild : MonoBehaviour,IBox
{
    private Box _box = null;

    private void OnEnable()
    {
        _box = GetComponentInParent<Box>();
    }

    public void Hold(Transform t)
    {
        _box.Hold(t);
    }

}

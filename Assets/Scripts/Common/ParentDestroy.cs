using UnityEngine;

public class ParentDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}

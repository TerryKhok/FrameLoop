using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [SerializeField,Tooltip("â°ïù")]
    private int _width = 1;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private void OnValidate()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        _spriteRenderer.size = new Vector2(_width,1);
        _boxCollider.size = new Vector2(_width, 1);

        transform.localScale = Vector3.one;
    }

    public void Break()
    {
        Destroy(gameObject);
    }
}

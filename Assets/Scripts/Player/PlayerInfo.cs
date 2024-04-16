using UnityEngine;

public class PlayerInfo : SingletonMonoBehaviour<PlayerInfo>
{
    [SerializeField,Tooltip("プレイヤーが着地できるレイヤー")]
    LayerMask _platformLayer;

    [HideInInspector]
    public Rigidbody2D g_rb = null;
    [HideInInspector]
    public BoxCollider2D g_collider = null;
    [HideInInspector]
    public bool g_isGround = true;
    [HideInInspector]
    public Transform g_transform = null;
    [HideInInspector]
    public bool g_takeUpFg = false;
    [HideInInspector]
    public float g_wall = 0;
    [HideInInspector]
    public Transform g_box = null;

    private void Start()
    {
        g_rb = GetComponent<Rigidbody2D>();
        g_collider = GetComponent<BoxCollider2D>();
        g_transform = transform;
    }

    private void Update()
    {
        Ray ray = new Ray(g_transform.position, Vector3.down);
        var size = new Vector2(g_collider.size.x, 0.5f);
        RaycastHit2D hit;
        //Debug.DrawRay(ray.origin, ray.direction * 1, Color.red, 0.1f);
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 1, _platformLayer);

        if (hit.collider != null)
        {
            //Debug.Log(hit.distance);
            if(hit.distance < 0.6f)
            {
                //Debug.Log($"{hit.distance}{hit.transform.name}");
                g_isGround = true;
                return;
            }
        }
        g_isGround = false;
    }
}

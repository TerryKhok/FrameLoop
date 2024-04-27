using System;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerInfo
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Player�̏��𓝊�����N���X
 *               
 *  Created     :2024/04/27
 */
public class PlayerInfo : SingletonMonoBehaviour<PlayerInfo>
{
    [SerializeField,Tooltip("�v���C���[�����n�ł��郌�C���[")]
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
    [HideInInspector]
    public float g_groundDistance = 0;

    private const float Ground_Dist = 0.72f;
    private string _layermaskValue;
    [HideInInspector]
    public LayerMask g_insideMask, g_outsideMask;

    private void Start()
    {
        g_rb = GetComponent<Rigidbody2D>();
        g_collider = GetComponent<BoxCollider2D>();
        g_transform = transform;

        //���̃��C���[���i���ɕϊ�
        _layermaskValue = Convert.ToString(_platformLayer.value,2);

        //����̃��C���[������p�ƊO���p�ɕ�����
        for (int i=0,j=_layermaskValue.Length-1; i < _layermaskValue.Length; i++,j--)
        {
            if (_layermaskValue[i] == '1')
            {
                //������̐擪��I���ǂ����Ŕ���
                if (LayerMask.LayerToName(j)[0] == 'I')
                {
                    g_insideMask |= 1 << j;
                }
                else
                {
                    g_outsideMask |= 1 << j;
                }
            }
        }
    }

    private void Update()
    {

        Ray ray = new Ray(g_transform.position, Vector3.down);
        var size = new Vector2(g_collider.size.x, 0.5f);
        RaycastHit2D hit;

        LayerMask mask = 0;

        //�t���[�����L�����ǂ�����LayerMask��Layer��ύX
        if (FrameLoop.Instance.g_isActive)
        {
            mask = g_insideMask;
            gameObject.layer = LayerMask.NameToLayer("IPlayer");
        }
        else
        {
            mask = g_outsideMask;
            gameObject.layer = LayerMask.NameToLayer("OPlayer");
        }

        //�����ɐݒu����
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 2, mask);
        if (hit.collider != null)
        {
            //Debug.Log($"{hit.distance}{hit.transform.name}");

            //�n�ʂƂ̋������X�V
            g_groundDistance = hit.distance;

            if (hit.distance < Ground_Dist)
            {
                g_isGround = true;
                return;
            }
        }
        else
        {
            g_groundDistance = Mathf.Infinity;
        }
        g_isGround = false;
    }
}

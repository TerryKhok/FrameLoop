using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Bullet
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�e�̋������Ǘ�����N���X
 *               �����ł̈ړ��A�j��\�ȃ^�O�̃I�u�W�F�N�g��j��
 *               
 *  Created     :2024/04/27
 */
public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb = null;
    private Vector2 _direction = Vector2.zero;
    private float _velocity = 1f;
    private float _lifeSpan = 1f;
    private float _elapsedTime = 0f;
    private List<string> _tagList = new List<string>();

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        //�����ňړ�
        _rb.position += _direction.normalized * _velocity * Time.fixedDeltaTime;

        //��莞�Ԍo�߂����玩�g��j��
        if(_lifeSpan < _elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //�j��\�ȃI�u�W�F�N�g�ɏՓ˂�����
        //����Ǝ��g��j�󂷂�
        if (_tagList.Contains(other.transform.tag))
        {
            Destroy(other.gameObject);
            return;
        }
        Destroy(this.gameObject);
    }

    //�O������ϐ����Z�b�g����
    public void SetValues(Vector2 direction, float velocity, float range, List<string> tagList)
    {
        _direction = direction;
        _velocity = velocity;
        _lifeSpan = range/velocity;
        _tagList = new List<string>(tagList);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Bullet
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :弾の挙動を管理するクラス
 *               等速での移動、破壊可能なタグのオブジェクトを破壊
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

        //等速で移動
        _rb.position += _direction.normalized * _velocity * Time.fixedDeltaTime;

        //一定時間経過したら自身を破壊
        if(_lifeSpan < _elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //破壊可能なオブジェクトに衝突したら
        //相手と自身を破壊する
        if (_tagList.Contains(other.transform.tag))
        {
            Destroy(other.gameObject);
            return;
        }
        Destroy(this.gameObject);
    }

    //外部から変数をセットする
    public void SetValues(Vector2 direction, float velocity, float range, List<string> tagList)
    {
        _direction = direction;
        _velocity = velocity;
        _lifeSpan = range/velocity;
        _tagList = new List<string>(tagList);
    }
}

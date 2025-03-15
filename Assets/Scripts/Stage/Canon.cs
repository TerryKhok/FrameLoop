using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Canon
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :弾を発射する
 *               
 *  Created     :2024/04/27
 */
public class Canon : MonoBehaviour
{
    [SerializeField,Tooltip("発射するPrefab")]
    private GameObject _bulletPrefab = null;
    [SerializeField, Tag,Tooltip("破壊可能なTag")]
    private List<string> _breakTag = new List<string>();
    [SerializeField,Tooltip("発射方向")]
    private Vector2 _direction = Vector2.zero;
    [SerializeField,Tooltip("発射速度")]
    private float _velocity = 1f;
    [SerializeField,Tooltip("射程")]
    private float _range = 1f;
    [SerializeField,Tooltip("発射間隔(s)")]
    private float _interval = 1f;
    [SerializeField,Tooltip("初めから有効か")]
    private bool _enabledOnAwake = true;

    private GameObject _instance = null;
    private Bullet _bullet = null;
    private float _elapsedTime = 0f;
    private Transform _transform = null;
    private Vector3 _position = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;
    private bool _enable = false;
    private bool _frameInside = false;

    private GameObject _colUpObj, _colLowObj;

    private void Start()
    {
        _colUpObj = transform.GetChild(0).GetChild(0).gameObject;
        _colLowObj = transform.GetChild(0).GetChild(1).gameObject;

        _enable = _enabledOnAwake;

        //発射位置は子オブジェクトの位置を参照する
        _transform = transform.GetChild(0);

        //弾の向きを発射方向に向かせる
        _rotation = Quaternion.FromToRotation(Vector3.right,_direction);

        //発射位置を発射方向に一定距離ずらす
        _position = _transform.position;
        _position += (Vector3)_direction.normalized;

        //最初から起動していたら発射する
        if (!_enable) { return; }
        InstantiateBullet();
    }

    private void Update()
    {
        if (!_enable) {  return; }

        _elapsedTime += Time.deltaTime;

        //一定時間毎に発射する
        if(_interval < _elapsedTime)
        {
            InstantiateBullet();
        }
    }

    //弾を発射する
    private void InstantiateBullet()
    {
        _instance = Instantiate(_bulletPrefab, _position, _rotation);
        _bullet = _instance.GetComponent<Bullet>();
        _bullet.SetValues(_direction, _velocity, _range, _breakTag);

        //生成位置がフレーム内で、フレームが有効な場合の処理
        if(FrameLoop.Instance.g_isActive && _frameInside)
        {
            //FrameのInsiderにBulletを追加
            FrameLoop.Instance.AddInsiders(_instance.GetComponent<Collider2D>());
        }

        //経過時間をリセット
        _elapsedTime = 0f;
    }

    //有効かどうかを引数で変更する
    public void SetEnable(bool enable)
    {
        _enable = enable;
    }

    //有効か無効かを反転させる
    public void SwitchEnable()
    {
        _enable = !_enable;
    }

    //フレームに重なっているかでレイヤーを変更する
    public void CanonLayerCheck()
    {
        Vector3 offset = new Vector3(0, 0.25f, 0);
        GameObject obj = _colUpObj;

        for (int i = 0; i < 2; i++)
        {
            //スクリーン座標に変換
            var pos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);

            //座標に位置にレイを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(pos);
            LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1.0f, mask);

            if (hit.collider != null)
            {
                obj.layer = LayerMask.NameToLayer("Inside");
                //発射位置がフレームの内側かを判定
                if(i == 0)
                {
                    _frameInside = true;
                }
            }
            else
            {
                obj.layer = LayerMask.NameToLayer("Outside");
                //発射位置がフレームの内側かを判定
                if (i == 0)
                {
                    _frameInside = false;
                }
            }

            obj = _colLowObj;
            offset.y -= 0.5f;
        }
    }
}

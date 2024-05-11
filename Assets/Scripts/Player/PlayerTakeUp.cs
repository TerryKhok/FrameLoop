using UnityEngine;
using UnityEngine.InputSystem;

/*  ProjectName :FrameLoop
 *  ClassName   :PlayerTakeUp
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :箱を掴む
 *               
 *  Created     :2024/04/27
 */
public class PlayerTakeUp : MonoBehaviour
{
    private Transform _transform = null;
    private PlayerInfo _playerInfo = null;
    private FrameLoop _frameLoop = null;
    private BoxCollider2D _boxCollider = null;
    private IBox _box = null;
    [SerializeField,Tooltip("切り替え")]
    private bool _toggle = false;

    private int _count = 0;
    private LayerMask _insideMask = 0, _outsideMask = 0;

    private PlayerAnimation _playerAnimation = null;

    private void Start()
    {
        _playerInfo = PlayerInfo.Instance;
        _frameLoop = FrameLoop.Instance;
        _playerAnimation = PlayerAnimation.Instance;
        _transform = _playerInfo.g_transform;
        _boxCollider = _playerInfo.g_collider;

        _insideMask = 1 << LayerMask.NameToLayer("IBox");
        _outsideMask = 1 << LayerMask.NameToLayer("OBox");
    }

    private void Update()
    {
        takeUp();
        _playerAnimation.SetHoldAnimation(_playerInfo.g_takeUpFg);
    }

    //箱があるかの判定と掴み
    public void TakeUpStarted(InputAction.CallbackContext context)
    {
        //切り替え操作の処理
        if(_toggle && _playerInfo.g_takeUpFg)
        {
            //箱を離す
            _box.Hold(null);
            return;
        }

        //プレイヤーの進行方向へのRay
        Ray ray = new Ray(_transform.position, _transform.right);
        RaycastHit2D hit;
        Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);

        LayerMask mask = 0;
        if (_frameLoop.g_isActive)
        {
            mask = _insideMask;
        }
        else
        {
            mask = _outsideMask;
        }

        //プレイヤーの進行方向に箱があるか判定
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, 1f,mask);
        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Box"))
            {
                //IBoxを継承したComponentを取得
                _box = hit.transform.GetComponent<IBox>();

                //箱を掴む
                _box.Hold(_transform);
                _playerInfo.g_boxDirection = (int)ray.direction.x;
            }
        }
    }

    //箱から手を離す
    public void TakeUpCanceled(InputAction.CallbackContext context)
    {
        //切り替え操作の場合はreturn
        if (_toggle) { return; }

        _box.Hold(null);
    }

    //箱を掴んでいる間の処理
    private void takeUp()
    {
        //箱を掴んでなかったらreturn
        if (!_playerInfo.g_takeUpFg) { return; }

        _playerInfo.g_takeUpFg = true;

        //進行方向へのRay
        Ray ray = new Ray(_transform.position, _transform.right);
        RaycastHit2D hit;

        Vector3 size = new Vector3(0.42f, 0.5f, 0.5f);
        float length = 1 + _boxCollider.size.x/2;


        //フレームが有効かどうかでLayerMaskを変更
        LayerMask mask = 1 << LayerMask.NameToLayer("OPlatform");
        if (_frameLoop.g_isActive)
        {
            mask = 1 << LayerMask.NameToLayer("IPlatform");
        }

        //壁があるかを判定
        hit = Physics2D.BoxCast(ray.origin, size, 0, ray.direction, length, mask);

        if(hit.collider != null)
        {
            //壁があったら壁の情報を更新
            _playerInfo.g_wall = _transform.right.normalized.x;
        }
        else
        {
            //壁が無かったら壁の情報をリセット
            _playerInfo.g_wall = 0;
        }
    }
}

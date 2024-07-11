using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/*  ProjectName :FrameLoop
 *  ClassName   :Button
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :ボタンの状態によってイベントを実行する
 *               
 *  Created     :2024/04/27
 */
[RequireComponent(typeof(BoxCollider2D))]
public class Button : MonoBehaviour
{
    private BoxCollider2D _collider = null;
    [SerializeField,Tooltip("Goalに必要か")]
    private bool _toGoal = false;
    [SerializeField, Tag ,Header("ボタンを押下可能なTag")]
    private List<string> _tagList = new List<string>();
    [SerializeField,Tooltip("押されたときに一度実行するメソッド")]
    private UnityEvent _onClick = null;
    [SerializeField,Tooltip("押されている間実行するメソッド")]
    private UnityEvent _onHold = null;
    [SerializeField,Tooltip("離したときに実行するメソッド")]
    private UnityEvent _onRelease = null;
    [SerializeField, Tooltip("押されている状態のsprite")]
    private Sprite _pushed;

    private Sprite _unpushed;

    private SpriteRenderer _renderer;
    private int _hitCount = 0;
    private bool _isPressed = false, _prevPressed = false;

    private bool pressedSoundFlag = false;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;

        _renderer = GetComponent<SpriteRenderer>();
        _unpushed = _renderer.sprite;
    }

    private void Update()
    {
        //前フレームでの状態を保存
        _prevPressed = _isPressed;

        //重なっているオブジェクトがあるかで状態を判定
        _isPressed = _hitCount > 0;


        if(_isPressed)
        {
            if (!pressedSoundFlag)
            {
                AudioManager.instance.Play("Button");
                AudioManager.instance.Play("DoorOpen");
                pressedSoundFlag = true;
            }

            _renderer.sprite = _pushed;

            if(_prevPressed)
            {
                //Debug.Log("hold");

                //押されている間実行する処理
                _onHold.Invoke();
            }
            else
            {
                //Debug.Log("click");

                //ゴールに必要なボタンならカウントアップ
                if (_toGoal)
                {
                    Goal.Instance.CountUp();
                }

                //押された時に一度実行する処理
                _onClick.Invoke();
            }
        }
        else
        {
            _renderer.sprite= _unpushed;

            if (pressedSoundFlag)
            {
                AudioManager.instance.Play("DoorClose");
                pressedSoundFlag = false;
            }

            if (_prevPressed)
            {
                //Debug.Log("release");

                //ゴールに必要なボタンならカウントダウン
                if (_toGoal)
                {
                    Goal.Instance.CountDown();
                }

                //離された時に一度実行する処理
                _onRelease.Invoke();
            }
        }
    }

    //フレームに重なっているかでレイヤーを変更する
    public void ButtonLayerCheck()
    {
        //スクリーン座標に変換
        var pos = Camera.main.WorldToScreenPoint(transform.position);

        //座標に位置にレイを飛ばす
        Ray ray = Camera.main.ScreenPointToRay(pos);
        LayerMask mask = 1 << LayerMask.NameToLayer("Frame");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10, mask);

        if (hit.collider != null)
        {
            gameObject.layer = LayerMask.NameToLayer("Inside");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Outside");
        }
    }

    //レイヤーを戻す
    public void SetOutsideLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Outside");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //ボタンを押せるタグのオブジェクトならカウントを増やす
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _hitCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //ボタンを押せるタグのオブジェクトならカウントを減らす
        if (!_tagList.Contains(other.tag))
        {
            return;
        }
        _hitCount--;
    }

    public bool IsToGoal()
    {
        return _toGoal;
    }
}

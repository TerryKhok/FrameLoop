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

    private int _hitCount = 0;
    private bool _isPressed = false, _prevPressed = false;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }

    private void Update()
    {
        //前フレームでの状態を保存
        _prevPressed = _isPressed;

        //重なっているオブジェクトがあるかで状態を判定
        _isPressed = _hitCount > 0;

        if(_isPressed)
        {
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
}

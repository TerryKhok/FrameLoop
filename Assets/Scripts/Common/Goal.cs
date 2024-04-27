using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Goal
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Goal判定をする
 *               
 *  Created     :2024/04/27
 */
public class Goal : SingletonMonoBehaviour<Goal>
{
    [SerializeField,Tooltip("クリア時に表示するキャンバス")]
    private Canvas _clearCanvas = null;
    [SerializeField,Tooltip("ゴールに必要なボタンの数")]
    private int _buttonCount = 0;
    private int _count = 0;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _clearCanvas.enabled = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //必要なボタンの数を超えているかで色を変更
        if(_count >= _buttonCount)
        {
            _spriteRenderer.color = new Color32(0, 255, 0, 150);
        }
        else
        {
            _spriteRenderer.color = new Color32(255, 0, 0, 150);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //ボタンの数が足りていたらゴール
        if(_count >= _buttonCount)
        {
            Debug.Log(other.transform.name);
            _clearCanvas.enabled = true;
        }
    }

    //ボタンの数を加算
    public void CountUp()
    {
        _count++;
    }

    //ボタンの数を減算
    public void CountDown()
    {
        _count--;
    }
}

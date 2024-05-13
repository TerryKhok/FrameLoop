using UnityEngine;
using UnityEngine.SceneManagement;

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
    private int _buttonCount = 0;
    private int _count = 0;
    private SpriteRenderer _spriteRenderer;

    [SerializeField,Tooltip("開いているドアのスプライト")]
    private Sprite _opendSprite = null;
    private Sprite _closedSprite = null;

    private void Start()
    {
        _clearCanvas.enabled = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _closedSprite = _spriteRenderer.sprite;

        var objs = GameObject.FindGameObjectsWithTag("Button");
        foreach( var obj in objs)
        {
            if (obj.GetComponent<Button>().IsToGoal())
            {
                _buttonCount++;
            }
        }
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

        //SetSprite(_count >= _buttonCount);
    }

    private void SetSprite(bool opened)
    {
        if (opened)
        {
            _spriteRenderer.sprite = _opendSprite;
        }
        else
        {
            _spriteRenderer.sprite = _closedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) { return; }

        //ボタンの数が足りていたらゴール
        if(_count >= _buttonCount)
        {
            _clearCanvas.enabled = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    //フレームに重なっているかでレイヤーを変更する
    public void GoalLayerCheck()
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

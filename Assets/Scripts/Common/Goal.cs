using UnityEngine;

public class Goal : MonoBehaviour
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
        if(_count >= _buttonCount)
        {
            _clearCanvas.enabled = true;
        }
    }

    public void CountUp()
    {
        _count++;
    }

    public void CountDown()
    {
        _count--;
    }
}

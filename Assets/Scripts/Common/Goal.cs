using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :Goal
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Goal���������
 *               
 *  Created     :2024/04/27
 */
public class Goal : SingletonMonoBehaviour<Goal>
{
    [SerializeField,Tooltip("�N���A���ɕ\������L�����o�X")]
    private Canvas _clearCanvas = null;
    [SerializeField,Tooltip("�S�[���ɕK�v�ȃ{�^���̐�")]
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
        //�K�v�ȃ{�^���̐��𒴂��Ă��邩�ŐF��ύX
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
        //�{�^���̐�������Ă�����S�[��
        if(_count >= _buttonCount)
        {
            Debug.Log(other.transform.name);
            _clearCanvas.enabled = true;
        }
    }

    //�{�^���̐������Z
    public void CountUp()
    {
        _count++;
    }

    //�{�^���̐������Z
    public void CountDown()
    {
        _count--;
    }
}

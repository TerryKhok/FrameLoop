using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField, Tooltip("�S�[���ɕK�v�ȃt���[���̎g�p��")]
    private int _minFrameCount = 1;

    private Canvas _clearCanvas = null;
    private int _buttonCount = 0;
    private int _count = 0;

    private Animator _animator;
    private bool _isOpened = false;

    private bool _clear = false;

    private int _frameCount = 0;

    private List<Animator> _starAnimators = new List<Animator>();
    private Animator _clearScreenAnimator = null;

    private PlayerInfo _playerInfo = null;
    private PlayerInput _playerInput;

    //�Œ�񐔁{����܂ł�2�ɂ��邩
    private const int STAR_GAP = 1;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _clearCanvas = GetComponentInChildren<Canvas>();
        _playerInfo = PlayerInfo.Instance;
        _playerInput = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerInput>();

        var animatorArray = GetComponentsInChildren<Animator>();
        foreach (var animator in animatorArray)
        {
            if (animator.CompareTag("Star"))
            {
                _starAnimators.Add(animator);
                animator.SetBool("Bright", true);
            }
            else if (animator.CompareTag("ClearScreen"))
            {
                _clearScreenAnimator = animator;
            }
        }
        _clearCanvas.enabled = false;

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
        _isOpened = _count >= _buttonCount;
        _animator.SetBool("isOpened", _isOpened);


        //�K�v�ȃ{�^���̐��𒴂��Ă��邩�ŐF��ύX
        //if (_isOpened)
        //{
        //    _spriteRenderer.color = new Color32(0, 255, 0, 150);
        //}
        //else
        //{
        //    _spriteRenderer.color = new Color32(255, 0, 0, 150);
        //}

        //SetSprite(_count >= _buttonCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other == _playerInfo.g_transform) { return; }
        if (_clear) { return; }
        //�{�^���̐�������Ă�����S�[��
        if(_count >= _buttonCount)
        {
            _clearCanvas.enabled = true;
            _playerInput.SwitchCurrentActionMap("UI");
            _clearScreenAnimator.SetTrigger("Scale");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    //�t���[���ɏd�Ȃ��Ă��邩�Ń��C���[��ύX����
    public void GoalLayerCheck()
    {
        //�X�N���[�����W�ɕϊ�
        var pos = Camera.main.WorldToScreenPoint(transform.position);

        //���W�Ɉʒu�Ƀ��C���΂�
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

    //���C���[��߂�
    public void SetOutsideLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Outside");
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

    public void FrameCount()
    {
        _frameCount++;
        SetStarParamater();
    }

    private void SetStarParamater()
    {
        if(_frameCount <= _minFrameCount)
        {
            foreach(var anim in _starAnimators)
            {
                anim.SetBool("Bright", true);
            }
        }
        else if(_frameCount <= _minFrameCount + STAR_GAP)
        {
            _starAnimators[0].SetBool("Bright", true);
            _starAnimators[1].SetBool("Bright", true);
            _starAnimators[2].SetBool("Bright", false);
        }
        else
        {
            _starAnimators[0].SetBool("Bright", true);
            _starAnimators[1].SetBool("Bright", false);
            _starAnimators[2].SetBool("Bright", false);
        }
    }

    public void Next()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Title()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

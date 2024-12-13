using UnityEngine;

public class MiniCharaAnimation : MonoBehaviour
{
    private Animator _animator = null;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayJumpAnimation()
    {
        //Debug.Log("������");
        _animator.SetTrigger("jump");
    }

    public void SetMoveAnimation(bool move)
    {
        _animator.SetBool("move", move);
    }

    public void SetLanding(bool land)
    {
        _animator.SetBool("isLanding", land);
    }

    public void SetSmile(bool smile)
    {
        _animator.SetBool("smile", smile);
    }

    public void PlayFrameAnimation()
    {
        _animator.SetTrigger("frameEnter");
    }

    public void StopFrameAnimation()
    {
        _animator.SetTrigger("frameExit");
    }
}

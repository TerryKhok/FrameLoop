using Unity.VisualScripting;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :BoxChild
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�e�I�u�W�F�N�g��Box�N���X�ɃA�N�Z�X������
 *               
 *  Created     :2024/04/27
 */
public class BoxChild : MonoBehaviour,IBox
{
    private Box _box = null;
    private Vector2 _childOffset = Vector2.zero;

    private void OnEnable()
    {
        _box = GetComponentInParent<Box>();
    }

    //�e�ɃA�^�b�`���ꂽBox�ɃA�N�Z�X����t��n��
    public void Hold(Transform t)
    {
        if(t == null)
        {
            _box.SetOffset(Vector2.zero);
        }
        else
        {
            _box.SetOffset(-_childOffset);
        }

        Debug.Log(-_childOffset);

        _box.Hold(t);
    }

    public void SetChildOffset(Vector2 offset)
    {
        _childOffset = offset;
    }
}

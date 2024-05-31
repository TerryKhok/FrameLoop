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

    private void OnEnable()
    {
        _box = GetComponentInParent<Box>();
    }

    //�e�ɃA�^�b�`���ꂽBox�ɃA�N�Z�X����t��n��
    public void Hold(Transform t)
    {
        _box.Hold(t);
    }

}

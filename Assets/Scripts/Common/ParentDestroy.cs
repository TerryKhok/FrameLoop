using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :ParentDestroy
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :���g���j�󂳂ꂽ��e�I�u�W�F�N�g��j�󂷂�
 *               
 *  Created     :2024/04/27
 */
public class ParentDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}

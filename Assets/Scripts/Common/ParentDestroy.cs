using System.Runtime.CompilerServices;
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
        Destroy(GetParent(transform).gameObject);
    }

    private Transform GetParent(Transform t)
    {
        Grid grid;
        if(t.parent != null && !t.parent.TryGetComponent<Grid>(out grid))
        {
            t = GetParent(t.parent);
        }

        return t;
    }
}
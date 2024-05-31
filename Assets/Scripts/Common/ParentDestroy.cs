using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :ParentDestroy
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :自身が破壊されたら親オブジェクトを破壊する
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

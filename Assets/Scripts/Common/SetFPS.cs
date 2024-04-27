using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :SetFPS
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :FPSÇå≈íËÇ∑ÇÈ
 *               
 *  Created     :2024/04/27
 */
public class SetFPS : MonoBehaviour
{
    [SerializeField, Tooltip("FPSè„å¿")]
    private int _targetFPS = 120;
    private void Awake()
    {
        Application.targetFrameRate = _targetFPS;
    }
}

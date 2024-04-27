using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :SetFPS
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :FPS���Œ肷��
 *               
 *  Created     :2024/04/27
 */
public class SetFPS : MonoBehaviour
{
    [SerializeField, Tooltip("FPS���")]
    private int _targetFPS = 120;
    private void Awake()
    {
        Application.targetFrameRate = _targetFPS;
    }
}

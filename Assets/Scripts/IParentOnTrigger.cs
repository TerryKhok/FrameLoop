using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :IPatentOnTrigger
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�q�I�u�W�F�N�g����OnTrigger�̃R�[���o�b�N���󂯎��Interface
 *               
 *  Created     :2024/04/27
 */
public interface IParentOnTrigger
{
    public void OnEnter(Collider2D other, Transform transform);
    public void OnStay(Collider2D other, Transform transform);
    public void OnExit(Collider2D other, Transform transform);
}

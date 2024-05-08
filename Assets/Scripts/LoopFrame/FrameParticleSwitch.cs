using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :FrameParticleSwitch
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :�����蔻��̗L���Ńt���[���̃p�[�e�B�N����؂�ւ���
 *               
 *  Created     :2024/05/08
 */
public class FrameParticleSwitch : MonoBehaviour
{
    private GameObject _collisionParticle = null;
    private GameObject _noCollisionParticle = null;

    private void Start()
    {
        _collisionParticle = transform.GetChild(0).gameObject;
        _noCollisionParticle = transform.GetChild(1).gameObject;

        SetParticle(false);
    }

    public void SetParticle(bool collision)
    {
        _collisionParticle.SetActive(collision);
        _noCollisionParticle.SetActive(!collision);
    }
}

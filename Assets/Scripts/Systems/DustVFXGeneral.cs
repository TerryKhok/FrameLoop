using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DustVFXGeneral : MonoBehaviour
{
    public float animationTime = 0; //�A�j���[�V�����̒���
    
    void Update()
    {
        StartCoroutine(DestroyVFX(animationTime));
    }

    private IEnumerator DestroyVFX(float t)
    {
        yield return new WaitForSeconds(t);
        Debug.Log("object destroyed");
        Destroy(gameObject);
    } 
}

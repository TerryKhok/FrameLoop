using UnityEngine;
using System;

/*  ProjectName :FrameLoop
 *  ClassName   :SingletonMonoBehavior
 *  Creator     :Fujishita.Arashi(https://qiita.com/Teach/items/c146c7939db7acbd7eee)
 *  
 *  Summary     :�C�ӂ̃N���XT���V���O���g���ɂ���
 *               
 *  Created     :2024/04/27
 */
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindAnyObjectByType(t);
                if (instance == null)
                {
                    //Debug.LogError(t + " ���A�^�b�`���Ă���GameObject�͂���܂���");
                }
            }

            return instance;
        }
    }

    virtual protected void Awake()
    {
        // ���̃Q�[���I�u�W�F�N�g�ɃA�^�b�`����Ă��邩���ׂ�
        // �A�^�b�`����Ă���ꍇ�͔j������B
        CheckInstance();
    }

    protected virtual bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }
        Destroy(this);
        return false;
    }
}

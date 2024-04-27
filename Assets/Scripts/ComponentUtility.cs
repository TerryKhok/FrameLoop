using System;
using System.Reflection;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :ComponentUtility
 *  Creator     :Fujishita.Arashi(https://zenn.dev/nogi_awn/articles/76440270429ad0)
 *  
 *  Summary     :Component���R�s�[����static�N���X
 *               
 *  Created     :2024/04/27
 */
public static class ComponentUtility
{
    //������component���R�s�[����
    public static T CopyFrom<T>(this T self, T other) where T : Component
    {
        Type type = typeof(T);

        //�p�u���b�N�t�B�[���h�����ׂĎ擾
        FieldInfo[] fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;

            //�t�B�[���h���R�s�[
            field.SetValue(self, field.GetValue(other));
        }

        //�v���p�e�B��S�Ď擾
        PropertyInfo[] props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanRead || prop.Name == "name" || prop.Name == "usedByComposite") continue;

            //�v���p�e�B���R�s�[
            prop.SetValue(self, prop.GetValue(other));
        }

        return self as T;
    }

    //�����̃v���p�e�B���R�s�[���ăA�^�b�`����
    public static T AddComponent<T>(this GameObject self, T other) where T : Component
    {
        return self.AddComponent<T>().CopyFrom(other);
    }
}
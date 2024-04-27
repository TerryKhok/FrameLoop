using System;
using System.Reflection;
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :ComponentUtility
 *  Creator     :Fujishita.Arashi(https://zenn.dev/nogi_awn/articles/76440270429ad0)
 *  
 *  Summary     :Componentをコピーするstaticクラス
 *               
 *  Created     :2024/04/27
 */
public static class ComponentUtility
{
    //引数のcomponentをコピーする
    public static T CopyFrom<T>(this T self, T other) where T : Component
    {
        Type type = typeof(T);

        //パブリックフィールドをすべて取得
        FieldInfo[] fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;

            //フィールドをコピー
            field.SetValue(self, field.GetValue(other));
        }

        //プロパティを全て取得
        PropertyInfo[] props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanRead || prop.Name == "name" || prop.Name == "usedByComposite") continue;

            //プロパティをコピー
            prop.SetValue(self, prop.GetValue(other));
        }

        return self as T;
    }

    //引数のプロパティをコピーしてアタッチする
    public static T AddComponent<T>(this GameObject self, T other) where T : Component
    {
        return self.AddComponent<T>().CopyFrom(other);
    }
}
using UnityEngine;

/*  ProjectName :FrameLoop
 *  ClassName   :BoxChild
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :親オブジェクトのBoxクラスにアクセスし直す
 *               
 *  Created     :2024/04/27
 */
public class BoxChild : MonoBehaviour,IBox
{
    private Box _box = null;

    private void OnEnable()
    {
        _box = GetComponentInParent<Box>();
    }

    //親にアタッチされたBoxにアクセスしてtを渡す
    public void Hold(Transform t)
    {
        Debug.Log("hoge");

        Vector2 vec = FrameLoop.Instance.GetSize();
        vec.y = 0;

        _box.SetOffset(-t.right * vec);
        Debug.Log(-t.right * vec);

        _box.Hold(t);
    }

}

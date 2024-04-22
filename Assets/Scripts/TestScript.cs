using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _obj = null;
    //private List<(Transform origin, Transform instance)> _loopList = new List<(Transform, Transform)>();

    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            Instantiate(_obj,Vector3.zero, Quaternion.identity);
        }

        //foreach(var pair in _loopList)
        //{
        //    var currentPos = pair.origin.position;
        //    currentPos.x -= 5;
        //    pair.instance.position = currentPos;
        //}
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    var otherTransform = other.transform;
    //    MeshRenderer renderer = other.GetComponent<MeshRenderer>();
    //    MeshFilter filter = otherTransform.GetComponent<MeshFilter>();
    //    var pos = otherTransform.position;
    //    pos.x -= 5;
    //    GameObject obj = Instantiate(_obj, pos, Quaternion.identity);
    //    obj.name = otherTransform.name;
    //    obj.AddComponent(renderer);
    //    obj.AddComponent(filter);
    //    _loopList.Add((otherTransform, obj.transform));
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFollowObject : MonoBehaviour
{
    [SerializeField, Tag]
    private string _followTag;

    private void Start()
    {
        var followObject = GetComponent<FollowObject>();
        followObject.Target = GameObject.FindGameObjectWithTag(_followTag).transform;
    }
}

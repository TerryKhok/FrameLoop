using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaySomeMethod : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _event;

    public void PlaySomeEvent()
    {
        _event.Invoke();
    }
}

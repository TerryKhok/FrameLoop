using UnityEngine;
using UnityEngine.Events;

public class ActionTimer : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _event;

    [SerializeField]
    private float _delay;
    private float _elapsedTime;

    private bool _done = false;

    private void Update()
    {
        if (_done)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;
        if(_elapsedTime >= _delay )
        {
            _event.Invoke();
            _done = true;
        }
    }
}

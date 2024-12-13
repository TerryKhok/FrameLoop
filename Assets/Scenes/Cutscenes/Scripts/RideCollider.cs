using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideCollider : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _gameObjects;
    [SerializeField]
    private float _wait = 0;

    private float _elapsedTime = 0;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _elapsedTime += Time.fixedDeltaTime;

            if (_elapsedTime >= _wait)
            {
                foreach (var gameObject in _gameObjects)
                {
                    gameObject.SetActive(false);
                    AudioManager.instance.Stop("Walk");
                }
            }
        }
    }
}

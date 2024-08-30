using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _steamPrefab;
    [SerializeField]
    private StageType _stageType;
    [SerializeField]
    private float _minInterval;
    [SerializeField]
    private float _maxInterval;
    [SerializeField,Range(1,100)]
    private int _probability = 5;

    private float _elapsedTime = 0;
    private Vector3 _position = Vector3.zero;
    private Quaternion _rotation = Quaternion.identity;

    [System.Serializable]
    enum StageType
    {
        World1, World2, World3, Count
    }

    private void Start()
    {
        _position = transform.position;
        var euler = transform.eulerAngles;
        euler.z -= 135.0f;
        _rotation = Quaternion.Euler(euler);

        var renderer = _steamPrefab.GetComponent<SpriteRenderer>();

        switch(_stageType)
        {
            case StageType.World1:
                renderer.color = new Color(0.4f, 0.4f, 0.5f, 1.0f);
                break;
            case StageType.World2:
                renderer.color = new Color(0.6f, 0.6f, 0.8f, 1.0f);
                break;
            case StageType.World3:
                renderer.color = new Color(0.4f, 0.3f, 0.3f, 1.0f);
                break;
        }

        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if(_elapsedTime < _minInterval )
        {
            return;
        }

        if(_elapsedTime >= _maxInterval )
        {
            SpawnSteam();
        }
        else
        {
            int rand = Random.Range(1, 100);
            if(rand <= _probability)
            {
                SpawnSteam();
            }
        }

    }

    private void SpawnSteam()
    {
        var instance = Instantiate(_steamPrefab, _position, _rotation);
        Destroy(instance, 35.0f / 60.0f);

        _elapsedTime = 0;
    }
}

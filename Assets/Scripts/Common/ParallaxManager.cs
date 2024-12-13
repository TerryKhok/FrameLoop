using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField]
    private List<SwitchObjectInfo> _parallaxObjectList = new List<SwitchObjectInfo>();

    [System.Serializable]
    public struct SwitchObjectInfo
    {
        public GameObject[] objects;
        public float time;
    }

    private int _index = 0;
    private float _elapsedTime = 0.0f;

    private void Start()
    {
        foreach (var parallax in _parallaxObjectList[_index].objects)
        {
            parallax.SetActive(true);
        }
        foreach(var parallax in _parallaxObjectList[_index+1].objects)
        {
            parallax.SetActive(false);
        }
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        //Debug.Log(_elapsedTime);

        if (_index >= _parallaxObjectList.Count - 1)
        {
            return;
        }

        if(_elapsedTime >= _parallaxObjectList[_index].time)
        {
            foreach (var parallax in _parallaxObjectList[_index].objects)
            {
                parallax.SetActive(false);
            }
            foreach (var parallax in _parallaxObjectList[_index+1].objects)
            {
                parallax.SetActive(true);
            }

            _index++;
            _elapsedTime = 0.0f;
        }
    }
}

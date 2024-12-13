using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntroScript : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPlayerSprite;
    [SerializeField]
    private GameObject playerTabletSprite;

    [System.Serializable]
    public struct SpriteChangeInfo
    {
        public float start, end;
    }

    [SerializeField]
    private SpriteChangeInfo[] spriteChangeInfos;
    private int index = 0;

    [System.Serializable]
    public struct Timing
    {
        public float start, end;
    }

    [SerializeField]
    private Timing[] _smileTiming;
    private float _smileElapsedTime = 0;
    private int _smileIndex = 0;

    private PlayerAnimation _anim = null;

    [SerializeField]
    private float exclamationTime = 0;
    [SerializeField]
    private GameObject exclamation;

    private bool exclamationFlag = false;

    float elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerTabletSprite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (spriteChangeInfos[index].start <= elapsedTime)
        {
            mainPlayerSprite.SetActive(false);
            playerTabletSprite.SetActive(true);
        }

        if (spriteChangeInfos[index].end <= elapsedTime)
        {
            index++;
            mainPlayerSprite.SetActive(true);
            playerTabletSprite.SetActive(false);
        }

        if (exclamationTime <= elapsedTime && !exclamationFlag)
        {
            exclamationFlag = true;

            var pos = transform.position;

            pos += new Vector3(1.0f, 1.0f);

            var instance = Instantiate(exclamation, pos, Quaternion.identity);
            Destroy(instance, 1.0f);
        }

        //_smileElapsedTime += Time.deltaTime;

        //if (_smileTiming[_smileIndex].start <= _smileElapsedTime)
        //{
        //    _anim.SetSmile(true);
        //    //Debug.Log("‚É‚±I");
        //}

        //if (_smileTiming[_smileIndex].end <= _smileElapsedTime)
        //{
        //    _smileIndex++;
        //    _smileElapsedTime = 0;
        //    _anim.SetSmile(false);
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntroScript : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPlayerSprite;
    [SerializeField]
    private GameObject playerTabletSprite;
    [SerializeField]
    private GameObject playerClimbingSprite;

    [System.Serializable]
    public struct SpriteChangeInfo
    {
        public float start, end;
    }

    [SerializeField]
    private SpriteChangeInfo[] spriteChangeInfos;
    private int index = 0;

    //climbing rocket
    [SerializeField]
    private float startPosY;
    [SerializeField]
    private float endPosY;
    [SerializeField]
    private float climbingStartTime;
    [SerializeField]
    private float climbingEndTime;

    Animator climbingAnim;

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
    private bool climbingFlag = false;

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
        playerClimbingSprite.SetActive(false);

        climbingAnim = playerClimbingSprite.GetComponent<Animator>();
        climbingAnim.Play("PlayerClimb");
        climbingFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        //tablet animation
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

        //climbing rocket animation
        if (!climbingFlag)
        {
            if (climbingStartTime <= elapsedTime)
            {
                playerClimbingSprite.SetActive(true);
                mainPlayerSprite.SetActive(false);
            }
            if (climbingStartTime <= elapsedTime && climbingEndTime >= elapsedTime)
            {
                var pos = playerClimbingSprite.transform.position;
                pos.y = Mathf.Lerp(startPosY, endPosY, (elapsedTime - climbingStartTime) / (climbingEndTime - climbingStartTime));
                playerClimbingSprite.transform.position = pos;
            }
            if (climbingEndTime <= elapsedTime)
            {
                climbingFlag = true;
                playerClimbingSprite.SetActive(false);
                mainPlayerSprite.SetActive(true);
            }
        }

        //exclamation
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

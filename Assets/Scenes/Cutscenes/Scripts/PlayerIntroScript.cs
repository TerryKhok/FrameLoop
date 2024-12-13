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
    }
}

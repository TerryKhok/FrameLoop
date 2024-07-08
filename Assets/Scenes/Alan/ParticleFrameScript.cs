using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleFrameScript : SingletonMonoBehaviour<ParticleFrameScript>
{
    [SerializeField]
    private Transform framePos;

    private FrameLoop frameLoop; //script

    private float elapsedTime;
    [SerializeField]
    private float lerpSpeed = 15f;

    [SerializeField]
    private Transform particleFramePos;

    [SerializeField]
    private GameObject activeFrameObject;

    [SerializeField]
    private float frameTransparency = 0.2f;
    [SerializeField]
    private Material mat;
    private Color matColInactive;
    private Color matColActive;

    [SerializeField]
    private ParticleSystem ps1; //top
    [SerializeField]
    private ParticleSystem ps2; //bottom
    [SerializeField]
    private ParticleSystem ps3; //left
    [SerializeField]
    private ParticleSystem ps4; //right

    [SerializeField]
    private GameObject frameClick;
    [SerializeField]
    private ParticleSystem burst1; //top
    [SerializeField]
    private ParticleSystem burst2; //bottom
    [SerializeField]
    private ParticleSystem burst3; //left
    [SerializeField]
    private ParticleSystem burst4; //right

    //new frame objects
    [SerializeField]
    private GameObject frameNewStatic;
    [SerializeField]
    private GameObject frameNewBurst;

    bool burstFlag = true;

    private FrameParticleSwitch[] topParticleSwitchArray, bottomParticleSwitchArray, leftParticleSwitchArray, rightParticleSwitchArray;

    private Dictionary<TileReplace, List<(int switchNum, int num)>> _breakableDic = new Dictionary<TileReplace, List<(int switchNum, int num)>>();

    private void Start()
    {
        frameLoop = FrameLoop.Instance;
        transform.position = PlayerInfo.Instance.g_transform.position;
        burstFlag = true;
        activeFrameObject.SetActive(false);

        frameNewBurst.SetActive(false);

        //----------------------------------------------------------------------------------------------
        //当たり判定の有無で切り替えるパーティクルからコンポーネントを取得して配列に格納する
        //----------------------------------------------------------------------------------------------
        var particleParent = activeFrameObject.transform.GetChild(0);
        topParticleSwitchArray = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        particleParent = activeFrameObject.transform.GetChild(1);
        bottomParticleSwitchArray = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        particleParent = activeFrameObject.transform.GetChild(2);
        leftParticleSwitchArray = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        particleParent = activeFrameObject.transform.GetChild(3);
        rightParticleSwitchArray = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        //-----------------------------------------------------------------------------------------------

        //_audioManager.Play("Main BGM");
    }

    private void Update()
    {
        if(!frameLoop.g_isActive)
        {
            if (frameNewStatic.GetComponent<SpriteRenderer>().color.r < 0.95f ||
                frameNewStatic.GetComponent<SpriteRenderer>().color.g < 0.95f ||
                frameNewStatic.GetComponent<SpriteRenderer>().color.b < 0.95f)
            {
                Color lerpedColor;
                lerpedColor.r = Mathf.Lerp(frameNewStatic.GetComponent<SpriteRenderer>().color.r, 1.0f, 0.05f);
                lerpedColor.g = Mathf.Lerp(frameNewStatic.GetComponent<SpriteRenderer>().color.g, 1.0f, 0.05f);
                lerpedColor.b = Mathf.Lerp(frameNewStatic.GetComponent<SpriteRenderer>().color.b, 1.0f, 0.05f);
                lerpedColor.a = Mathf.Lerp(frameNewStatic.GetComponent<SpriteRenderer>().color.a, frameTransparency, 0.05f);
                frameNewStatic.GetComponent<SpriteRenderer>().color = lerpedColor;
            }
            else
            {
                matColInactive = new Color(1f, 1f, 1f, frameTransparency);
                frameNewStatic.GetComponent<SpriteRenderer>().color = matColInactive;
            }

            frameNewBurst.SetActive(false);

            activeFrameObject.SetActive(false);
            matColInactive = new Color(1f, 1f, 1f, frameTransparency);
            mat.color = matColInactive;
            if (!burstFlag)
            {
                burst1.Play(); burst2.Play(); burst3.Play(); burst4.Play();
                burstFlag = true;
            }
        }

        //フレームが起動したときに一度実行
        if (frameLoop.g_activeTrigger)
        {
            frameNewBurst.SetActive(true);

            activeFrameObject.SetActive(true);
            matColActive = new Color(0.4948379f, 0.6311985f, 0.7547169f, 1f);
            mat.color = matColActive;
            burstFlag = false;

            frameNewStatic.GetComponent<SpriteRenderer>().color = matColActive;

            //上下左右の当たり判定を取得するために4回ループ
            for (int i = 0; i < 4; i++)
            {
                //当たり判定の配列（bool）を取得
                bool[] workArray = frameLoop.GetHitArray(i);

                //当たり判定でパーティクルを切り替え
                for (int j = 0; j < workArray.Length; j++)
                {
                    switch (i)
                    {
                        case 0:
                            topParticleSwitchArray[j].SetParticle(workArray[j]);
                            break;
                        case 1:
                            bottomParticleSwitchArray[j].SetParticle(workArray[j]);
                            break;
                        case 2:
                            leftParticleSwitchArray[j].SetParticle(workArray[j]);
                            break;
                        case 3:
                            rightParticleSwitchArray[j].SetParticle(workArray[j]);
                            break;
                    }
                }
            }


            _breakableDic = frameLoop.GetBreakableDic();
        }

        if (particleFramePos.position != framePos.position)
        {
            elapsedTime += Time.deltaTime;
            float complete = elapsedTime / lerpSpeed;
            particleFramePos.position = Vector3.Lerp(particleFramePos.position, framePos.position, complete);
        }
        else
        {
            particleFramePos.position = framePos.position;
        }
    }

    public void SendDestroyMsg(TileReplace tileReplace)
    {
        if (!_breakableDic.ContainsKey(tileReplace))
        {
            return;
        }
        List<(int switchNum, int num)> numbersList = _breakableDic[tileReplace];

        foreach (var numbers in numbersList)
        {

            switch (numbers.switchNum)
            {
                case 0:
                    topParticleSwitchArray[numbers.num].SetParticle(false);
                    break;
                case 1:
                    bottomParticleSwitchArray[numbers.num].SetParticle(false);
                    break;
                case 2:
                    leftParticleSwitchArray[numbers.num].SetParticle(false);
                    break;
                case 3:
                    rightParticleSwitchArray[numbers.num].SetParticle(false);
                    break;
            }
        }
    }
}

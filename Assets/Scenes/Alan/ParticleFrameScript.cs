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
    private GameObject ps1; //top
    [SerializeField]
    private GameObject ps2; //bottom
    [SerializeField]
    private GameObject ps3; //left
    [SerializeField]
    private GameObject ps4; //right

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

    private FrameParticleSwitch[] topParticleSwitchArray,
                                  bottomParticleSwitchArray,
                                  leftParticleSwitchArray,
                                  rightParticleSwitchArray,
                                  topParticleSwitchArray_outside,
                                  bottomParticleSwitchArray_outside,
                                  leftParticleSwitchArray_outside,
                                  rightParticleSwitchArray_outside;

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

        particleParent = activeFrameObject.transform.GetChild(4);
        topParticleSwitchArray_outside = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        particleParent = activeFrameObject.transform.GetChild(5);
        bottomParticleSwitchArray_outside = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        particleParent = activeFrameObject.transform.GetChild(6);
        leftParticleSwitchArray_outside = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        particleParent = activeFrameObject.transform.GetChild(7);
        rightParticleSwitchArray_outside = particleParent.GetComponentsInChildren<FrameParticleSwitch>();

        //-----------------------------------------------------------------------------------------------

        //_audioManager.Play("Main BGM");
    }

    private void Update()
    {
        if(!frameLoop.g_isActive)
        {
            ps1.SetActive(true);
            ps2.SetActive(true);
            ps3.SetActive(true);
            ps4.SetActive(true);

            SpriteRenderer renderer = frameNewStatic.GetComponent<SpriteRenderer>();
            if (renderer.color.r < 0.95f ||
                renderer.color.g < 0.95f ||
                renderer.color.b < 0.95f)
            {
                Color lerpedColor;
                lerpedColor.r = Mathf.Lerp(renderer.color.r, 1.0f, 0.05f);
                lerpedColor.g = Mathf.Lerp(renderer.color.g, 1.0f, 0.05f);
                lerpedColor.b = Mathf.Lerp(renderer.color.b, 1.0f, 0.05f);
                lerpedColor.a = Mathf.Lerp(renderer.color.a, frameTransparency, 0.05f);
                renderer.color = lerpedColor;
            }
            else
            {
                matColInactive = new Color(1f, 1f, 1f, frameTransparency);
                renderer.color = matColInactive;
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
            ps1.SetActive(false);
            ps2.SetActive(false);
            ps3.SetActive(false);
            ps4.SetActive(false);

            frameNewBurst.SetActive(true);

            activeFrameObject.SetActive(true);
            matColActive = new Color(0.4948379f, 0.6311985f, 0.7547169f, 1f);
            mat.color = matColActive;
            burstFlag = false;

            frameNewStatic.GetComponent<SpriteRenderer>().color = matColActive;

            //上下左右の当たり判定を取得するために4回ループ
            for (int i = 0; i < 6; i++)
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
                        case 4:
                            topParticleSwitchArray_outside[j].SetParticle(workArray[j]);
                            bottomParticleSwitchArray_outside[j].SetParticle(workArray[j]);
                            break;
                        case 5:
                            leftParticleSwitchArray_outside[j].SetParticle(workArray[j]);
                            rightParticleSwitchArray_outside[j].SetParticle(workArray[j]);
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
                case 4:
                    topParticleSwitchArray_outside[numbers.num].SetParticle(false);
                    break;
                case 5:
                    bottomParticleSwitchArray_outside[numbers.num].SetParticle(false);
                    break;
                case 6:
                    leftParticleSwitchArray_outside[numbers.num].SetParticle(false);
                    break;
                case 7:
                    rightParticleSwitchArray_outside[numbers.num].SetParticle(false);
                    break;
            }
        }
    }
}

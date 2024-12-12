using UnityEngine;

public class EndingFrame : MonoBehaviour
{
    [System.Serializable]
    public struct Timing
    {
        public float start, end;
    }

    [SerializeField]
    private Timing[] _burstTiming;
    private int _burstIndex = 0;

    [SerializeField]
    private GameObject activeFrameObject;

    [SerializeField]
    private float frameTransparency = 0.2f;
    [SerializeField]
    private Material mat;
    private Color matColInactive;
    private Color matColActive;

    //[SerializeField]
    //private GameObject ps1; //top
    //[SerializeField]
    //private GameObject ps2; //bottom
    //[SerializeField]
    //private GameObject ps3; //left
    //[SerializeField]
    //private GameObject ps4; //right

    [SerializeField]
    private ParticleSystem burst1; //top
    [SerializeField]
    private ParticleSystem burst2; //bottom
    [SerializeField]
    private ParticleSystem burst3; //left
    [SerializeField]
    private ParticleSystem burst4; //right

    //new frame objects
    //[SerializeField]
    //private GameObject frameNewStatic;
    //[SerializeField]
    //private GameObject frameNewBurst;

    private float _elapsedTime = 0.0f;
    bool burstFlag = true, _played = false, _stoped = false;

    bool framePlayedSound = false;

    void Start()
    {
        burstFlag = true;
        activeFrameObject.SetActive(false);

        //frameNewBurst.SetActive(false);
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_burstTiming[_burstIndex].start <= _elapsedTime)
        {
            Play();

            if(framePlayedSound == false)
            {
                AudioManager.instance.Play("Frame");
                AudioManager.instance.Stop("FrameTP");
                framePlayedSound = true;
            }
        }

        if (_burstTiming[_burstIndex].end <= _elapsedTime)
        {
            _burstIndex++;
            _elapsedTime = 0;
            Stop();

            if (framePlayedSound == true)
            {
                AudioManager.instance.Stop("Frame");
                AudioManager.instance.Play("FrameTP");
                framePlayedSound = false;
            }
        }
    }

    private void Play()
    {
        //ps1.SetActive(false);
        //ps2.SetActive(false);
        //ps3.SetActive(false);
        //ps4.SetActive(false);

        //frameNewBurst.SetActive(true);

        activeFrameObject.SetActive(true);
        matColActive = new Color(0.4948379f, 0.6311985f, 0.7547169f, 1f);
        mat.color = matColActive;
        burstFlag = false;

        //frameNewStatic.GetComponent<SpriteRenderer>().color = matColActive;
    }

    private void Stop()
    {
        //ps1.SetActive(true);
        //ps2.SetActive(true);
        //ps3.SetActive(true);
        //ps4.SetActive(true);

        //SpriteRenderer renderer = frameNewStatic.GetComponent<SpriteRenderer>();
        //if (renderer.color.r < 0.95f ||
        //    renderer.color.g < 0.95f ||
        //    renderer.color.b < 0.95f)
        //{
        //    Color lerpedColor;
        //    lerpedColor.r = Mathf.Lerp(renderer.color.r, 1.0f, 0.05f);
        //    lerpedColor.g = Mathf.Lerp(renderer.color.g, 1.0f, 0.05f);
        //    lerpedColor.b = Mathf.Lerp(renderer.color.b, 1.0f, 0.05f);
        //    lerpedColor.a = Mathf.Lerp(renderer.color.a, frameTransparency, 0.05f);
        //    renderer.color = lerpedColor;
        //}
        //else
        //{
        //    matColInactive = new Color(1f, 1f, 1f, frameTransparency);
        //    renderer.color = matColInactive;
        //}

        //frameNewBurst.SetActive(false);

        activeFrameObject.SetActive(false);
        matColInactive = new Color(1f, 1f, 1f, frameTransparency);
        mat.color = matColInactive;
        if (!burstFlag)
        {
            burst1.Play(); burst2.Play(); burst3.Play(); burst4.Play();
            burstFlag = true;
        }
    }
}

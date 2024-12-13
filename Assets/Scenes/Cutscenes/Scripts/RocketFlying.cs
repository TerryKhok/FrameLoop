using UnityEngine;

public class RocketFlying : MonoBehaviour
{
    [SerializeField]
    private float rocketPositionX;
    [SerializeField]
    private float rocketPositionY;

    [SerializeField]
    private float closeTiming;

    [SerializeField]
    private float takeOffTiming;

    [SerializeField]
    private float flyTiming;

    [SerializeField]
    private float flyingTime;

    private float rocketDestinationX;
    private float rocketDestinationY;

    private float elapsedTime;
    bool hasOpened = false, hasTookOff = false;

    [SerializeField]
    private GameObject rocketParticles;
    [SerializeField]
    private float shakeAmount = 10f;
    [SerializeField]
    private float shakeIntensity = 1f;

    private Animator animator;

    bool rocketSoundPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        rocketDestinationX = transform.position.x;
        rocketDestinationY = transform.position.y;

        rocketParticles.SetActive(false);

        animator = GetComponentInChildren<Animator>();
        animator.Play("rocket landed");
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= closeTiming && !hasOpened)
        {
            transform.position = new Vector3(rocketDestinationX, rocketDestinationY, 0);
            rocketParticles.SetActive(false);
            hasOpened = true;

            animator.Play("rocket close");
            //AudioManager.instance.Play("DoorOpen");
        }
        else if (elapsedTime >= takeOffTiming && !hasTookOff)
        {
            animator.Play("rocket takeOff");
            hasTookOff = true;
        }
        else if (elapsedTime >= flyTiming)
        {
            if (!rocketSoundPlayed)
            {
                rocketSoundPlayed = true;
                AudioManager.instance.Play("Rocket");
                animator.Play("rocket flying");
                rocketParticles.SetActive(true);
            }

            float t = (elapsedTime - flyTiming) / flyingTime;

            //shaking
            float offsetX = Mathf.PerlinNoise(Time.time * shakeAmount, 0f) * 2f - 1f;
            float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeAmount) * 2f - 1f;

            //set pos
            Vector3 lerpedPosition = new Vector3(Mathf.Lerp(rocketDestinationX, rocketPositionX, t), Mathf.Lerp(rocketDestinationY, rocketPositionY, t), 0);
            this.GetComponent<Transform>().position = lerpedPosition + (new Vector3(offsetX, offsetY, 0) * shakeIntensity);
        }
    }

    private void OnDisable()
    {
        AudioManager.instance.Stop("Rocket");
    }
}


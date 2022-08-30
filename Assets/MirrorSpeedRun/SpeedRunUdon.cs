//Originally created by Rami-Pastrami for VRC - originally uploaded for Booth
//Credit to Sazuneko for the banner image
//Feel free to use and modify!
//Note: The system scales itself according to the mirror gameobject added. All you need to do is put the "MirrorSpeedRun" on the bottom left of the mirror, below the bottom
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SpeedRunUdon : UdonSharpBehaviour
{

    #region setup
    [Header("Sync Confetti over Network")]
    [Tooltip("Should all users see a confetti effect if one user gets the achievement?")]
    public bool syncConfetti = true;

    [Header("Sync Banner over Network")]
    [Tooltip("Should all users see the achievement banner if one user gets the achievement?")]
    public bool syncBanner = true;

    [Header("Sync Sound over Network")]
    [Tooltip("Should all users hear the sound if one user gets the achievement?")]
    public bool syncSound = false;

    [Header("Max time for achievement")]
    [Tooltip("The maximum number of seconds a player can wait before triggering the achievement")]
    public float timer = 12f;

    [Header("Mirror Script is Attached to. MAKE SURE NEITHER THE MIRROR NOR THE SCRIPT ARE PARENTED TO EACH OTHER!")]
    [Tooltip("From the Unity Hierachy, drag the mirror this script should be attached to here. This script and the associated mirror can directly share a parent, but neither should be parented to each other")]
    public GameObject Mirror;

    [Header("Banner Gameobject")]
    [Tooltip("The Banner Gameobject in the scene. best leave this to default")]
    public GameObject Banner;




    private ParticleSystem confetti;
    private Animator bannerAnimator;
    private AudioSource audioCheers;

    #endregion

    #region UnityCalls

    private void Start()
    {
        confetti = GetComponent<ParticleSystem>();
        bannerAnimator = Banner.GetComponent<Animator>();
        audioCheers = GetComponent<AudioSource>();
        Transform mirrorTransform = Mirror.GetComponent<Transform>();
        BoxCollider PlayerDetector = GetComponent<BoxCollider>();

        Vector3 mirrorPos = mirrorTransform.localPosition;
        Vector3 mirrorScale = mirrorTransform.localScale;

        PlayerDetector.size = new Vector3(mirrorScale.x, 1f, 1f);
        PlayerDetector.center = new Vector3((mirrorScale.x - 1.5f) / -2f, 0.75f, -0.5f);

        //I don't know why, I don't want to know why, I shouldn't have to wonder why, but for whatever reason this stupid particle system isn't updating unless we do this terribleness
        var confettiShape = confetti.shape;
        confettiShape.position = new Vector3((mirrorScale.x - 1.5f) / -2f, -0.25f, -0.5f);
        confettiShape.scale = new Vector3((mirrorScale.x * 0.8f) / -2f, 1f, 1f);

    }

    private void Update()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }

    }

    private void OnPlayerTriggerEnter()
    {
        if(timer > 0f)
        {
            timer = -1f; //avoid repeats
            triggerAchievement();
        }
    }

    #endregion

    /// <summary>
    /// Triggers the achievement locally, and over network if enabled
    /// </summary>
    private void triggerAchievement()
    {
        if (syncConfetti) { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayConfetti"); } else { PlayConfetti(); }
        if (syncBanner) { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayBanner"); } else { PlayBanner(); }
        if (syncSound) { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlaySound"); } else { PlaySound(); }
    }

    //below are segregated into their own functions so that they can be broadcasted over Photon
    public void PlayConfetti()
    {
        confetti.Play();
    }

    public void PlayBanner()
    {
        bannerAnimator.Play("Banner Enabled");
    }

    public void PlaySound()
    {
        audioCheers.Play();
    }

}

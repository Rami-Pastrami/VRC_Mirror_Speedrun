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

    [Header("The Mirror in the hierarchy, make sure this prefab isn't parented to the mirror or vice versa.")]
    [Tooltip("From the Unity hierarchy, drag the mirror this script should be attached to here. This script and the associated mirror can directly share a parent, but neither should be parented to each other")]
    public Transform mirrorTransform;

    [Header("Banner Gameobject")]
    [Tooltip("The Banner Gameobject in the scene. best leave this to default")]
    public GameObject Banner;

    [Header("Player Collider Scale Adjustment (Use Gizmos to see)")]
    [Tooltip("If using a custom mirror, the scale calculation done in here may be inaccurate. Change these values from the default [1,0,0] if the sizing isn't quite right")]
    public Vector3 MirrorAdjustment = new Vector3(1f, 0f, 0f);


    private ParticleSystem confetti;
    private Animator bannerAnimator;
    private AudioSource audioCheers;

    #endregion

    //Following region are all calls made by unity
    #region UnityCalls

    private void Start()
    {
        if(mirrorTransform == null)
        {
            Debug.LogError("MirrorSpeedRun: You forgot to drag the mirror into the Udon script in the inspector!");
        }

        confetti = GetComponent<ParticleSystem>();
        bannerAnimator = Banner.GetComponent<Animator>();
        audioCheers = GetComponent<AudioSource>();
        BoxCollider PlayerDetector = GetComponent<BoxCollider>();
        Vector3 mirrorScale = GetMirrorScale(mirrorTransform, MirrorAdjustment);

        PlayerDetector.size = GetSizeCollider(mirrorScale);
        PlayerDetector.center = GetCenterCollider(mirrorScale);

        //I don't know why, I don't want to know why, I shouldn't have to wonder why, but for whatever reason this stupid particle system isn't updating unless we do this terribleness
        var confettiShape = confetti.shape;
        confettiShape.position = GetCenterConfetti(mirrorScale);
        confettiShape.scale = GetSizeConfetti(mirrorScale);

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

    //Custom Gizmo handler because people are dum dums
    void OnDrawGizmos()
    {
        Vector3 mirrorScale = GetMirrorScale(mirrorTransform, MirrorAdjustment);
        Gizmos.color = Color.green;

        //Collider
        Gizmos.DrawWireCube(transform.position + GetCenterCollider(mirrorScale), GetSizeCollider(mirrorScale));

        Gizmos.color = Color.cyan;

        //Particle Emitter
        Gizmos.DrawWireCube(transform.position + GetCenterConfetti(mirrorScale), GetSizeConfetti(mirrorScale));
    }


    #endregion

    //Following region contains functions that calculates scales of the particles and colliders based on the mirror transform
    #region MirrorScaling

    private Vector3 GetMirrorScale(Transform MirrorTrans, Vector3 MirrorAdjustment)
    {
        Vector3 MirrorScale = new Vector3();
        if(MirrorTrans == null)
        {
            Debug.LogWarning("MirrorSpeedRun: You need to drag the mirror from the scene to the Mirror Transform Component in the MirrorSpeedrun!");
        }
        else
        {
            MirrorScale = MirrorTrans.localScale;
        }
        return new Vector3(MirrorScale.x * MirrorAdjustment.x, MirrorScale.y * MirrorAdjustment.y, MirrorScale.z * MirrorAdjustment.z);
    }

    private Vector3 GetSizeCollider(Vector3 mirrorScale)
    {
        return new Vector3(mirrorScale.x, 1f, 1f);
    }

    private Vector3 GetCenterCollider(Vector3 mirrorScale)
    {
        return (new Vector3((mirrorScale.x - 1.5f) / -2f, 0.75f, -0.5f)) + new Vector3(0f, MirrorAdjustment.y, MirrorAdjustment.z);
    }

    private Vector3 GetSizeConfetti(Vector3 mirrorScale)
    {
        return new Vector3((mirrorScale.x * 0.8f) / -2f, 1f, 1f);
    }
    private Vector3 GetCenterConfetti(Vector3 mirrorScale)
    {
        return new Vector3((mirrorScale.x - 1.5f) / -2f, -0.25f, -0.5f) + new Vector3(0f, MirrorAdjustment.y, MirrorAdjustment.z);
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

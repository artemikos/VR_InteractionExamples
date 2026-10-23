using UnityEngine;


[RequireComponent(typeof(AudioSource), typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class SpinWheel1 : MonoBehaviour
{
    public Material normalMat;
    public Material hoverMat;
    public Material grabMat;
    public AudioClip grabClip;
    public AudioClip tickClip;

    public Transform visual;                    // SpinPivot с моделью колеса
    public Vector3 spinAxis = Vector3.forward;  // локальная ось колеса (в осях корня Wheel)
    public float tickAngle = 45f;               // щелчок каждые N градусов
    public float deadZone = 0.02f;              // рука ближе к центру, чем это (м), игнорируется

    Renderer rend;
    AudioSource audioSrc;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    Transform hand;                             // attach-точка контроллера, пока колесо взято
    Vector3 lastDir;
    float tickAccum;
    bool grabbed;
    int hoverCount;

    void Awake()
    {
        rend = visual.GetComponentInChildren<Renderer>();
        audioSrc = GetComponent<AudioSource>();
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(_ => { hoverCount++; UpdateMaterial(); });
        interactable.hoverExited.AddListener(_ => { hoverCount = Mathf.Max(0, hoverCount - 1); UpdateMaterial(); });

        interactable.selectEntered.AddListener(args =>
        {
            hand = args.interactorObject.GetAttachTransform(interactable);
            grabbed = true;
            tickAccum = 0f;
            TryGetHandDir(out lastDir);
            audioSrc.PlayOneShot(grabClip);
            UpdateMaterial();
        });

        interactable.selectExited.AddListener(_ =>
        {
            grabbed = false;
            hand = null;
            UpdateMaterial();
        });

        UpdateMaterial();
    }

    Vector3 WorldAxis => transform.TransformDirection(spinAxis).normalized;

    // направление от центра колеса к руке, спроецированное на плоскость колеса
    bool TryGetHandDir(out Vector3 dir)
    {
        dir = Vector3.zero;
        if (hand == null) return false;

        Vector3 v = hand.position - visual.position;
        v = Vector3.ProjectOnPlane(v, WorldAxis);
        if (v.magnitude < deadZone) return false;

        dir = v.normalized;
        return true;
    }

    void Update()
    {
        if (!grabbed) return;
        if (!TryGetHandDir(out Vector3 dir)) return;

        if (lastDir != Vector3.zero)
        {
            float delta = Vector3.SignedAngle(lastDir, dir, WorldAxis);
            visual.Rotate(WorldAxis, delta, Space.World);

            tickAccum += Mathf.Abs(delta);
            if (tickAccum >= tickAngle)
            {
                tickAccum = 0f;
                audioSrc.PlayOneShot(tickClip);
            }
        }
        lastDir = dir;
    }

    void UpdateMaterial()
    {
        if (grabbed) rend.material = grabMat;
        else if (hoverCount > 0) rend.material = hoverMat;
        else rend.material = normalMat;
    }
}
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(AudioSource), typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class BowlingBall : MonoBehaviour
{
    public Material normalMat;
    public Material hoverMat;
    public Material grabMat;
    public AudioClip grabClip;
    public AudioClip hitClip;

    Renderer rend;
    AudioSource audioSrc;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable;

    bool grabbed;
    int hoverCount;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        audioSrc = GetComponent<AudioSource>();
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        interactable.hoverEntered.AddListener(_ => { hoverCount++; UpdateMaterial(); });
        interactable.hoverExited.AddListener(_ => { hoverCount = Mathf.Max(0, hoverCount - 1); UpdateMaterial(); });
        interactable.selectEntered.AddListener(_ =>
        {
            grabbed = true;
            audioSrc.PlayOneShot(grabClip);
            UpdateMaterial();
        });
        interactable.selectExited.AddListener(_ => { grabbed = false; UpdateMaterial(); });

        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (grabbed) rend.material = grabMat;
        else if (hoverCount > 0) rend.material = hoverMat;
        else rend.material = normalMat;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<BowlingPin>() != null)
            audioSrc.PlayOneShot(hitClip);
    }
}
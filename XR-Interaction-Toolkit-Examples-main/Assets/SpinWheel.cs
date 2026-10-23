using UnityEngine;


[RequireComponent(typeof(AudioSource), typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class SpinWheel : MonoBehaviour
{
    public Material normalMat;
    public Material hoverMat;
    public Material activeMat;
    public AudioClip clickClip;

    public Transform visual;                    // SpinPivot с моделью колеса внутри
    public Vector3 spinAxis = Vector3.forward;
    public float spinSpeed = 240f;

    Renderer rend;
    AudioSource audioSrc;
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    bool active;
    int hoverCount;

    void Awake()
    {
        rend = visual.GetComponentInChildren<Renderer>();
        audioSrc = GetComponent<AudioSource>();
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(_ => { hoverCount++; UpdateMaterial(); });
        interactable.hoverExited.AddListener(_ => { hoverCount = Mathf.Max(0, hoverCount - 1); UpdateMaterial(); });
        interactable.selectEntered.AddListener(_ =>
        {
            active = true;
            audioSrc.PlayOneShot(clickClip);
            UpdateMaterial();
        });
        interactable.selectExited.AddListener(_ => { active = false; UpdateMaterial(); });

        UpdateMaterial();
    }

    void Update()
    {
        if (active)
            visual.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
    }

    void UpdateMaterial()
    {
        if (active) rend.material = activeMat;
        else if (hoverCount > 0) rend.material = hoverMat;
        else rend.material = normalMat;
    }
}
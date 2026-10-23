using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class BowlingPin : MonoBehaviour
{
    public Material hitMat;
    public AudioClip hitClip;
    public float hitForce = 4f;
    public float shakeTime = 0.3f;
    public float shakeAmount = 0.03f;

    Renderer rend;
    Rigidbody rb;
    AudioSource audioSrc;
    bool hit;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        audioSrc = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hit || collision.gameObject.GetComponent<BowlingBall>() == null)
            return;

        hit = true;
        rend.material = hitMat;                                        // материал
        audioSrc.PlayOneShot(hitClip);                                 // звук
        StartCoroutine(Shake());                                       // анимация

        Vector3 dir = (transform.position - collision.transform.position).normalized;
        rb.AddForce((dir + Vector3.up * 0.5f) * hitForce, ForceMode.Impulse);   // физика
        rb.AddTorque(Random.insideUnitSphere * hitForce, ForceMode.Impulse);
    }

    IEnumerator Shake()
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;
        while (t < shakeTime)
        {
            t += Time.deltaTime;
            float k = 1f + Mathf.Sin(t * 60f) * shakeAmount * (1f - t / shakeTime);
            transform.localScale = startScale * k;
            yield return null;
        }
        transform.localScale = startScale;
    }
}
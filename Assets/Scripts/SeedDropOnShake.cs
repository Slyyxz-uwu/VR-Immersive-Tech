using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class SeedDropOnShake : MonoBehaviour
{
    [Header("Seed Settings")]
    public GameObject seedPrefab; // Assign specific seed prefab per veggie
    public int maxSeeds = 2;

    [Header("Shake Detection")]
    public float velocityThreshold = 1.5f;
    public float spawnCooldown = 1f;
    public float checkInterval = 0.1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] shakeSounds;
    public float audioCooldown = 0.5f;

    private Rigidbody rb;
    private int seedsDropped = 0;
    private float lastSeedTime = -999f;
    private float lastAudioTime = -999f;
    private float checkTimer = 0f;
    private bool isHeld = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args) => isHeld = true;
    private void OnRelease(SelectExitEventArgs args) => isHeld = false;

    private void Update()
    {
        if (!isHeld || seedsDropped >= maxSeeds)
            return;

        checkTimer += Time.deltaTime;
        if (checkTimer < checkInterval) return;
        checkTimer = 0f;

        float velocity = rb.velocity.magnitude;

        if (velocity > velocityThreshold)
        {
            if (Time.time - lastSeedTime > spawnCooldown && seedsDropped < maxSeeds)
            {
                DropSeed();
                seedsDropped++;
                lastSeedTime = Time.time;
            }

            if (Time.time - lastAudioTime > audioCooldown)
            {
                PlayShakeSound();
                lastAudioTime = Time.time;
            }

            if (seedsDropped >= maxSeeds)
                enabled = false;
        }
    }

    private void DropSeed()
    {
        if (seedPrefab == null) return;

        Vector3 dropPosition = transform.position + new Vector3(0, 0.2f, 0);
        GameObject seed = Instantiate(seedPrefab, dropPosition, Quaternion.identity);

        Rigidbody seedRb = seed.GetComponent<Rigidbody>();
        if (seedRb != null)
            seedRb.velocity = Vector3.down * 0.3f;
    }

    private void PlayShakeSound()
    {
        if (audioSource != null && shakeSounds.Length > 0)
        {
            AudioClip clip = shakeSounds[Random.Range(0, shakeSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}
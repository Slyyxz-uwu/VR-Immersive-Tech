using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class SeedDropOnShake : MonoBehaviour
{
    [Header("Seed Settings")]
    public GameObject seedPrefab;
    public int maxSeeds = 2;

    [Header("Shake Detection")]
    public float velocityThreshold = 1.5f;
    public float spawnCooldown = 1f;
    public float checkInterval = 0.1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] shakeSounds;
    public float audioCooldown = 0.5f; // ⏱️ Prevents audio spam

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
        var grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        var grab = GetComponent<XRGrabInteractable>();
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
            // ⏱️ Drop seed with cooldown
            if (Time.time - lastSeedTime > spawnCooldown && seedsDropped < maxSeeds)
            {
                DropSeed();
                seedsDropped++;
                lastSeedTime = Time.time;
            }

            // 🎵 Play shake sound with cooldown
            if (Time.time - lastAudioTime > audioCooldown)
            {
                PlayShakeSound();
                lastAudioTime = Time.time;
            }

            // 🛑 Optional: stop updates when max is reached
            if (seedsDropped >= maxSeeds)
                enabled = false;
        }
    }

    private void DropSeed()
    {
        if (seedPrefab == null) return;

        Vector3 dropPosition = transform.position + new Vector3(0, 0.15f, 0);
        GameObject seed = Instantiate(seedPrefab, dropPosition, Quaternion.identity);

        if (seed.TryGetComponent(out Rigidbody seedRb))
            seedRb.velocity = Vector3.down * 0.5f;

        Debug.Log($"[SEED DROP] Spawned seed: {seed.name} at {dropPosition}");
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
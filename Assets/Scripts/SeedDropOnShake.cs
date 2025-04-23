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
    public float audioCooldown = 0.5f;

    private Rigidbody rb;
    private XRGrabInteractable grab;
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
        grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
            Debug.Log("[SEED] Grab events registered!");
        }
        else
        {
            Debug.LogWarning("[SEED] XRGrabInteractable not found!");
        }
    }

    private void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
            Debug.Log("[SEED] Grab events unregistered.");
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
        Debug.Log("[SEED] Grabbed!");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        Debug.Log("[SEED] Released!");
    }

    private void Update()
    {
        if (!isHeld)
        {
            Debug.Log("[SEED] Not held, skipping shake check.");
            return;
        }

        if (seedsDropped >= maxSeeds)
        {
            Debug.Log("[SEED] Max seeds dropped, skipping.");
            return;
        }

        checkTimer += Time.deltaTime;
        if (checkTimer < checkInterval) return;
        checkTimer = 0f;

        float velocity = rb.velocity.magnitude;
        Debug.Log($"[SEED] Velocity: {velocity}");

        if (velocity > velocityThreshold)
        {
            if (Time.time - lastSeedTime > spawnCooldown)
            {
                DropSeed();
                seedsDropped++;
                lastSeedTime = Time.time;
                Debug.Log("[SEED] Seed dropped!");
            }

            if (Time.time - lastAudioTime > audioCooldown)
            {
                PlayShakeSound();
                lastAudioTime = Time.time;
                Debug.Log("[SEED] Shake sound played.");
            }

            if (seedsDropped >= maxSeeds)
            {
                Debug.Log("[SEED] Disabling script after max seeds.");
                enabled = false;
            }
        }
    }

    private void DropSeed()
    {
        if (seedPrefab == null)
        {
            Debug.LogWarning("[SEED] Seed prefab is missing!");
            return;
        }

        Vector3 dropPosition = transform.position + new Vector3(0, 0.2f, 0);
        GameObject seed = Instantiate(seedPrefab, dropPosition, Quaternion.identity);

        Rigidbody seedRb = seed.GetComponent<Rigidbody>();
        if (seedRb != null)
        {
            seedRb.velocity = Vector3.down * 0.3f;
        }
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
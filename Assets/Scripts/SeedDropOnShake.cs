using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SeedDropOnShake : MonoBehaviour
{
    [Header("Seed Settings")]
    public GameObject seedPrefab;
    public int maxTotalSeeds = 2;

    [Header("Shake Detection")]
    public float velocityThreshold = 1.5f;
    public float spawnCooldown = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] shakeSounds;

    private Rigidbody rb;
    private float lastSpawnTime = -999f;
    private int seedsDropped = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (rb.velocity.magnitude > velocityThreshold
            && Time.time - lastSpawnTime > spawnCooldown
            && seedsDropped < maxTotalSeeds)
        {
            DropSeed();
            lastSpawnTime = Time.time;
        }
    }

    void DropSeed()
    {
        if (audioSource != null && shakeSounds.Length > 0)
        {
            AudioClip clip = shakeSounds[Random.Range(0, shakeSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        if (seedPrefab != null)
        {
            // Raise it more clearly above ground
            Vector3 dropPosition = transform.position + transform.up * 0.6f + Random.insideUnitSphere * 0.05f;

            GameObject seed = Instantiate(seedPrefab, dropPosition, Quaternion.identity);

            Debug.Log($"[SEED DROP] Spawned seed: {seed.name} at {dropPosition}");
        }
    }
}
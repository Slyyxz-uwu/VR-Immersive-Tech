using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SeedDropOnShake : MonoBehaviour
{
    [Header("Seed Settings")]
    public GameObject seedPrefab;
    public Transform spawnPoint;

    [Header("Shake Detection")]
    public float velocityThreshold = 1.5f;
    public float spawnCooldown = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] shakeSounds;

    [Header("Particles")]
    public ParticleSystem shakeParticles;

    private Rigidbody rb;
    private float lastSpawnTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check if moving fast enough and cooldown has passed
        if (rb.velocity.magnitude > velocityThreshold && Time.time - lastSpawnTime > spawnCooldown)
        {
            DropSeed();
            lastSpawnTime = Time.time;
        }
    }

    void DropSeed()
    {
        // Play audio
        if (audioSource != null && shakeSounds.Length > 0)
        {
            AudioClip clip = shakeSounds[Random.Range(0, shakeSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        // Spawn seed
        if (seedPrefab != null && spawnPoint != null)
        {
            Instantiate(seedPrefab, spawnPoint.position, Quaternion.identity);
        }

        // Play particles
        if (shakeParticles != null)
        {
            shakeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            shakeParticles.Play();
        }
    }
}

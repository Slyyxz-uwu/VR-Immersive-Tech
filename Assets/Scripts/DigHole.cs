using UnityEngine;
using System.Collections;

public class TrowelDig : MonoBehaviour
{
    [Tooltip("Prefab to replace the soil with after digging.")]
    public GameObject soilWithHolePrefab;

    [Tooltip("Particle system to play at dig location.")]
    public ParticleSystem digEffect;

    [Tooltip("How long it takes to fully reveal the hole.")]
    public float digAnimationTime = 1f;

    private bool hasDug = false; // ✅ NEW: prevents re-triggering

    private void OnTriggerEnter(Collider other)
    {
        if (hasDug) return; // ✅ already dug, skip

        if ((other.CompareTag("SoilTile") || other.CompareTag("DiggableSoil")) && soilWithHolePrefab != null)
        {
            hasDug = true; // ✅ mark as dug

            Vector3 pos = other.transform.position;
            Quaternion rot = other.transform.rotation;
            Vector3 scale = other.transform.localScale;

            Destroy(other.gameObject);
            StartCoroutine(AnimateDugHole(pos, rot, scale));
        }
    }

    private IEnumerator AnimateDugHole(Vector3 position, Quaternion rotation, Vector3 targetScale)
    {
        GameObject newSoil = Instantiate(soilWithHolePrefab, position, rotation);
        newSoil.transform.localScale = Vector3.zero;

        float timer = 0f;
        while (timer < digAnimationTime)
        {
            timer += Time.deltaTime;
            float t = timer / digAnimationTime;
            newSoil.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        newSoil.transform.localScale = targetScale;

        if (digEffect != null)
        {
            digEffect.transform.position = position;
            digEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            digEffect.Play();
        }
    }
}
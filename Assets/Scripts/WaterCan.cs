using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WateringCan : MonoBehaviour
{
    [Header("Water Settings")]
    public bool isFull = false;
    public ParticleSystem waterSpray;
    public AudioSource fillSound;
    public AudioSource spraySound;

    [Header("XR Input")]
    public XRGrabInteractable grabInteractable;
    public InputActionReference triggerAction;
    private bool isHeld = false;

    [Header("Water Fill Visual")]
    public GameObject waterPrefab;              // The visual water object
    public Transform waterSpawnPoint;           // Where the water appears
    public Vector3 waterScale = new Vector3(1f, 1f, 1f);
    private GameObject currentWaterObject;

    [Header("Drain Settings")]
    public float drainTime = 3f;                // Time to fully drain
    private float currentDrainTime = 0f;
    private bool isSpraying = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") && !isFull)
        {
            isFull = true;

            if (fillSound) fillSound.Play();

            if (waterPrefab && waterSpawnPoint && currentWaterObject == null)
            {
                currentWaterObject = Instantiate(
                    waterPrefab,
                    waterSpawnPoint.position,
                    waterSpawnPoint.rotation,
                    waterSpawnPoint
                );

                currentWaterObject.transform.localScale = waterScale;
                currentDrainTime = 0f;
            }
        }
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        if (triggerAction != null)
            triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);

        if (triggerAction != null)
            triggerAction.action.Disable();
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
        StopSpraying();
    }

    private void Update()
    {
        if (!isHeld || !isFull || triggerAction == null)
            return;

        float triggerValue = triggerAction.action.ReadValue<float>();

        if (triggerValue > 0.1f)
        {
            StartSpraying();

            if (currentWaterObject != null)
            {
                currentDrainTime += Time.deltaTime;
                float progress = currentDrainTime / drainTime;
                progress = Mathf.Clamp01(progress);

                currentWaterObject.transform.localScale = Vector3.Lerp(waterScale, Vector3.zero, progress);

                if (progress >= 1f)
                {
                    EmptyCan();
                }
            }
        }
        else
        {
            StopSpraying();
        }
    }

    private void StartSpraying()
    {
        if (!isSpraying && waterSpray != null && !waterSpray.isPlaying)
        {
            waterSpray.Play();
            if (spraySound) spraySound.Play();
            isSpraying = true;
        }
    }

    private void StopSpraying()
    {
        if (isSpraying && waterSpray != null && waterSpray.isPlaying)
        {
            waterSpray.Stop();
            if (spraySound) spraySound.Stop();
            isSpraying = false;
        }
    }

    private void EmptyCan()
    {
        isFull = false;
        currentDrainTime = 0f;
        isSpraying = false;

        if (waterSpray != null && waterSpray.isPlaying)
            waterSpray.Stop();

        if (spraySound && spraySound.isPlaying)
            spraySound.Stop();

        if (currentWaterObject != null)
        {
            Destroy(currentWaterObject);
            currentWaterObject = null;
        }
    }
}
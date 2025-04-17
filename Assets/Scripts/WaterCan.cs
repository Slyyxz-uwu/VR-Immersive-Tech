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
    public InputActionReference triggerAction; // Drag your trigger action here

    private bool isHeld = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isFull = true;
            if (fillSound) fillSound.Play();
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
            StartSpraying();
        else
            StopSpraying();
    }

    private void StartSpraying()
    {
        if (waterSpray != null && !waterSpray.isPlaying)
        {
            waterSpray.Play();
            if (spraySound) spraySound.Play();
        }
    }

    private void StopSpraying()
    {
        if (waterSpray != null && waterSpray.isPlaying)
        {
            waterSpray.Stop();
            if (spraySound) spraySound.Stop();
        }
    }
}
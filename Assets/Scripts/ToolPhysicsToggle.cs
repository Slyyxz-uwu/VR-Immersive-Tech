using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class ToolPhysicsToggle : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        rb.isKinematic = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        rb.isKinematic = false;
    }
}
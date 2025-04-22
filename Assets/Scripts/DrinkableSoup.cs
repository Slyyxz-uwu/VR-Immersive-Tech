using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrinkableSoup : MonoBehaviour
{
    public string playerHeadTag = "PlayerHead";
    public AudioClip slurpSound;
    public float drinkDistance = 0.25f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isHeld = false;
    private AudioSource audioSource;

    private void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        audioSource = gameObject.AddComponent<AudioSource>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args) => isHeld = true;
    private void OnRelease(SelectExitEventArgs args) => isHeld = false;

    private void Update()
    {
        if (!isHeld) return;

        GameObject head = GameObject.FindGameObjectWithTag(playerHeadTag);
        if (head == null) return;

        float dist = Vector3.Distance(transform.position, head.transform.position);
        if (dist <= drinkDistance)
        {
            DrinkSoup();
        }
    }

    private void DrinkSoup()
    {
        isHeld = false;

        if (slurpSound != null)
        {
            audioSource.PlayOneShot(slurpSound);
        }

        Destroy(gameObject, slurpSound != null ? slurpSound.length : 0f);
    }
}
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RakeTool : MonoBehaviour
{
    [Header("Soil Tilling")]
    public GameObject tilledSoilPrefab;        // The prefab to instantiate
    public Transform soilSpawnPoint;           // Where to spawn the prefab
    public int maxTiles = 5;                   // Max allowed spawns
    public float tillAnimationTime = 1f;       // Time to grow the tile in

    [Header("XR Input")]
    public XRGrabInteractable grabInteractable;
    public InputActionReference triggerAction;

    private bool isHeld = false;
    private bool touchingSoil = false;
    private bool isTilling = false;
    private int tilesSpawned = 0;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soil"))
        {
            Debug.Log("Trigger entered: Soil detected.");
            touchingSoil = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Soil"))
        {
            touchingSoil = false;
        }
    }

    private void Update()
    {
        if (!isHeld || !touchingSoil || isTilling || tilesSpawned >= maxTiles)
            return;

        float triggerValue = triggerAction.action.ReadValue<float>();
        Debug.Log("Trigger value: " + triggerValue);

        if (triggerValue > 0.5f)
        {
            Debug.Log("Trigger pressed while on soil – attempting to till.");
            StartCoroutine(SpawnSoilSmooth());
        }
    }

    private System.Collections.IEnumerator SpawnSoilSmooth()
    {
        isTilling = true;
        Debug.Log("Tilling started.");

        if (tilledSoilPrefab == null || soilSpawnPoint == null)
        {
            Debug.LogWarning("Missing tilledSoilPrefab or soilSpawnPoint reference!");
            yield break;
        }

        GameObject soilTile = Instantiate(
            tilledSoilPrefab,
            soilSpawnPoint.position,
            Quaternion.identity
        );

        Debug.Log("Soil prefab instantiated.");
        tilesSpawned++;

        Vector3 targetScale = soilTile.transform.localScale;
        soilTile.transform.localScale = Vector3.zero;

        float timer = 0f;
        while (timer < tillAnimationTime)
        {
            timer += Time.deltaTime;
            float t = timer / tillAnimationTime;
            soilTile.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        soilTile.transform.localScale = targetScale;
        isTilling = false;
    }
}
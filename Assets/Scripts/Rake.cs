using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RakeTool : MonoBehaviour
{
    [Header("Soil Tilling")]
    public GameObject tilledSoilPrefab;
    public int maxTiles = 5;
    public float tillAnimationTime = 1f;

    [Header("Soil Appearance")]
    public Vector3 soilScale = new Vector3(1.5f, 1f, 1.5f); // Width, Height, Depth

    [Header("Soil Placement Control")]
    public float minSpacing = 1.5f;
    private List<Vector3> placedSoilPositions = new List<Vector3>();

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

    private void OnGrab(SelectEnterEventArgs args) => isHeld = true;

    private void OnRelease(SelectExitEventArgs args) => isHeld = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soil"))
        {
            Debug.Log("Soil detected.");
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

        if (triggerValue > 0.5f)
        {
            StartCoroutine(SpawnSoilSmooth());
        }
    }

    private IEnumerator<WaitForEndOfFrame> SpawnSoilSmooth()
    {
        isTilling = true;

        if (tilledSoilPrefab == null)
        {
            Debug.LogWarning("Missing soil prefab!");
            yield break;
        }

        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 0.01f;

        // Check for spacing
        foreach (Vector3 pos in placedSoilPositions)
        {
            if (Vector3.Distance(pos, spawnPosition) < minSpacing)
            {
                Debug.Log("Too close to another tile.");
                isTilling = false;
                yield break;
            }
        }

        GameObject soilTile = Instantiate(tilledSoilPrefab, spawnPosition, Quaternion.identity);
        placedSoilPositions.Add(spawnPosition);
        tilesSpawned++;

        soilTile.transform.localScale = Vector3.zero;

        float timer = 0f;
        while (timer < tillAnimationTime)
        {
            timer += Time.deltaTime;
            float t = timer / tillAnimationTime;
            soilTile.transform.localScale = Vector3.Lerp(Vector3.zero, soilScale, t);
            yield return new WaitForEndOfFrame();
        }

        soilTile.transform.localScale = soilScale;
        isTilling = false;
    }
}
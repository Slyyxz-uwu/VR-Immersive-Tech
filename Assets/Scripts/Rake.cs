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
    public Vector3 soilScale = new Vector3(1.5f, 1f, 1.5f);

    [Header("Soil Placement Control")]
    public float minSpacing = 1.5f;
    private List<Vector3> placedSoilPositions = new List<Vector3>();

    [Header("Ghost Tile")]
    public GameObject ghostTilePrefab;
    private GameObject ghostTileInstance;

    [Header("Raking Particle Effect")]
    public ParticleSystem rakeParticles;

    [Header("Raking Sound")]
    public AudioSource rakeSound; // ✅ New

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
        HideGhostTile();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soil"))
        {
            touchingSoil = true;
            ShowGhostTile();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Soil"))
        {
            touchingSoil = false;
            HideGhostTile();
        }
    }

    private void Update()
    {
        if (!isHeld || !touchingSoil || isTilling || tilesSpawned >= maxTiles)
        {
            HideGhostTile();
            return;
        }

        float triggerValue = triggerAction.action.ReadValue<float>();
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 0.01f;

        if (ghostTileInstance != null)
        {
            ghostTileInstance.transform.position = spawnPosition;

            bool tooClose = false;
            foreach (Vector3 pos in placedSoilPositions)
            {
                if (Vector3.Distance(pos, spawnPosition) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            ghostTileInstance.SetActive(!tooClose);
        }

        if (triggerValue > 0.5f)
        {
            StartCoroutine(SpawnSoilSmooth(spawnPosition));
        }
    }

    private IEnumerator<WaitForEndOfFrame> SpawnSoilSmooth(Vector3 spawnPosition)
    {
        isTilling = true;

        foreach (Vector3 pos in placedSoilPositions)
        {
            if (Vector3.Distance(pos, spawnPosition) < minSpacing)
            {
                isTilling = false;
                yield break;
            }
        }

        // ✅ Play particle effect
        if (rakeParticles != null)
            rakeParticles.Play();

        // ✅ Play sound effect
        if (rakeSound != null)
            rakeSound.Play();

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

    private void ShowGhostTile()
    {
        if (ghostTilePrefab == null || ghostTileInstance != null)
            return;

        ghostTileInstance = Instantiate(ghostTilePrefab);
        ghostTileInstance.transform.localScale = soilScale;
    }

    private void HideGhostTile()
    {
        if (ghostTileInstance != null)
        {
            Destroy(ghostTileInstance);
            ghostTileInstance = null;
        }
    }
}
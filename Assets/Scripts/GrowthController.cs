using System.Collections;
using UnityEngine;

public class GrowthController : MonoBehaviour
{
    [Header("Growth Settings")]
    public GameObject plantPrefab;
    public float growthDuration = 5f;
    public float riseHeight = 0.2f;

    [Header("Vegetable Prefabs")]
    public GameObject carrotPrefab;
    public GameObject cucumberPrefab;
    public GameObject tomatoPrefab;
    public GameObject cabbagePrefab;
    public GameObject pepperPrefab;

    private bool hasStartedGrowing = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasStartedGrowing) return;

        if (collision.gameObject.CompareTag("SoilTile"))
        {
            Debug.Log("[SEED] Seed touched soil. Preparing to grow...");
            hasStartedGrowing = true;

            Vector3 spawnPoint = collision.transform.position + new Vector3(0f, 0.01f, 0f);
            StartCoroutine(GrowPlant(spawnPoint));
        }
    }

    private IEnumerator GrowPlant(Vector3 position)
    {
        if (plantPrefab == null)
        {
            Debug.LogWarning("[GROWTH] Plant prefab not set!");
            yield break;
        }

        GameObject plant = Instantiate(plantPrefab, position, Quaternion.identity);
        Vector3 startPos = plant.transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;

        float timer = 0f;
        while (timer < growthDuration)
        {
            plant.transform.position = Vector3.Lerp(startPos, endPos, timer / growthDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        plant.transform.position = endPos;

        GameObject finalVeggie = GetVegetableForSeed();
        if (finalVeggie != null)
        {
            Instantiate(finalVeggie, endPos, Quaternion.identity);
        }

        // ✅ Now destroy the seed
        Destroy(gameObject);
    }

    private GameObject GetVegetableForSeed()
    {
        SeedType seedType = GetComponent<SeedType>();
        if (seedType == null)
        {
            Debug.LogWarning("[GROWTH] Missing SeedType component on seed.");
            return null;
        }

        switch (seedType.vegType)
        {
            case SeedType.VegetableType.Carrot: return carrotPrefab;
            case SeedType.VegetableType.Cucumber: return cucumberPrefab;
            case SeedType.VegetableType.Tomato: return tomatoPrefab;
            case SeedType.VegetableType.Cabbage: return cabbagePrefab;
            case SeedType.VegetableType.Pepper: return pepperPrefab;
            default: return null;
        }
    }
}

using UnityEngine;
using System.Collections;

public class GrowthController : MonoBehaviour
{
    [Header("Growth Settings")]
    public GameObject plantPrefab;
    public float growthDuration = 20f;
    public float riseHeight = 0.2f;

    [Header("Vegetable Prefabs")]
    public GameObject carrotPrefab;
    public GameObject cucumberPrefab;
    public GameObject tomatoPrefab;
    public GameObject cabbagePrefab;
    public GameObject pepperPrefab;

    private bool isWatered = false;
    private GameObject plant;
    private Transform spawnPoint;

    private void Start()
    {
        // Set spawn point from soil
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.3f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("SoilTile"))
            {
                spawnPoint = hit.transform.Find("PlantSpawnPoint");
                break;
            }
        }

        if (spawnPoint == null)
        {
            Debug.LogError("No PlantSpawnPoint found!");
            return;
        }

        plant = Instantiate(plantPrefab, spawnPoint.position, Quaternion.identity);
        StartCoroutine(GrowWhenWatered());
    }

    public void SetWatered()
    {
        isWatered = true;
    }

    IEnumerator GrowWhenWatered()
    {
        Debug.Log("[GROWTH] Plant spawned. Waiting for water...");
        yield return new WaitUntil(() => isWatered);

        Debug.Log("[GROWTH] Water received. Growing...");
        Vector3 startPos = plant.transform.position;
        Vector3 endPos = startPos + new Vector3(0, riseHeight, 0);

        float timer = 0f;
        while (timer < growthDuration)
        {
            float t = timer / growthDuration;
            plant.transform.position = Vector3.Lerp(startPos, endPos, t);
            timer += Time.deltaTime;
            yield return null;
        }

        // Swap plant with final vegetable
        GameObject finalVeg = GetVegetableForSeed();
        if (finalVeg != null)
        {
            Instantiate(finalVeg, plant.transform.position, Quaternion.identity);
        }

        Destroy(plant);
        Destroy(gameObject); // Destroy seed
    }

    private GameObject GetVegetableForSeed()
    {
        SeedType seed = GetComponent<SeedType>();
        if (seed == null) return null;

        switch (seed.vegType)
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
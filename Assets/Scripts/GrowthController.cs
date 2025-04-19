using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SeedType))]
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

    private SeedType.VegetableType seedTypeValue;
    private bool hasStartedGrowing = false;

    void Start()
    {
        seedTypeValue = GetComponent<SeedType>().seedType;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasStartedGrowing) return;

        if (collision.gameObject.CompareTag("SoilTile"))
        {
            Transform spawnPoint = collision.transform.Find("PlantSpawnPoint");
            if (spawnPoint != null)
            {
                hasStartedGrowing = true;
                StartCoroutine(GrowPlant(spawnPoint));
            }
            else
            {
                Debug.LogWarning("[GROWTH] No PlantSpawnPoint found on soil tile.");
            }
        }
    }

    IEnumerator GrowPlant(Transform spawnPoint)
    {
        GameObject plant = Instantiate(plantPrefab, spawnPoint.position, Quaternion.identity);
        Vector3 startPos = plant.transform.position - new Vector3(0, riseHeight, 0);
        Vector3 endPos = plant.transform.position;
        plant.transform.position = startPos;

        float timer = 0f;
        while (timer < growthDuration)
        {
            float t = timer / growthDuration;
            plant.transform.position = Vector3.Lerp(startPos, endPos, t);
            timer += Time.deltaTime;
            yield return null;
        }

        plant.transform.position = endPos;

        // 🌱 Replace plant with vegetable
        GameObject finalVeg = GetVegetableForSeed();
        if (finalVeg != null)
        {
            Instantiate(finalVeg, plant.transform.position, Quaternion.identity);
            // Keep the plant object for visual effect
        }

        Destroy(gameObject); // ✅ Remove the seed at the END
    }

    private GameObject GetVegetableForSeed()
    {
        switch (seedTypeValue)
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

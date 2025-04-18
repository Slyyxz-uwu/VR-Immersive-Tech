using UnityEngine;

public class GrowthController : MonoBehaviour
{
    [Header("Growth Settings")]
    public GameObject plantPrefab;
    public float baseGrowthTime = 20f;
    public float riseHeight = 0.2f;

    [Header("Final Vegetable Prefabs")]
    public GameObject carrotPrefab;
    public GameObject cucumberPrefab;
    public GameObject tomatoPrefab;
    public GameObject cabbagePrefab;
    public GameObject pepperPrefab;

    private GameObject growingPlant;
    private float currentGrowthTime;
    private bool isWatered = false;
    private bool isGrowing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isGrowing && other.CompareTag("Seed"))
        {
            string tag = other.gameObject.tag;
            GameObject finalPrefab = GetFinalPrefab(tag);

            if (finalPrefab != null)
            {
                Destroy(other.gameObject); // Remove seed
                StartCoroutine(GrowPlant(finalPrefab));
            }
        }
    }

    private GameObject GetFinalPrefab(string tag)
    {
        switch (tag)
        {
            case "Carrot": return carrotPrefab;
            case "Cucumber": return cucumberPrefab;
            case "Tomato": return tomatoPrefab;
            case "Cabbage": return cabbagePrefab;
            case "Pepper": return pepperPrefab;
            default: return null;
        }
    }

    private System.Collections.IEnumerator GrowPlant(GameObject finalPrefab)
    {
        isGrowing = true;
        currentGrowthTime = baseGrowthTime;

        // Spawn plant prefab rising from ground
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;
        growingPlant = Instantiate(plantPrefab, startPos, Quaternion.identity);

        float elapsed = 0f;
        while (elapsed < currentGrowthTime)
        {
            if (isWatered)
                elapsed += Time.deltaTime * 2f; // Watering speeds up growth
            else
                elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / currentGrowthTime);
            growingPlant.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        Destroy(growingPlant);
        Instantiate(finalPrefab, endPos, Quaternion.identity);
        isGrowing = false;
    }

    public void WaterSoil()
    {
        isWatered = true;
    }
}
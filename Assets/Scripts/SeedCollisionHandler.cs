using UnityEngine;

public class SeedCollisionHandler : MonoBehaviour
{
    private bool hasStarted = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasStarted) return;

        if (collision.collider.CompareTag("SoilTile"))
        {
            Debug.Log("[SEED DEBUG] SoilTile hit, attempting to grow.");

            var growth = gameObject.AddComponent<GrowthController>();
            growth.plantPrefab = GameObject.Find("Plant"); // Or use a public static reference or ScriptableObject

            // Assign vegetable prefabs (you can also do this via scriptable config)
            growth.carrotPrefab = GameObject.Find("Carrot");
            growth.cucumberPrefab = GameObject.Find("Cucumber Variant");
            growth.tomatoPrefab = GameObject.Find("Tomato Variant");
            growth.cabbagePrefab = GameObject.Find("Cabbage");
            growth.pepperPrefab = GameObject.Find("Pepper");

            hasStarted = true;
        }
    }
}
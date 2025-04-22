using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoupPot : MonoBehaviour
{
    public AudioSource cookingSound;
    public GameObject soupPrefab;
    public Transform spawnPoint;
    public float cookingTime = 2f;
    public Vector3 soupSpawnScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float animationDuration = 1.5f; // Smooth scaling time

    private HashSet<string> requiredIngredients = new HashSet<string> { "Tomato", "Pepper", "Cucumber" };
    private HashSet<string> currentIngredients = new HashSet<string>();
    private bool isCooking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (requiredIngredients.Contains(other.tag) && !isCooking)
        {
            currentIngredients.Add(other.tag);
            Destroy(other.gameObject);

            if (currentIngredients.SetEquals(requiredIngredients))
            {
                StartCoroutine(CookSoup());
            }
        }
    }

    private IEnumerator CookSoup()
    {
        isCooking = true;

        if (cookingSound != null)
            cookingSound.Play();

        yield return new WaitForSeconds(cookingTime);

        if (soupPrefab != null && spawnPoint != null)
        {
            GameObject soup = Instantiate(soupPrefab, spawnPoint.position, Quaternion.identity);
            soup.transform.localScale = Vector3.zero;

            float timer = 0f;
            while (timer < animationDuration)
            {
                timer += Time.deltaTime;
                float t = timer / animationDuration;
                soup.transform.localScale = Vector3.Lerp(Vector3.zero, soupSpawnScale, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }

            soup.transform.localScale = soupSpawnScale;
        }

        currentIngredients.Clear();
        isCooking = false;
    }
}
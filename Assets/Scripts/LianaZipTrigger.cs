using UnityEngine;
using System.Collections;

public class AutoVineZipline : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRig;
    public string playerTag = "Player1";

    [Header("Vine Points")]
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("Movement")]
    public float climbSpeed = 2f;

    [Header("Sound")]
    public AudioSource climbSound; // Assign in inspector

    private bool isClimbing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isClimbing || !other.CompareTag(playerTag)) return;

        float distToTop = Vector3.Distance(other.transform.position, topPoint.position);
        float distToBottom = Vector3.Distance(other.transform.position, bottomPoint.position);

        Vector3 target = (distToTop > distToBottom) ? topPoint.position : bottomPoint.position;

        StartCoroutine(MovePlayerTo(target));
    }

    private IEnumerator MovePlayerTo(Vector3 destination)
    {
        isClimbing = true;

        if (climbSound != null)
        {
            climbSound.loop = true;
            climbSound.Play();
        }

        while (Vector3.Distance(playerRig.position, destination) > 0.05f)
        {
            playerRig.position = Vector3.MoveTowards(playerRig.position, destination, climbSpeed * Time.deltaTime);
            yield return null;
        }

        if (climbSound != null)
        {
            climbSound.Stop();
        }

        isClimbing = false;
    }
}
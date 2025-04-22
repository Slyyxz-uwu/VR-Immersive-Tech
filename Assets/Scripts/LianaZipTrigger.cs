using UnityEngine;
using System.Collections;

public class AutoVineZipline : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRig;          // Reference to XR Rig
    public string playerTag = "Player1";

    [Header("Vine Points")]
    public Transform topPoint;           // Empty GameObject at top of vine
    public Transform bottomPoint;        // Empty GameObject at bottom of vine

    [Header("Movement")]
    public float climbSpeed = 2f;

    private bool isClimbing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isClimbing || !other.CompareTag(playerTag)) return;

        float distToTop = Vector3.Distance(other.transform.position, topPoint.position);
        float distToBottom = Vector3.Distance(other.transform.position, bottomPoint.position);

        Vector3 target = (distToTop > distToBottom) ? topPoint.position : bottomPoint.position;

        Debug.Log($"[ZIPLINE] Player entered. Heading to {(distToTop > distToBottom ? "Top" : "Bottom")}");
        StartCoroutine(MovePlayerTo(target));
    }

    private IEnumerator MovePlayerTo(Vector3 destination)
    {
        isClimbing = true;

        while (Vector3.Distance(playerRig.position, destination) > 0.05f)
        {
            playerRig.position = Vector3.MoveTowards(playerRig.position, destination, climbSpeed * Time.deltaTime);
            yield return null;
        }

        isClimbing = false;
        Debug.Log("[ZIPLINE] Player reached destination.");
    }
}
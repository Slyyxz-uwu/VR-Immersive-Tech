using UnityEngine;

public class AutoVineClimb : MonoBehaviour
{
    public Transform playerRig; // XR Origin
    public Transform targetPoint; // Where the player should end up
    public float climbSpeed = 2f;
    private bool isClimbing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isClimbing = true;
        }
    }

    private void Update()
    {
        if (isClimbing && playerRig != null && targetPoint != null)
        {
            playerRig.position = Vector3.MoveTowards(playerRig.position, targetPoint.position, climbSpeed * Time.deltaTime);

            if (Vector3.Distance(playerRig.position, targetPoint.position) < 0.05f)
            {
                isClimbing = false;
            }
        }
    }
}
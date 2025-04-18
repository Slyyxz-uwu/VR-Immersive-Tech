using UnityEngine;

public class SoilTile : MonoBehaviour
{
    private bool isWatered = false;
    private bool isHoled = false;

    // Call this when the soil is watered (e.g., by water particles)
    public void SetWatered(bool value)
    {
        isWatered = value;
    }

    // Call this when the shovel digs a hole
    public void SetHoled(bool value)
    {
        isHoled = value;
    }

    public bool IsWatered()
    {
        return isWatered;
    }

    public bool IsHoled()
    {
        return isHoled;
    }

    // Optional helper: Check if the soil is ready to plant
    public bool IsReadyForPlanting()
    {
        return isWatered && isHoled;
    }
}
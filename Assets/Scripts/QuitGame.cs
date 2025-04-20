using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit Game requested.");
        Application.Quit();

#if UNITY_EDITOR
        // Stop playing the scene in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

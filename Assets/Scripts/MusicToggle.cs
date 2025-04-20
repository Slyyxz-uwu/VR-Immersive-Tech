using UnityEngine;

public class MusicToggle : MonoBehaviour
{
    public AudioSource musicSource;

    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            musicSource.mute = !musicSource.mute;
        }
    }
}
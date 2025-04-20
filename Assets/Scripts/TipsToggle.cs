using UnityEngine;

public class TipsAudio : MonoBehaviour
{
    public AudioSource tutorialAudio;

    public void PlayTutorial()
    {
        if (tutorialAudio != null && !tutorialAudio.isPlaying)
        {
            tutorialAudio.Play();
        }
    }
}

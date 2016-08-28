using UnityEngine;
using System.Collections.Generic;

public class AudioRandomizer : MonoBehaviour
{
    public AudioSource Audio;

    public float Min = 0.75f;
    public float Max = 1.0f;

    public void Play()
    {
        if (Audio)
        {
            var random = Random.Range(Min, Max);
            Audio.pitch = random;
            Audio.Play();
        }
    }
}

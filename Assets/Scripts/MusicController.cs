using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    public AudioSource MainTheme;
    public AudioSource DeathClip;
    public AudioSource MysteryClip;

    public AudioMixer MainMusic;
    public AudioMixer DeathMusic;
    public AudioMixer Sound;
    
    public string MainMusicVolume;
    public string DeathMusicVolume;
    public string SoundVolume;

    public float MainFadeTime;
    public float DeathUnfadeTime;
    public float SoundFadeTime;
    
    public void Reset()
    {
        Sound.SetFloat(SoundVolume, LinearToDecibel(1));
        MainMusic.SetFloat(MainMusicVolume, LinearToDecibel(1));
        DeathMusic.SetFloat(DeathMusicVolume, LinearToDecibel(0));

        MainTheme.Play();
    }

    public void PlayMystery()
    {
        MysteryClip.Play();
    }

    public void FadeMain()
    {
        StartCoroutine(FadeMusic(MainMusic, MainMusicVolume, MainFadeTime));
    }

    public void FadeSound()
    {
        StartCoroutine(FadeMusic(Sound, SoundVolume, SoundFadeTime));
    }

    public void UnfadeDeath()
    {
        DeathClip.Play();
        StartCoroutine(UnfadeMusic(DeathMusic, DeathMusicVolume, DeathUnfadeTime));
    }

    IEnumerator UnfadeMusic(AudioMixer mixer, string param, float time)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;

            mixer.SetFloat(param, LinearToDecibel(elapsed / time));

            yield return null;
        }

        mixer.SetFloat(param, LinearToDecibel(1));
    }

    IEnumerator FadeMusic(AudioMixer mixer, string param, float time)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;

            mixer.SetFloat(param, LinearToDecibel(1 - (elapsed / time)));

            yield return null;
        }

        mixer.SetFloat(param, LinearToDecibel(0));
    }

    public static float LinearToDecibel(float linear)
    {
        float dB;

        if (Mathf.Abs(linear) > Mathf.Epsilon)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }

    public static float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);

        return linear;
    }
}

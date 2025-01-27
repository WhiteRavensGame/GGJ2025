using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;
    public AudioSource sfxSource;

    public AudioClip jumpAudioClip;
    public AudioClip yayAudioClip;
    public AudioClip deathAudioClip;
    public AudioClip bubblePopAudioClip;
    public AudioClip sparkleClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void PlayJumpSFX()
    {
        sfxSource.PlayOneShot(jumpAudioClip);
    }
    public void PlayYaySFX()
    {
        sfxSource.PlayOneShot(yayAudioClip);
    }
    public void PlayDeathSFX()
    {
        sfxSource.PlayOneShot(deathAudioClip);
    }
    public void PlayBubblePopSFX()
    {
        sfxSource.PlayOneShot(bubblePopAudioClip);
    }
    public void PlaySparkleSFX()
    {
        sfxSource.PlayOneShot(sparkleClip);
    }

}

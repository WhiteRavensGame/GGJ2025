using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip yayAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] private AudioClip bubblePopAudioClip;
    [SerializeField] private AudioClip sparkleClip;
    [SerializeField] private AudioClip waterSplashClip;

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
    public void PlayWaterSplashSFX()
    {
        sfxSource.PlayOneShot(waterSplashClip);
    }

}

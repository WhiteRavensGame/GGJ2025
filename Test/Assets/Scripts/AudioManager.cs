using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;

    public AudioClip jumpAudioClip;
    public AudioClip yayAudioClip;
    public AudioClip deathAudioClip;
    public AudioClip bubblePopAudioClip;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void PlayAudio()
    {

    }

}

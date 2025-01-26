using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;

    public AudioClip sampleclip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        Invoke("test", 3);
    }

    public void test()
    {
        Debug.Log("test");
        audioSource.PlayOneShot(sampleclip);
    }


    public void PlayAudio()
    {

    }

}

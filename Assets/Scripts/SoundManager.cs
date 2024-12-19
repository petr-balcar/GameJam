using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip footstepsClip1;
    public AudioClip footstepsClip2;
    public AudioClip footstepsClip3;
    public AudioClip burnClip;
    public AudioClip beepClip;
    public AudioClip bubbleClip;
    public AudioClip leafClip;
    public AudioClip winClip;
    
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
    }

    public void PlayFootstep()
    {
        if (audioSource.isPlaying) return;
        
        switch (Random.Range(1, 4))
        {
            case 1:
                audioSource.clip = footstepsClip1;
                break;
            case 2:
                audioSource.clip = footstepsClip2;
                break;
            case 3:
                audioSource.clip = footstepsClip3;
                break;
        }
        
        audioSource.Play();
    }

    public void PlayBurn()
    {
        audioSource.clip = burnClip;
        audioSource.Play();
    }

    public void PlayBeep()
    {
        audioSource.clip = beepClip;
        audioSource.pitch = Mathf.Pow(2, Random.Range(0, 5) / 12f);
        audioSource.Play();
    }

    public void PlayBubble()
    {
        audioSource.clip = bubbleClip;
        audioSource.Play();
    }

    public void PlayLeaf()
    {
        audioSource.clip = leafClip;
        audioSource.Play();
    }
    
    public void PlayWin()
    {
        audioSource.clip = winClip;
        audioSource.Play();
    }
}

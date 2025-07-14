using UnityEngine;

public class audioSpaner : MonoBehaviour
{
    public AudioSource audioSource;
    public static audioSpaner mainSpaner;
    private void OnEnable()
    {
        mainSpaner = this;
    }
    private void Awake()
    {
        mainSpaner = this;
    }
    public static void span(AudioClip audioClip)
    {
        if (mainSpaner.audioSource != null && audioClip != null)
        {
            var span = Instantiate(mainSpaner.audioSource.gameObject, mainSpaner.transform).GetComponent<AudioSource>();
            span.clip = audioClip;
            span.Play();
            Destroy(span.gameObject, audioClip.length);
        }
    }
    public void spanAudio(AudioClip audioClip)
    {
        if (mainSpaner.audioSource != null && audioClip != null)
        {
            var span = Instantiate(mainSpaner.audioSource.gameObject, mainSpaner.transform).GetComponent<AudioSource>();
            span.clip = audioClip;
            span.Play();
            Destroy(span.gameObject, audioClip.length);
        }
    }
}

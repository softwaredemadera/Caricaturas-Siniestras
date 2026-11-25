using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sonidos")]
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource mechanismSound;
    [SerializeField] private AudioSource recordingNoiseSound;
    [SerializeField] private AudioSource stepSound;

    [Header("Musica")]
    [SerializeField] private AudioSource suspenseMusic;
    [SerializeField] private AudioSource actionMusic;
    Coroutine stepCoroutine;
    private float stepSpeed = 1f;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeMusic(string musicName) //cambia la música según el nombre especificado
    {
        switch (musicName)
        {
            case "suspense":
                if (!suspenseMusic.isPlaying)
                {
                    actionMusic.Stop();
                    suspenseMusic.Play();
                }
                break;
            case "action":
                if (!actionMusic.isPlaying)
                {
                    suspenseMusic.Stop();
                    actionMusic.Play();
                }
                break;
        }
    }

    public void PlaySound(AudioSource audioSource, float duration) //reproduce un sonido y lo detiene después de la duración especificada
    {
        audioSource.Play();
        StartCoroutine(StopSoundAfterDuration(audioSource, duration));
    }

    public void PlaySound(AudioSource audioSource) //reproduce un sonido y lo detiene después de su duración
    {
        PlaySound(audioSource, audioSource.clip.length);
    }

    IEnumerator StopSoundAfterDuration(AudioSource audioSource, float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
    }

    public void IncreaseNoiseSound()
    {
        if (recordingNoiseSound.volume < 1f)
        {
            recordingNoiseSound.volume += 0.4f;
        }
    }

    public void PlaySound(string soundName) //reproduce un sonido según su nombre
    {
        switch (soundName)
        {
            case "hit":
                PlaySound(hitSound);
                break;
            case "mechanism":
                PlaySound(mechanismSound, 1f);
                break;
            case "recordingNoise":
                PlaySound(recordingNoiseSound);
                break;
        }
    }

    public void StartSteps(float speed)
    {
        stepSpeed = speed;         
        if (stepCoroutine == null)               
            stepCoroutine = StartCoroutine(CoroutineWalk());
    }

    public void StopSteps()
    {
        if (stepCoroutine == null) return;
        StopCoroutine(stepCoroutine);
        stepCoroutine = null;
    }

    IEnumerator CoroutineWalk()
    {
        while (true)
        {
            stepSound.pitch = Random.Range(0.9f, 1.1f);
            PlaySound(stepSound);
            yield return new WaitForSeconds(0.6f / stepSpeed);
        }
    }



}

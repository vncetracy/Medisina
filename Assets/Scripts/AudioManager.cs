
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string soundName;
        public AudioClip clip;

        [Range(0f, 10f)]
        public float volume = 10f;

        public bool loop;
    }

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    [Header("Sound Effects")]
    public List<Sound> sounds = new List<Sound>();

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;

        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    public void PlaySound(string soundName)
    {
        Sound sound = sounds.Find(s => s.soundName == soundName);

        if (sound == null || sound.clip == null)
        {
            Debug.LogWarning("Sound not found: " + soundName);
            return;
        }

        sfxSource.PlayOneShot(sound.clip, sound.volume);
    }
}

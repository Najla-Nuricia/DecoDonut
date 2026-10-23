using UnityEngine;
using System.Collections;

public class Musicmanager : MonoBehaviour
{
    public static Musicmanager Instance { get; private set; }

    private const string MusicEnabledKey = "MusicEnabled";

    [Header("BGM")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    [SerializeField, Min(0f)] private float fadeDuration = 1f;
    [SerializeField] private bool playOnStart = true;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private Coroutine fadeCoroutine;
    private bool musicEnabled = true;

    public bool IsMusicEnabled => musicEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateAudioSources();
        musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
    }

    private void Start()
    {
        if (playOnStart && backgroundMusic != null && musicEnabled)
            PlayMusicWithFadeIn();
    }

    private void CreateAudioSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = 0f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    public void PlayMusicWithFadeIn()
    {
        if (backgroundMusic == null)
        {
            Debug.LogWarning("BGM belum diisi pada Musicmanager.", this);
            return;
        }

        musicEnabled = true;
        bgmSource.clip = backgroundMusic;

        if (!bgmSource.isPlaying)
            bgmSource.Play();

        StartFade(bgmVolume);
    }

    public void ToggleMusic()
    {
        if (musicEnabled)
            DisableMusic();
        else
            EnableMusic();
    }

    public void EnableMusic()
    {
        musicEnabled = true;
        PlayerPrefs.SetInt(MusicEnabledKey, 1);
        PlayerPrefs.Save();
        PlayMusicWithFadeIn();
    }

    public void DisableMusic()
    {
        musicEnabled = false;
        PlayerPrefs.SetInt(MusicEnabledKey, 0);
        PlayerPrefs.Save();
        StartFade(0f, true);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButtonHoverSFX()
    {
        PlaySFX(buttonHoverSound);
    }

    public void PlayButtonClickSFX()
    {
        PlaySFX(buttonClickSound);
    }

    public void PlayWinSFX()
    {
        PlaySFX(winSound);
    }

    public void PlayLoseSFX()
    {
        PlaySFX(loseSound);
    }

    private void StartFade(float targetVolume, bool stopAfterFade = false)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMusic(targetVolume, stopAfterFade));
    }

    private IEnumerator FadeMusic(float targetVolume, bool stopAfterFade)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        if (fadeDuration <= 0f)
        {
            bgmSource.volume = targetVolume;
        }
        else
        {
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
                yield return null;
            }

            bgmSource.volume = targetVolume;
        }

        if (stopAfterFade)
            bgmSource.Stop();

        fadeCoroutine = null;
    }
}

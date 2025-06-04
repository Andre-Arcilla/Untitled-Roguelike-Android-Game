using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Initialize();
    }

    [System.Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Scene BGMs")]
    [SerializeField] private List<SceneBGM> sceneBGMList;

    private Dictionary<string, AudioClip> sceneBGMMap = new();

    [Header("SFX Clips")]
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip killSFX;
    [SerializeField] private AudioClip missSFX;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    private void Initialize()
    {
        foreach (var bgm in sceneBGMList)
        {
            if (!sceneBGMMap.ContainsKey(bgm.sceneName))
                sceneBGMMap.Add(bgm.sceneName, bgm.bgmClip);
        }
        musicSource.volume = 0.1f;
        sfxSource.volume = 1f;
    }

    public void LoadBGM(Scene scene)
    {
        if (sceneBGMMap.TryGetValue(scene.name, out AudioClip clip))
        {
            if (musicSource.clip != clip)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeToNewBGM(clip));
            }
        }

        // Set volume depending on scene
        if (scene.name.ToLower().Contains("combat"))
        {
            musicSource.volume = 0.3f; // Lower volume for combat
        }
        else
        {
            musicSource.volume = 1f; // Full volume otherwise
        }
    }

    private IEnumerator FadeToNewBGM(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.pitch = Random.Range(0.95f, 1.05f); // Slight pitch variation
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = 1f; // Reset to default to avoid affecting next sound
        }
    }

    public void PlayHitSFX() => PlaySFX(hitSFX);
    public void PlayKillSFX() => PlaySFX(killSFX);
    public void PlayMissSFX() => PlaySFX(missSFX);
}
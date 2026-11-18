using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("Global Track")]
    public AudioClip globalMusic; // ambient underwater sound, plays the whole game
    [Range(0f, 1f)] public float globalVolume = 0.4f;

    [Header("Crossfade")]
    public float fadeDuration = 2f;
    // Crossfade inspiration: https://discussions.unity.com/t/audio-effects-in-c/550562/2
    // We use a Coroutine instead of a timer — see Crossfade() below

    private AudioSource globalSource;

    private AudioSource[] regionSources = new AudioSource[2];

    // active is which of the two sources is "live" (0 or 1)
    private int active = 0;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        globalSource = gameObject.AddComponent<AudioSource>();
        globalSource.loop = true;
        globalSource.playOnAwake = false;
        globalSource.volume = globalVolume;

        for (int i = 0; i < 2; i++)
        {
            regionSources[i] = gameObject.AddComponent<AudioSource>();
            regionSources[i].loop = true;
            regionSources[i].playOnAwake = false;
            regionSources[i].volume = 0f;
        }
    }

    void Start()
    {
        if (globalMusic != null)
        {
            globalSource.clip = globalMusic;
            globalSource.Play();
        }
        else Debug.LogWarning("globalMusic is null — no ambient audio assigned!");
    }

    public void PlayRegion(MusicRegionData region)
    {
        if (region == null || region.music == null) return;

        //  the track will not restart if it's already playing here
        AudioSource current = regionSources[active];
        if (current.clip == region.music && current.isPlaying) return;

        // cancel crossfade thats already in progress before starting a new one
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Crossfade(region));
    }

    #region CHAT GENERATED
    private IEnumerator Crossfade(MusicRegionData region)
    {
        // 1 - active toggles between 0 and 1 without an if statement:
        // if active is 0, next is 1. If active is 1, next is 0.
        int next = 1 - active;

        regionSources[next].clip = region.music;
        regionSources[next].volume = 0f;
        regionSources[next].Play();

        float elapsed = 0f;
        float startVolume = regionSources[active].volume;

        // Each iteration of this loop runs on one frame then pauses (yield return null)
        // t goes from 0 to 1 over fadeDuration seconds
        // Mathf.Lerp(a, b, t): returns a value between a and b based on t
        //   t=0 gives a, t=1 gives b, t=0.5 gives the midpoint
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            // unscaledDeltaTime instead of deltaTime so music fades even when
            // timeScale = 0 (i.e. when a popup has paused the game)
            float t = elapsed / fadeDuration; // normalised 0-to-1 progress value

            regionSources[active].volume = Mathf.Lerp(startVolume, 0f, t); // fade out
            regionSources[next].volume = Mathf.Lerp(0f, region.volume, t); // fade in

            yield return null; // pause here, resume on the next frame
        }

        regionSources[active].Stop();
        regionSources[active].clip = null; // clear so clip comparisons stay accurate
        active = next;
    }
    #endregion
}
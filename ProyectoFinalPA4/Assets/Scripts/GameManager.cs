using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int coinCount = 0;
    public bool gameOver = false;
    public bool victory = false;

    // Audio components
    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    // Audio clips
    private AudioClip _bgmClip;
    private AudioClip _coinClip;

    private void Awake()
    {
        // Singleton pattern
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

        // Add audio sources
        _musicSource = gameObject.AddComponent<AudioSource>();
        _sfxSource = gameObject.AddComponent<AudioSource>();

        _musicSource.loop = true;
        _musicSource.volume = 0.35f;

        // Generate procedural audio clips
        _bgmClip = GenerateBGMClip();
        _coinClip = GenerateCoinSoundClip();
    }

    void Start()
    {
        // Play background music
        if (_musicSource != null && _bgmClip != null)
        {
            _musicSource.clip = _bgmClip;
            _musicSource.Play();
        }
    }

    public void AddCoin()
    {
        coinCount++;
        Debug.Log("Coins: " + coinCount);
    }

    public void SetVictory()
    {
        victory = true;
        gameOver = true;
        Debug.Log("Victory!");
    }

    public void SetDefeat()
    {
        victory = false;
        gameOver = true;
        Debug.Log("Defeat!");
    }

    public void PlayerHitByEnemy()
    {
        // When player is hit by enemy, treat as defeat
        SetDefeat();
    }

    public void PlayCoinSound()
    {
        if (_sfxSource != null && _coinClip != null)
        {
            _sfxSource.PlayOneShot(_coinClip);
        }
    }

    void Update()
    {
        // Restart with R key
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// Generates a soft ambient background music clip (loopable).
    /// </summary>
    private AudioClip GenerateBGMClip()
    {
        int sampleRate = 44100;
        int lengthSeconds = 2; // 2 seconds long, loopable
        int samples = sampleRate * lengthSeconds;
        float[] data = new float[samples * 2]; // stereo

        float frequency = 110.0f; // A2, low and soft
        float phase = 0f;

        for (int i = 0; i < samples; i++)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;
            if (phase > 2 * Mathf.PI) phase -= 2 * Mathf.PI;

            // Base sine wave
            float sample = Mathf.Sin(phase) * 0.2f;
            // Add a subtle second harmonic for richness
            sample += Mathf.Sin(phase * 2) * 0.05f;
            // Add a very subtle third harmonic
            sample += Mathf.Sin(phase * 3) * 0.02f;

            // Stereo
            data[i * 2] = sample;
            data[i * 2 + 1] = sample;
        }

        AudioClip clip = AudioClip.Create("BGMClip", samples, 2, sampleRate, true);
        clip.SetData(data, 0);
        return clip;
    }

    /// <summary>
    /// Generates a short bright coin collection sound (ascending frequency chirp).
    /// </summary>
    private AudioClip GenerateCoinSoundClip()
    {
        int sampleRate = 44100;
        float length = 0.12f; // seconds
        int samples = (int)(sampleRate * length);
        float[] data = new float[samples * 2]; // stereo

        float freqStart = 500f;
        float freqEnd = 1000f;
        float phase = 0f;

        for (int i = 0; i < samples; i++)
        {
            // Linear frequency sweep
            float t = (float)i / samples;
            float freq = Mathf.Lerp(freqStart, freqEnd, t);
            phase += 2 * Mathf.PI * freq / sampleRate;
            if (phase > 2 * Mathf.PI) phase -= 2 * Mathf.PI;

            float sample = Mathf.Sin(phase) * 0.5f; // amplitude 0.5 to avoid clipping
            data[i * 2] = sample;
            data[i * 2 + 1] = sample;
        }

        AudioClip clip = AudioClip.Create("CoinSound", samples, 2, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
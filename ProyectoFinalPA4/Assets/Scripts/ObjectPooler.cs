using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static ObjectPooler Instance;
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; // Important to exit if we destroy this duplicate
        }

        // Initialization of pools: ensure CoinParticles pool exists
        bool hasCoinParticles = false;
        foreach (var p in pools)
        {
            if (p.tag == "CoinParticles")
            {
                hasCoinParticles = true;
                break;
            }
        }
        if (!hasCoinParticles)
        {
            // Create the prefab for coin particles
            GameObject prefab = CreateCoinParticlePrefab();
            pools.Add(new Pool { tag = "CoinParticles", prefab = prefab, size = 5 });
        }
    }
    #endregion

    public List<Pool> pools;
    [System.NonSerialized] // <-- as required
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // Accept both "CoinParticle" and "CoinPartitions" as the same pool
        string actualTag = tag;
        if (tag == "CoinParticle")
        {
            actualTag = "CoinParticles";
        }

        if (!poolDictionary.ContainsKey(actualTag))
        {
            Debug.LogWarning("Pool with tag " + actualTag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[actualTag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        if (actualTag == "CoinParticles")
        {
            // For coin particles: activate, play, and return to pool after delay
            StartCoroutine(DisableParticleAfterDelay(objectToSpawn, 0.6f));
        }
        else
        {
            // For other pool types (if any), return to pool immediately
            poolDictionary[actualTag].Enqueue(objectToSpawn);
        }

        return objectToSpawn;
    }

    private IEnumerator DisableParticleAfterDelay(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
        poolDictionary["CoinParticles"].Enqueue(go);
    }

    /// <summary>
    /// Creates a prefab with a ParticleSystem configured as a golden spark explosion.
    /// </summary>
    private GameObject CreateCoinParticlePrefab()
    {
        GameObject go = new GameObject("CoinParticlePrefab");
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startSpeed = 3f;
        main.startSize = 0.15f;

        // Color: yellow/gold over lifetime
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.yellow, 0.0f),
                new GradientColorKey(new Color(1f, 0.84f, 0f), 0.5f), // golden
                new GradientColorKey(Color.yellow, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Emission: burst of 20 particles at t=0
        var emission = ps.emission;
        emission.enabled = true;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20)
        });

        // Ensure the renderer uses a visible material
        var rend = go.GetComponent<ParticleSystemRenderer>();
        if (rend != null)
        {
            // Use the default sprite shader which is visible and unlit
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = new Color(1f, 0.9f, 0.2f, 1f); // soft golden
            rend.material = mat;
            rend.renderMode = ParticleSystemRenderMode.Billboard;
        }

        // Keep the prefab inactive for pooling
        go.SetActive(false);
        return go;
    }
}
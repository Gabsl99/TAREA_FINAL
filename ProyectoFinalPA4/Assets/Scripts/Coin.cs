using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotate around Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Optional: floating animation
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Emit particle from ObjectPooler (using tag "CoinParticles")
            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.SpawnFromPool("CoinParticles", transform.position, Quaternion.identity);
            }

            // Play coin sound via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayCoinSound();
            }

            // Add coin to GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoin();
            }

            // Deactivate the coin
            gameObject.SetActive(false);
        }
    }
}
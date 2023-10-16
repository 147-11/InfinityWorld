using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public float spawnRadius = 5f;
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 30f;

    private bool isPowerUpActive = false;

    private void Start()
    {
        // Comienza la rutina de aparición de power-ups
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (true)
        {
            if (!isPowerUpActive)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector3 playerPosition = player.transform.position;
                    Vector3 spawnPosition = playerPosition + Random.insideUnitSphere * spawnRadius;
                    spawnPosition.y = 0.5f;

                    Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
                    isPowerUpActive = true;
                }
            }
            yield return null;
        }
    }

    // Llamado cuando el jugador recoge el power-up
    public void PowerUpCollected()
    {
        isPowerUpActive = false;
    }
}

using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float initialSpawnInterval = 2f;
    public float speedIncreaseRate = 0.1f;
    public float moveSpeed = 2f;
    public Transform playerTransform;
    public GameObject enemyPrefab;
    private int enemyCount = 0; // Contador de enemigos creados
    private float spawnInterval = 3.0f; // Intervalo de tiempo entre apariciones
    private int enemiesPerSpawn = 0; // Cantidad inicial de enemigos por aparición

    private float currentSpawnInterval;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        //InvokeRepeating(nameof(SpawnEnemy), 1f, currentSpawnInterval);
    }

    private void Update()
    {
        spawnInterval -= Time.deltaTime;
        if (spawnInterval <= 0)
        {
            spawnInterval = 3.0f; // Restablecer el intervalo de tiempo

            // Aumentar la cantidad de enemigos por aparición cada vez que aparecen
            enemiesPerSpawn++;

            // Llamar a la función para crear los enemigos al mismo tiempo
            SpawnEnemies();
        }

        MoveEnemy();
        UpdateSpawnInterval();
    }

    private void UpdateSpawnInterval()
    {
        currentSpawnInterval--;
        currentSpawnInterval = Mathf.Max(1f, currentSpawnInterval); // Ajustar el mínimo intervalo
    }

    private void SpawnEnemies()
    {
        if (playerTransform == null) return;

        Camera mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Generar múltiples enemigos al mismo tiempo
        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            Vector3 spawnPosition = CalculateSpawnPosition(cameraWidth, cameraHeight);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemyCount++; // Incrementar el contador de enemigos creados
        }
    }

    private Vector3 CalculateSpawnPosition(float cameraWidth, float cameraHeight)
    {
        Vector3 spawnPosition = Vector3.zero;

        bool spawnOnVerticalSide = Random.Range(0, 2) == 0;

        if (spawnOnVerticalSide)
        {
            float spawnX = Random.Range(-cameraWidth / 2f, cameraWidth / 2f);
            float spawnZ = Random.Range(0f, 1f) < 0.5f ? -cameraHeight / 2f : cameraHeight / 2f;
            spawnPosition = playerTransform.position + new Vector3(spawnX, 0f, spawnZ);
        }
        else
        {
            float spawnX = Random.Range(0f, 1f) < 0.5f ? -cameraWidth / 2f : cameraWidth / 2f;
            float spawnZ = Random.Range(-cameraHeight / 2f, cameraHeight / 2f);
            spawnPosition = playerTransform.position + new Vector3(spawnX, 0f, spawnZ);
        }

        return spawnPosition;
    }

    private void MoveEnemy()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);
    }
}

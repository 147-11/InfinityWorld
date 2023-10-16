using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float initialSpawnInterval = 2f;
    public float speedIncreaseRate = 0.00000000000000000000000000000000000000000000001f;
    public float moveSpeed = 2f;
    public Transform playerTransform;
    public GameObject enemyPrefab;
    public float maxEnemyDistance = 30f; // La distancia máxima a la que un enemigo puede alejarse del jugador antes de ser destruido

    private int enemyCount = 0; // Contador de enemigos creados
    private float spawnInterval = 3.0f; // Intervalo de tiempo entre apariciones
    private int enemiesPerSpawn = 0; // Cantidad inicial de enemigos por aparición
    private float currentSpawnInterval;
    private Camera mainCamera;
    private Vector3 cameraPosition;
    private float cameraHeight;
    private float cameraWidth;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        mainCamera = Camera.main;
        cameraHeight = mainCamera.orthographicSize * 2f;
        cameraWidth = cameraHeight * mainCamera.aspect;
        cameraPosition = mainCamera.transform.position;

        InvokeRepeating(nameof(SpawnEnemy), 1f, currentSpawnInterval);
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
            SpawnEnemy();
        }

        MoveEnemy();
        UpdateSpawnInterval();
        DestroyFarEnemies();
    }

    private void UpdateSpawnInterval()
    {
        currentSpawnInterval -= Time.deltaTime;
        currentSpawnInterval = Mathf.Max(1f, currentSpawnInterval); // Ajustar el mínimo intervalo
    }

    private void SpawnEnemy()
    {
        if (playerTransform == null) return;

        Vector3 spawnPosition = CalculateSpawnPosition();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemyCount++; // Incrementar el contador de enemigos creados
    }

    private Vector3 CalculateSpawnPosition()
    {
        Vector3 playerPosition = playerTransform.position;
        Vector3 playerDirection = playerPosition - cameraPosition;

        Vector3 spawnPosition = playerPosition + playerDirection.normalized * maxEnemyDistance;
        spawnPosition.y = 1f; // La altura a la que aparecerán los enemigos

        return spawnPosition;
    }

    private void MoveEnemy()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);
    }

    private void DestroyFarEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, playerTransform.position) > maxEnemyDistance)
            {
                Destroy(enemy);
            }
        }
    }
}

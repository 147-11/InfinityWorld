using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float initialSpawnInterval = 2f;
    public float speedIncreaseRate = 0.1f;
    public float moveSpeed = 2f;
    public Transform playerTransform;
    public GameObject enemyPrefab;

    private float currentSpawnInterval;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        InvokeRepeating(nameof(SpawnEnemy), 1f, currentSpawnInterval);
    }

    private void Update()
    {
        MoveEnemy();
        UpdateSpawnInterval();
    }

    private void UpdateSpawnInterval()
    {
        currentSpawnInterval --;
        currentSpawnInterval = Mathf.Max(-100f, currentSpawnInterval);
    }

    private void SpawnEnemy()
    {
        if (playerTransform == null) return;

        Camera mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 spawnPosition = CalculateSpawnPosition(cameraWidth, cameraHeight);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 CalculateSpawnPosition(float cameraWidth, float cameraHeight)
    {
        Vector3 spawnPosition = Vector3.zero;

        bool spawnOnVerticalSide = Random.Range(0, 2) == 0;

        if (spawnOnVerticalSide)
        {
            float spawnX = Random.Range(0f, 1f) < 0.5f ? -cameraWidth : cameraWidth;
            float spawnZ = Random.Range(-cameraHeight / 2f, cameraHeight / 2f);
            spawnPosition = new Vector3(spawnX, 0f, spawnZ);
        }
        else
        {
            float spawnX = Random.Range(-cameraWidth / 2f, cameraWidth / 2f);
            float spawnZ = Random.Range(0f, 1f) < 0.5f ? -cameraHeight : cameraHeight;
            spawnPosition = new Vector3(spawnX, 0f, spawnZ);
        }

        spawnPosition += playerTransform.position;
        return spawnPosition;
    }

    private void MoveEnemy()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);
    }
}

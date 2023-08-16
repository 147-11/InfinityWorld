using UnityEngine;
using System.Collections; // Agrega esta línea para acceder a las clases de IEnumerator


public class PowerUp : MonoBehaviour
{
    public float spawnInterval = 10f; // Intervalo de tiempo entre apariciones en segundos
    public GameObject powerUpPrefab; // Prefab del power up a instanciar
    public float spawnRadius = 5f; // Radio dentro del cual aparecerá el PowerUp cerca del jugador

    private void Start()
    {
        // Iniciar la rutina de aparición de power ups
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Encontrar el jugador con el tag "Player"
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Obtener la posición del jugador
                Vector3 playerPosition = player.transform.position;

                // Calcular una posición aleatoria dentro del radio cerca del jugador
                Vector3 spawnPosition = playerPosition + Random.insideUnitSphere * spawnRadius;
                spawnPosition.y = 0.5f; // Asegurarse de que el PowerUp esté a una altura constante

                // Instanciar el power up en la posición calculada
                Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}

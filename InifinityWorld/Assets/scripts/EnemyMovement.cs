using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public string playerTag = "Player"; // El tag del objeto del jugador
    public int damageAmount = 2;

    private Transform playerTransform;

    private void Start()
    {
        // Buscar el objeto del jugador por su tag y obtener su transform
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found with tag: " + playerTag);
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform != null)
        {
            // Obtener la dirección hacia el jugador
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

            // Mover el enemigo en dirección al jugador
            transform.Translate(directionToPlayer * moveSpeed * Time.fixedDeltaTime);
        }
    }
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag(playerTag))
    {
        // Si el proyectil impacta con el enemigo, reducir su vida
        HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);

            // Verificar si la vida del jugador ha llegado a cero
            if (enemyHealth.currentHealth <= 0)
            {
                // Reiniciar la escena
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}


}
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth; // Vida máxima del enemigo
    public int currentHealth; // Vida actual del enemigo

    private void Start()
    {
        currentHealth = maxHealth; // Configura la vida actual como la vida máxima al inicio
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; // Reducir la vida del enemigo por la cantidad de daño recibido

        if (currentHealth <= 0)
        {
            // Si la vida del enemigo llega a cero o menos, destruir el GameObject del enemigo
            Destroy(gameObject);
        }
    }
}

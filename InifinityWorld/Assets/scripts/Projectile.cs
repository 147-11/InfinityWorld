using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public int damageAmount = 2;
    public string enemyTag = "Enemy"; // Tag del enemigo a seguir y destruir

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destruir el proyectil después de cierto tiempo de vida
    }

    private void Update()
    {
        // Mover el proyectil hacia adelante en la dirección actual
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            // Si el proyectil impacta con el enemigo, reducir su vida
            HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }

            // Destruir el proyectil después de impactar
            Destroy(gameObject);
        }
    }
}

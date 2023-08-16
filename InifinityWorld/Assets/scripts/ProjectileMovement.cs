using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public string targetTag = "Player"; // Etiqueta del objeto alrededor del cual orbitará el proyectil
    public float orbitSpeed = 2f;
    private float angle = 0f;
    public float orbitRadius = 2f;

    private Transform targetTransform; // Referencia al objeto alrededor del cual orbitará el proyectil

    private void Start()
    {
        // Buscar el objeto objetivo usando la etiqueta targetTag
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            targetTransform = targetObject.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con la etiqueta " + targetTag + ". El proyectil no orbitará alrededor de ningún objeto.");
        }
        // No destruir el proyectil al tiempo
        Destroy(gameObject, Mathf.Infinity);
    }

    private void Update()
    {
        // Verificar si se encontró un objeto objetivo antes de intentar orbitar
        if (targetTransform != null)
        {
            // Incrementar el ángulo para que el proyectil orbite alrededor del objeto objetivo
            angle += orbitSpeed * Time.deltaTime;

            // Calcular la nueva posición del proyectil en la órbita
            Vector3 newPosition = targetTransform.position;
            newPosition.x += Mathf.Cos(angle) * orbitRadius;
            newPosition.z += Mathf.Sin(angle) * orbitRadius;

            // Actualizar la posición del proyectil
            transform.position = newPosition;
        }
    }
}

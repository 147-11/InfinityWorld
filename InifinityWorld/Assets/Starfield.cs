/* using System.Collections.Generic;
using UnityEngine;

public class Starfield : MonoBehaviour
{
    public GameObject starPrefab; // Prefab de una estrella
    public int starCount = 1000; // Cantidad de estrellas
    public float starfieldRadius = 100.0f; // Radio del área donde se generan las estrellas

    private List<GameObject> stars = new List<GameObject>();

    void Start()
    {
        // Genera estrellas aleatorias en el espacio
        for (int i = 0; i < starCount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * starfieldRadius;
            GameObject star = Instantiate(starPrefab, randomPos, Quaternion.identity);
            stars.Add(star);
        }
    }
} */
using System.Collections.Generic;
using UnityEngine;

public class Starfield : MonoBehaviour
{
    public ParticleSystem starParticles; // El sistema de partículas que representa las estrellas
    public Transform playerTransform;
    public float bufferDistance = 10f; // Margen de seguridad para generación y destrucción
    public float horizontalSpeed = 0.1f; // Velocidad horizontal de las estrellas

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleStars();
    }

    private void HandleStars()
    {
        if (playerTransform == null) return;

        // Ajusta la posición del sistema de partículas en X, siguiendo al jugador pero a una velocidad más lenta
        float newXPosition = playerTransform.position.x * horizontalSpeed;
        Vector3 particlePosition = new Vector3(newXPosition, playerTransform.position.y, starParticles.transform.position.z);
        starParticles.transform.position = particlePosition;

        // Detén o inicia el sistema de partículas según si está en la vista de la cámara
        if (!IsPositionInCameraView(particlePosition))
        {
            if (starParticles.isPlaying)
            {
                starParticles.Stop();
            }
        }
        else
        {
            if (!starParticles.isPlaying)
            {
                starParticles.Play();
            }
        }
    }

    private bool IsPositionInCameraView(Vector3 position)
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(position);
        return screenPos.x >= -bufferDistance && screenPos.x <= 1 + bufferDistance &&
               screenPos.y >= -bufferDistance && screenPos.y <= 1 + bufferDistance &&
               screenPos.z >= mainCamera.nearClipPlane;
    }
}

using UnityEngine;
using System.Collections;


public class Player : MonoBehaviour
{
    private float powerupDuration = 5.0f;
    //camara
    public float moveSpeed = 5f;

    private Camera mainCamera;
    private Transform playerTransform;

    private void CamPlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        playerTransform.Translate(movement * moveSpeed * Time.deltaTime);

        // Asegura que la cámara siempre esté centrada en el jugador en la posición X y Z,
        // y que esté a una distancia específica en la posición Y (elevación) para una perspectiva 3/4.
        mainCamera.transform.position = new Vector3(playerTransform.position.x, mainCamera.transform.position.y, playerTransform.position.z - 10f);
    }

    //jugador

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float shootInterval = 0.5f;
    //public float powerupDuration = 10f;
    public float powerupCooldown = 5f;
    public float powerupProjectileSpeedMultiplier = 2f;

    private bool canShoot = true;
    private bool hasPowerup = false;

    public float fireRate = 5f; // Tiempo en segundos entre cada disparo
    private float timeSinceLastShot = 0f;

    private int cont = 0; //contador de power up
    public float orbitDistance = 2f; // Distancia de órbita alrededor del jugador

    private void Update()
    {
        CamPlayer();
        MovePlayer();
        timeSinceLastShot += Time.deltaTime;

        if (hasPowerup)
        {
            if (cont == 1)
            {
                // Incrementar gradualmente la velocidad de disparo hasta llegar a minFireRate
                fireRate = Mathf.Max(0.1f, 0);
            }
            if (cont == 2)
            {
                // Llamar al método Shoot cada vez que se presione la tecla de disparo y haya pasado el tiempo requerido desde el último disparo
                if (canShoot && timeSinceLastShot >= fireRate)
                {
                    Shoot01();
                    timeSinceLastShot = 0f;
                }
            }
            if (cont == 3)
            {
                if (canShoot && timeSinceLastShot >= fireRate)
                {
                    Shoot02();
                    timeSinceLastShot = 0f;
                }
            }
            if (cont == 4)
            {
                if (canShoot && timeSinceLastShot >= fireRate)
                {
                    Shoot01();
                    Shoot02();
                    timeSinceLastShot = 0f;
                }
            }
            if (cont > 4)
            {
                cont = 2;
            }
        }

        // Control del tiempo entre disparos
        //timeSinceLastShot += Time.deltaTime;

        // Control del disparo
        if (canShoot && timeSinceLastShot >= fireRate)
        {
            Shoot();

            // Restablecer el tiempo desde el último disparo
            timeSinceLastShot = 0f;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        playerTransform = transform;

        InvokeRepeating("Shoot", 0f, shootInterval);
    }

    private void MovePlayer()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Verificar si se están presionando las teclas W, A, S o D
        if (Input.GetKey(KeyCode.W))
            verticalInput = 1f;
        if (Input.GetKey(KeyCode.S))
            verticalInput = -1f;
        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }


    private void Shoot()
    {
        if (canShoot /* && timeSinceLastShot >= fireRate */)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length > 0)
            {
                // Restablecer el tiempo desde el último disparo
                timeSinceLastShot = 0f;
                GameObject closestEnemy = enemies[0];
                float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

                foreach (GameObject enemy in enemies)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestEnemy = enemy;
                        closestDistance = distance;
                    }
                }

                Vector3 directionToEnemy = Vector3.Normalize(closestEnemy.transform.position - transform.position);
                Quaternion rotationToEnemy = Quaternion.LookRotation(directionToEnemy);

                Instantiate(projectilePrefab, projectileSpawnPoint.position, rotationToEnemy);
            }
        }
    }

    private void Shoot01()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            // Restablecer el tiempo desde el último disparo
            timeSinceLastShot = 0f;

            // Calcular el ángulo entre los proyectiles
            float angleStep = 360f / 6f;

            for (int i = 0; i < 6; i++)
            {
                // Calcular la dirección del proyectil
                Quaternion rotationToEnemy = Quaternion.Euler(0f, i * angleStep, 0f);
                Vector3 directionToEnemy = rotationToEnemy * Vector3.forward;

                // Instanciar el proyectil en la dirección calculada
                Instantiate(projectilePrefab, projectileSpawnPoint.position, rotationToEnemy);
            }
        }
    }

    private void Shoot02()
    {
        if (canShoot)
        {
            // Restablecer el tiempo desde el último disparo
            timeSinceLastShot = 0f;

            // Calcular la dirección hacia adelante del jugador
            Vector3 forwardDirection = transform.forward;

            // Calcular la posición inicial del proyectil en la órbita
            Vector3 startPosition1 = transform.position + Quaternion.Euler(0f, 90f, 0f) * forwardDirection * orbitDistance;
            Vector3 startPosition2 = transform.position + Quaternion.Euler(0f, -90f, 0f) * forwardDirection * orbitDistance;

            // Instanciar los proyectiles en las posiciones iniciales con los ángulos correctos
            GameObject projectile1 = Instantiate(projectilePrefab, startPosition1, Quaternion.identity);
            GameObject projectile2 = Instantiate(projectilePrefab, startPosition2, Quaternion.identity);

            // Asegurarnos de que los proyectiles tengan el componente ProjectileMovement adjunto y activado
            if (projectile1.TryGetComponent(out ProjectileMovement projectileMovement1))
            {
                if (!projectileMovement1.enabled)
                {
                    projectileMovement1.enabled = true;
                }
                projectileMovement1.orbitSpeed = 90; // Ajusta la velocidad de órbita según tu preferencia
            }

            if (projectile2.TryGetComponent(out ProjectileMovement projectileMovement2))
            {
                if (!projectileMovement2.enabled)
                {
                    projectileMovement2.enabled = true;
                }
                projectileMovement2.orbitSpeed = -90; // Ajusta la velocidad de órbita según tu preferencia
            }
        }
    }












    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            CollectPowerup();
            Destroy(other.gameObject);
            cont++;
        }
    }

    private void CollectPowerup()
    {
        hasPowerup = true;
        //powerupTimer = powerupDuration;
        //StartCoroutine(PowerupCooldown());
                // Busca el script del PowerUp en la escena y llama a PowerUpCollected()
        PowerUp powerUpScript = FindObjectOfType<PowerUp>();
        if (powerUpScript != null)
        {
            powerUpScript.PowerUpCollected();
        }
    }
    /*
        private void UpdatePowerup()
        {
            // Implement power-up behavior here, e.g., more powerful projectiles, shooting in all directions, etc.
            powerupTimer -= Time.deltaTime;

            if (powerupTimer <= 0f)
                hasPowerup = false;
        }

        private IEnumerator PowerupCooldown()
        {
            yield return new WaitForSeconds(powerupCooldown);
            hasPowerup = false;
        }*/
}

using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrainManager : MonoBehaviour
{
    public GameObject terrainPrefab;
    public Transform playerTransform;
    public float bufferDistance = 10f; // Margen de seguridad para generación y destrucción

    private Camera mainCamera;
    private List<GameObject> spawnedTerrains = new List<GameObject>();

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        if (playerTransform == null) return;

        // Obtener la cámara principal y sus límites
        float cameraHeight = mainCamera.orthographicSize * 2f + bufferDistance;
        float cameraWidth = cameraHeight * mainCamera.aspect + bufferDistance;

        float startOffsetX = Mathf.Floor((playerTransform.position.x - cameraWidth / 2f) / 10f) * 10f;
        float startOffsetZ = Mathf.Floor((playerTransform.position.z - (cameraHeight-30f) / 2f) / 10f) * 10f;

        for (float x = startOffsetX; x < startOffsetX + cameraWidth; x += 10f)
        {
            for (float z = startOffsetZ; z < startOffsetZ + cameraHeight; z += 10f)
            {
                Vector3 spawnPosition = new Vector3(x, terrainPrefab.transform.position.y, z);

                if (!IsTerrainSpawnedAtPosition(spawnPosition))
                {
                    GameObject terrain = Instantiate(terrainPrefab, spawnPosition, Quaternion.identity);
                    spawnedTerrains.Add(terrain);
                }
            }
        }

        // Destruir terrenos fuera de la vista
        List<GameObject> terrenosParaDestruir = new List<GameObject>();

        foreach (GameObject terrain in spawnedTerrains)
        {
            Vector3 position = terrain.transform.position;
            if (!IsPositionInCameraView(position))
            {
                terrenosParaDestruir.Add(terrain);
            }
        }

        foreach (GameObject terrainParaDestruir in terrenosParaDestruir)
        {
            spawnedTerrains.Remove(terrainParaDestruir);
            Destroy(terrainParaDestruir);
        }
    }

    private bool IsTerrainSpawnedAtPosition(Vector3 position)
    {
        foreach (GameObject terrain in spawnedTerrains)
        {
            if (Vector3.Distance(position, terrain.transform.position) < 10f)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPositionInCameraView(Vector3 position)
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(position);
        return screenPos.x >= -bufferDistance && screenPos.x <= 1 + bufferDistance &&
               screenPos.y >= -bufferDistance && screenPos.y <= 1 + bufferDistance &&
               screenPos.z >= mainCamera.nearClipPlane;
    }
}

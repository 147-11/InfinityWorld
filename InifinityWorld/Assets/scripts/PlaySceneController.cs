using System.Collections;
using UnityEngine;

public class PlaySceneController : MonoBehaviour
{
    public AudioSource audioSource; // Asegúrate de asignar el AudioSource desde el Inspector

    void Start()
    {
        // Obtén el índice actual de la canción desde PersistentManager
        int currentSongIndex = PersistentManager.Instance.GetCurrentSongIndex();
        
        // Cambia la canción actual
        StartCoroutine(ChangeSong(currentSongIndex));
    }

    IEnumerator ChangeSong(int songIndex)
    {
        // Detén la canción actual si está sonando
        audioSource.Stop();

        // Realiza aquí cualquier configuración necesaria antes de cambiar la canción

        // Simula una carga o configuración adicional si es necesario
        yield return new WaitForSeconds(1f);

        // Cambia la canción según el nuevo índice
        // Aquí debes tener lógica para cargar la nueva canción según el índice
        // Puedes usar un array de clips de audio o cualquier otra lógica que tengas
        AudioClip newSong = GetSongByIndex(songIndex);

        // Configura el nuevo clip en el AudioSource y comienza a reproducirlo
        audioSource.clip = newSong;
        audioSource.Play();
    }

    private AudioClip GetSongByIndex(int index)
    {
        // Aquí debes implementar la lógica para obtener el AudioClip según el índice
        // Puedes usar un array, un switch, o cualquier otra estructura de datos según tu diseño
        // Retorna el AudioClip correspondiente al índice proporcionado
        return null; // Reemplaza esto con la lógica real
    }
}

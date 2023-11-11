using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    void Start()
    {
        // Obtén el índice actual de la canción desde PersistentManager
        int storedIndex = PersistentManager.Instance.GetCurrentSongIndex();

        // Carga la escena actualmente activa
        Scene currentScene = SceneManager.GetActiveScene();

        // Comprueba el nombre de la escena actual y realiza acciones en consecuencia
        if (currentScene.name == "Menu")
        {
            // Carga la primera canción solo si no hay una canción almacenada
            if (storedIndex == -1)
            {
                LoadSong(0);
            }
            // De lo contrario, sigue reproduciendo la canción almacenada
            else
            {
                LoadSong(storedIndex);
            }
        }
        else if (currentScene.name == "Configuracion")
        {
            // No es necesario hacer nada específico en la escena de configuración
            // ya que queremos que la canción continúe reproduciéndose sin interrupciones.
        }
        else if (currentScene.name == "Play")
        {
            // Carga la segunda canción al cambiar a la escena "Play"
            LoadSong(1);
        }
    }

    // Método para cargar una canción según el índice
    private void LoadSong(int songIndex)
    {
        // Almacena el nuevo índice en PersistentManager
        PersistentManager.Instance.SetCurrentSongIndex(songIndex);

        // Carga la escena actual para reiniciar la reproducción de la canción
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance { get; private set; }
    private int currentSongIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu" || scene.name == "Configuracion")
        {
            currentSongIndex = 0;
        }
        else if (scene.name == "Play")
        {
            currentSongIndex = 1;
        }
    }

    public int GetCurrentSongIndex()
    {
        return currentSongIndex;
    }

    public void SetCurrentSongIndex(int index)
    {
        currentSongIndex = index;
    }
}

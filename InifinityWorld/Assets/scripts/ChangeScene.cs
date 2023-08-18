using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ChangeScene : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void ChangeToPlayScene()
    {
        SceneManager.LoadScene("Configuracion");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Reducir el tamaño del botón al estar encima
        transform.localScale = originalScale * 0.9f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Restaurar el tamaño original al salir del botón
        transform.localScale = originalScale;
    }
}


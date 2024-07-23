using UnityEngine;
using UnityEngine.UI;

public class FadeInController : MonoBehaviour
{
    [SerializeField] private float delayBeforeFade = 1f; // Retraso antes de que comience el fade
    [SerializeField] private float fadeDuration = 1f; // Duracion de la transicion de fade en segundos
    [SerializeField] private Image imageToFade; // Imagen del canvas que se desvanecera

    private float currentAlpha = 1f; // Alfa actual de la imagen
    private float fadeTimer = 0f; // Temporizador para el fade
    private bool delayCompleted = false; // Flag para indicar si el retraso ha terminado

    private void Start()
    {
        if (imageToFade == null)
        {
            Debug.LogError("No se ha asignado una imagen para desvanecer en el inspector.");
            enabled = false;
            return;
        }

        // Inicia el fade con el retraso
        SetImageAlpha(1f); // Asegurarse de que la imagen este completamente visible antes de empezar el fade
        fadeTimer = fadeDuration;
        Invoke("StartFade", delayBeforeFade);
    }

    private void Update()
    {
        // Si el retraso aun no ha terminado, no pasa nada
        if (!delayCompleted)
            return;

        // Reducir el temporizador
        fadeTimer -= Time.deltaTime;

        // Calcular el nuevo alfa
        currentAlpha = fadeTimer / fadeDuration;
        SetImageAlpha(currentAlpha);

        // Desactivar el componente una vez que el fade este completo
        if (fadeTimer <= 0f)
        {
            enabled = false;
        }
    }

    private void StartFade()
    {
        delayCompleted = true;
    }

    private void SetImageAlpha(float alpha)
    {
        Color imageColor = imageToFade.color;
        imageColor.a = alpha;
        imageToFade.color = imageColor;
    }
}

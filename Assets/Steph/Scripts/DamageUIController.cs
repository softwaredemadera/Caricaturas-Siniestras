using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DamageUIController : MonoBehaviour
{
    // Referencias a las imágenes de grietas
    [SerializeField] private CanvasGroup golpeado;
    [SerializeField] private CanvasGroup herido;
    [SerializeField] private CanvasGroup muerto;

    // Duración del fundido de entrada, en segundos
    [SerializeField] private float fadeDuration = 0.2f;

    // Arreglo que nos permite manejar las tres grietas en un ciclo
    private CanvasGroup[] grietas;
    // Referencia a la corutina
    private Coroutine aparicion;

    private void Awake()
    {
        // Agrupamos las referencias en un array
        grietas = new[] { golpeado, herido, muerto };

        // Al iniciar, ocultamos todas las grietas
        HideStatus();
    }

    // Estos métodos permiten llamar cada estado por separado
    public void ShowGolpeado() => Show(golpeado);
    public void ShowHerido() => Show(herido);
    public void ShowMuerto() => Show(muerto);

    public void HideStatus()
    {
        if (aparicion != null)
        {
            // Si hay un fundido en curso, lo detenemos
            StopCoroutine(aparicion);
            aparicion = null;
        }

        // Apagamos cada objeto y dejamos su opacidad en cero
        foreach (CanvasGroup grieta in grietas)
        {
            // Evita un error si olvidamos asignar alguna referencia
            if (grieta == null) continue;
            grieta.alpha = 0f;

            grieta.gameObject.SetActive(false);
        }
    }

    private void Show(CanvasGroup objective)
    {
        // Si no asignamos la grieta en el Inspector, salimos del método
        if (objective == null) return;

        // Detenemos un fundido anterior para que no se mezcle con este
        if (aparicion != null)
            StopCoroutine(aparicion);

        // Apagamos las otras grietas y preparamos la elegida
        foreach (CanvasGroup grieta in grietas)
        {
            if (grieta == null) continue;

            bool isObjective = grieta == objective;

            // Solo se activa el objeto elegido
            grieta.gameObject.SetActive(isObjective);
            // Empezamos su opacidad en cero
            grieta.alpha = 0f;
        }

        // Iniciamos el fundido de entrada y guardamos su referencia
        aparicion = StartCoroutine(Aparecer(objective));

    }

    private IEnumerator Aparecer(CanvasGroup objective)
    {
        // Si la duración es cero o menor, mostramos la grieta al instante
        if (fadeDuration <= 0f)
        {
            objective.alpha = 1f;
            aparicion = null;
            yield break;
        }

        float tiempo = 0f;

        // Aumentamos poco a poco la opacidad hasta que termine el fundido
        while (tiempo < fadeDuration)
        {
            // Usamos tiempo no escalado para que funcione incluso en pausa
            // Modificar en caso de que no funcione como se espera
            tiempo += Time.unscaledDeltaTime;

            // Convertimos el tiempo transcurrido en opacidad de 0 a 1
            objective.alpha = Mathf.Clamp01(tiempo / fadeDuration);
            // Esperamos al siguiente fotograma para continuar
            yield return null;
        }

        // Aseguramos que termine totalmente visible.
        objective.alpha = 1f;
        // Indicamos que ya no hay una corrutina de aparición activa
        aparicion = null;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelTransition : MonoBehaviour
{

    // Duración efecto fade
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float clickDelay = 0.12f;

    public Coroutine transition;

    // Prevents that there are multiple transitions at the same time
    public void ShowPanel(GameObject[] panels, GameObject panelToShow)
    {
        if (transition != null)
            StopCoroutine(transition);

        transition = StartCoroutine(ShowPanelRoutine(panels, panelToShow));
    }

    // Coroutine to handle panel transitions with fade effect
    private IEnumerator ShowPanelRoutine(GameObject[] panels, GameObject panelToShow)
    {
        // Wait for a short delay
        yield return new WaitForSecondsRealtime(clickDelay);

        // Call event system if there is one
        EventSystem.current?.SetSelectedGameObject(null);

        // Activate the target panel
        foreach (GameObject panel in panels)
            panel.SetActive(panel == panelToShow);

        // Get the CanvasGroup component for fade effect
        CanvasGroup group = panelToShow.GetComponent<CanvasGroup>();

        // Panel starts fully transparent for fade-in effect
        group.alpha = 0f;

        // Fade in the panel over the specified duration
        float elapsedTime = 0f;

        // Repeat until the elapsed time reaches the fade duration
        while (elapsedTime < fadeDuration)
        {
            // Increment elapsed time since the last frame
            elapsedTime += Time.unscaledDeltaTime;
            // Transform the fade to a value between 0 and 1 based on elapsed time
            group.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            // Wait for the next frame to update the fade effect
            yield return null;
        }

        // Ensure the panel is fully visible at the end of the fade
        group.alpha = 1f;

        transition = null;
    }
}

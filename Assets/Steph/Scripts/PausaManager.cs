using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausaManager : MonoBehaviour
{
    // UI Elements
    [Header("Pause Menu Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject confirmMainMenuPanel;

    // Duración efecto fade
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float clickDelay = 0.12f;
    private Coroutine panelTransition;

    private bool isPaused;

    private void Awake()
    {
        
    }

    private void Start()
    {
        // Restablece el tiempo normal
        Time.timeScale = 1f;
        // Juego comienza sin estar pausado
        isPaused = false;
    }

    // Methods to show different panels
    
    public void ShowSettingsPanel() => ShowPanel(settingsPanel);
    public void ShowControlsPanel() => ShowPanel(controlsPanel);
    public void ShowRestartPanel() => ShowPanel(restartPanel);
    public void ShowConfirmMainMenuPanel() => ShowPanel(confirmMainMenuPanel);

    public void ConfirmRestart()
    {
        // Tiempo se restaura para evitar inicio en pausa
        Time.timeScale = 1f;
        // Carga la escena inicial
        SceneManager.LoadScene(1);
    }

    // Regresa al menú principal
    public void ReturnToMainMenu()
    {
        // Restablece el tiempo para que el menú no quede pausado
        Time.timeScale = 1f;

        // Carga la escena del main menu
        SceneManager.LoadScene(0);
    }
    
    // Pause the game
    public void ShowPauseMenu()
    {
        ShowPanel(pauseMenuPanel);

        isPaused = true;

        // Detiene el tiempo
        Time.timeScale = 0f;
    }

    public void BackToPause()
    {
        ShowPanel(pauseMenuPanel);
    }
    public void BackToGame()
    {
        // Restaura el tiempo
        Time.timeScale = 1f;
        isPaused = false;

        //Oculta el menú de pausa
        pauseMenuPanel.SetActive(false);
    }

    // Prevents that there are multiple transitions at the same time
    private void ShowPanel(GameObject panelToShow)
    {
        if (panelTransition != null)
            StopCoroutine(panelTransition);

        panelTransition = StartCoroutine(ShowPanelWithFade(panelToShow));
    }

    private void Update()
    {
        // Comprueba que haya teclado disponible
        if (Keyboard.current == null)
            return;

        // Continua solo si ESC es presionado
        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        // Comprueba que los otros paneles no esten activos
        // Evita que ESC se active desde controles o confirmación 
        
        if ((settingsPanel != null && settingsPanel.activeSelf) ||
    (controlsPanel != null && controlsPanel.activeSelf) ||
    (restartPanel != null && restartPanel.activeSelf) ||
    (confirmMainMenuPanel != null && confirmMainMenuPanel.activeSelf))
        
        {
            return;
        }
        
        {
            // Si el juego esta en Pausa lo reanuda
            if (isPaused)
            {
                BackToGame();
            }

            // Si el juego esta activo lo pausa
            else
            {
                ShowPauseMenu();
            }
        }
    }
    // Coroutine to handle panel transitions with fade effect
    private IEnumerator ShowPanelWithFade(GameObject panelToShow)
    {
        // Wait for a short delay
        yield return new WaitForSecondsRealtime(clickDelay);

        // Call event system if there is one
        EventSystem.current?.SetSelectedGameObject(null);

        // List of all panels to manage their visibility
        GameObject[] panels = { pauseMenuPanel, restartPanel, settingsPanel, controlsPanel, confirmMainMenuPanel };

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

        panelTransition = null;
    }

}

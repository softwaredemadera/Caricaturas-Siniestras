using System.Collections;
using Unity.VisualScripting;
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

    // Referencia al HUD
    [Header("Referencia al canvas InGame")]
    [SerializeField] private GameObject hudInGame;

    private bool isPaused;

    // Referencia a script PanelTransition
    [SerializeField] private PanelTransition panelTransition;
    private GameObject[] panels;

    private bool isGameOver;

    // Lista de paneles
    private void Awake()
    {
        panels = new[]
        { pauseMenuPanel,
          restartPanel,
          settingsPanel,
          controlsPanel,
          confirmMainMenuPanel
        };
    }

    private void Start()
    {
        // Restablece el tiempo normal
        Time.timeScale = 1f;
        // Juego comienza sin estar pausado
        isPaused = false;
    }

    private void ShowPanel(GameObject panelToShow)
    {
        panelTransition.ShowPanel(panels, panelToShow);
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
        SceneManager.LoadScene("PausaMenu");
    }

    // Regresa al menú principal
    public void ReturnToMainMenu()
    {
        // Restablece el tiempo para que el menú no quede pausado
        Time.timeScale = 1f;

        // Carga la escena del main menu
        SceneManager.LoadScene("MainMenu");
    }
    
    // Pause the game
    public void ShowPauseMenu()
    {

        hudInGame.SetActive(false);

        // Libera el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        // Vuelve a bloquear el curso
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Restaura el tiempo
        Time.timeScale = 1f;
        isPaused = false;

        //Oculta el menú de pausa
        pauseMenuPanel.SetActive(false);
        hudInGame.SetActive(true);
    }

    //Metodo para final de la partida
    public void ShowGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        //Comprueba que no se pueda pausar mientras GameOver esta activo
        if (isGameOver)
            return;

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

        if (isPaused)
        {
            BackToGame();
        }
        else
        {
            ShowPauseMenu();
        }
    }

}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // UI Elements
    [Header("Main Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject creditsPanel;

    // Referencia a script PanelTransition
    [SerializeField] private PanelTransition panelTransition;

    private GameObject[] panels;

    // Lista de paneles
    private void Awake()
    {
        panels = new[]
        {
            mainMenuPanel, settingsPanel, controlsPanel, creditsPanel
        };
    }

    private void Start()
    {
        // Menu principal inicia activo
        ShowMainMenu();
    }

    private void ShowPanel(GameObject panelToShow)
    {
        panelTransition.ShowPanel(panels, panelToShow);
    }

    // Methods to show different panels
    public void ShowMainMenu() => ShowPanel(mainMenuPanel);
    public void ShowSettingsPanel() => ShowPanel(settingsPanel);
    public void ShowControlsPanel() => ShowPanel(controlsPanel);
    public void ShowCreditsPanel() => ShowPanel(creditsPanel);


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}

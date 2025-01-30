using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;

    /// <summary>
    /// Start the game
    /// </summary>
    public void OnStartClicked() 
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Open the setting menu
    /// </summary>
    public void OnSettingClicked()
    {
        SettingsPanel.SetActive(true);
    }

    /// <summary>
    /// Close the setting menu
    /// </summary>
    public void OnSettingClose() 
    {
        SettingsPanel.SetActive(false);
    }

    /// <summary>
    /// Open the credit menu
    /// </summary>
    public void OnCreditClicked() 
    {
        CreditsPanel.SetActive(true);
    }

    /// <summary>
    /// close the credit menu
    /// </summary>
    public void OnCreditClose() 
    {
        CreditsPanel.SetActive(false);
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void OnExitClicked() {
        Application.Quit();
    }

    /// <summary>
    /// Go back to main menu
    /// </summary>
    public void OnMainMenuClicked() {
        SceneManager.LoadScene(0);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    private async void Awake()
    {
        await Controls.LoadControls();
    }

    public void NewGame()
    {
        GameProgress.Reset();
        SceneManager.LoadScene("World");
    }

    public void Continue()
    {
        SceneManager.LoadScene("World");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Exit()
    {
        Application.Quit();
    }
}

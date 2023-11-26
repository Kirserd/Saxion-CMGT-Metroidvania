using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsActions : MonoBehaviour
{
    public void Controls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}

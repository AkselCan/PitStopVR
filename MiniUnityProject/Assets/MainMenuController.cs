using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Call this method when Start button is pressed
    public void StartGame()
    {
        Debug.Log("Loading Main scene...");
        SceneManager.LoadScene("Main");
    }

    // If you want to use the scene index instead of name:
    // SceneManager.LoadScene(1); // where 1 is the index in Build Settings
}
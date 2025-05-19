using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool PauseGame = false;
    public GameObject PauseGameMenu;
    public GameObject MainMenupref;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(PauseGame)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseGameMenu.SetActive(false);
        Time.timeScale = 1.0f;
        PauseGame = false;
        Debug.Log("Game Paused! TimeScale: " + Time.timeScale); // Должно быть 0

    }
    public void Pause()
    {
        PauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
        Debug.Log("Game Paused! TimeScale: " + Time.timeScale); // Должно быть 0
    }
    public void LoadMenu()
    {
        PauseGame = false;
        PauseGameMenu.SetActive(false);
        Instantiate(MainMenupref);
        Time.timeScale = 1.0f;
    }
}

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool PauseGame = false;
    public GameObject PauseGameMenu;
    public GameObject MainMenupref;
    public Action<bool> OnPause;
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
        OnPause?.Invoke(PauseGame);
        Debug.Log("Game Paused! TimeScale: " + Time.timeScale); // ������ ���� 0

    }
    public void Pause()
    {
        PauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
        OnPause?.Invoke(PauseGame);
        Debug.Log("Game Paused! TimeScale: " + Time.timeScale); // ������ ���� 0
    }
    public void LoadMenu()
    {
        Bootstrap.Instance.UnloadGame();
        PauseGame = false;
        PauseGameMenu.SetActive(false);
        Instantiate(MainMenupref);
        Time.timeScale = 1.0f;
    }
}

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MainMenu : MonoBehaviour
{
    public void Start_OnCLick()
    {
        Destroy(gameObject);
        //Тут надо вызывать окно с выбором кол-ва игроков ников и тд, потом формируем это
        Player[] players =
        {
            new Player("Maxon", Color.red),
            new Player("Asan", Color.blue),
            new Player("Skiba", Color.green),
        };
        //и вызываем это
        Bootstrap.Instance.GameInit(players);
    }
    public void Quit_OnClick()
    {
        Debug.Log("Closed");
        Application.Quit();
    }

}

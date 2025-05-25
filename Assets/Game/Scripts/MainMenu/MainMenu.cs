using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject playerMenuPref;
    public GameObject playerMenu;
    public void Start_OnCLick()
    {
        Destroy(gameObject);
        GameObject playerMenu = Instantiate(playerMenuPref);
        //Тут надо вызывать окно с выбором кол-ва игроков ников и тд, потом формируем это
    }
    public void Quit_OnClick()
    {
        Debug.Log("Closed");
        Application.Quit();
    }

}

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MainMenu : MonoBehaviour
{
    public void Quit_OnClick()
    {
        Debug.Log("Closed");
        Application.Quit();
    }

}

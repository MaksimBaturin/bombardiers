using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bootstrap: MonoBehaviour
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private GameObject inGameUIPrefab;
    [SerializeField] private GameObject tankControllerPrefab;
    [SerializeField] private GameController gameControllerPrefab;
    [SerializeField] private WindController windControllerPrefab;
    
    public static Bootstrap Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void GameInit(Player[] players)
    {
        StartCoroutine(EnableLoadingScreen());

        Instantiate(terrainPrefab);

        for (int i = 0; i < players.Length; i++)
        {
            float positionX = Random.Range(-Camera.main.rect.width / 2, Camera.main.rect.width / 2);
            float positionY = 50f;

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(positionX, positionY), -Vector2.up, 500, LayerMask.GetMask("Ground"));

            if (hit)
            {
                players[i].tank = Instantiate(tankPrefab);
                players[i].tank.transform.position = hit.transform.position + new Vector3(0, 20f);
            }
        }
        GameController gameController = Instantiate(gameControllerPrefab);
        Instantiate(windControllerPrefab);
        gameController.StartGame(players);
    }

    private IEnumerator EnableLoadingScreen()
    {
        //Включаем экран загрузки
        yield return new WaitForSeconds(2);
        //Выключаем экран загрузки
    }
}

public struct Player
{
    public string name;
    public Color color;
    public GameObject tank;

    public Player(string name, Color color)
    {
        this.name = name;
        this.color = color;
        this.tank = null;
    }
}

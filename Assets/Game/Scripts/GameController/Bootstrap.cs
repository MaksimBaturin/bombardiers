using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Scripts;

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

    private void Start()
    {
        Player[] players =
        {
            new Player("Maxon", Color.red),
            new Player("Asan", Color.blue),
            new Player("Skiba", Color.green),
        };

        GameInit(players);
    }

    public void GameInit(Player[] players)
    {

        StartCoroutine(EnableLoadingScreen());

        Instantiate(terrainPrefab);

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

            // Теперь можно получить размеры
            float width = topRight.x - bottomLeft.x;
            
            float positionX = Random.Range(-width / 2, width / 2);
            float positionY = 50f;

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(positionX, positionY), -Vector2.up, 500, LayerMask.GetMask("Ground"));

            if (hit)
            { 
                players[i].Tank = Instantiate(tankPrefab).GetComponent<TankModel>();
                Debug.Log(players[i].Tank.gameObject.GetInstanceID());
                players[i].Tank.transform.position = hit.point + new Vector2(0, 30f);
            }
        }
        GameController gameController = Instantiate(gameControllerPrefab);
        Instantiate(tankControllerPrefab).GetComponent<TankController>().UITransform = Instantiate(inGameUIPrefab).transform;
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

public class Player
{
    public string name;
    public Color color;
    private TankModel tank;
    public TankModel Tank 
    { 
        get => tank; 
        set
        {
            if (value == null)
            {
                tank.OnDeath -=TankDeath;
            }
            tank = value;
            if (tank != null)
            {
                tank.OnDeath += TankDeath;
            }
        }
    }
    public bool isAlive;

    public Player(string name, Color color)
    {
        this.name = name;
        this.color = color;
        this.tank = null;
        this.isAlive = true;
    }

    private void TankDeath()
    {
        Tank = null;
        isAlive = false;
    }
}

using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Scripts;
using static UnityEditor.U2D.ScriptablePacker;


public class Bootstrap: MonoBehaviour
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private GameObject inGameUIPrefab;
    [SerializeField] private TankController tankControllerPrefab;
    [SerializeField] private GameObject MainMenuPrefab;
    [SerializeField] private GameObject GlobalUIPrefab;
    [SerializeField] private GameObject EventPrefab;
    [SerializeField] private GameController gameControllerPrefab;
    [SerializeField] private WindController windControllerPrefab;

    private System.Collections.Generic.List<GameObject> gameObjects = new System.Collections.Generic.List<GameObject>();
    
    public static Bootstrap Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MainMenuInit();
    }

    public void MainMenuInit()
    {
        //Instantiate(EventPrefab);
        Instantiate(MainMenuPrefab);
    }

    public void UnloadGame()
    {
        foreach (GameObject obj in gameObjects)
        {
            Destroy(obj?.gameObject);
        }
    }

    public void GameInit(Player[] players)
    {
        
        StartCoroutine(EnableLoadingScreen());
        
        GameObject inGameUI = Instantiate(inGameUIPrefab);
        gameObjects.Add(inGameUI);
        
        GameObject globalUI = Instantiate(GlobalUIPrefab);
        gameObjects.Add(globalUI);

        gameObjects.Add(Instantiate(terrainPrefab));
        

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
                gameObjects.Add(players[i].Tank.gameObject);
                Debug.Log(players[i].Tank.gameObject.GetInstanceID());
                players[i].Tank.transform.position = hit.point + new Vector2(0, 30f);

                // Создаем HealthBar для танка
                CreateTankHealthBar(players[i].Tank, players[i].color);
            }
        }
        GameController gameController = Instantiate(gameControllerPrefab);
        gameObjects.Add(gameController.gameObject);
        
        TankController tankController = Instantiate(tankControllerPrefab);
        tankController.UITransform = inGameUI.transform;
        gameObjects.Add(tankController.gameObject);
        globalUI.GetComponent<PauseMenu>().OnPause += tankController.OnPause;
        
        WindController windController = Instantiate(windControllerPrefab);
        gameObjects.Add(windController.gameObject);
        
        gameController.StartGame(players);
    }

    private void CreateTankHealthBar(TankModel tank, Color playerColor)
    {
        GameObject healthBarObj = Instantiate(inGameUIPrefab);
        healthBarObj.transform.SetParent(tank.transform);

        healthBarObj.transform.localScale = Vector3.one * 0.05f;

        HealthBar healthBar = healthBarObj.GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.tank = tank; // Привязываем танк к HealthBar
            healthBar.imgObj.color = playerColor;
            healthBar.Active();
        }

        gameObjects.Add(healthBarObj);
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

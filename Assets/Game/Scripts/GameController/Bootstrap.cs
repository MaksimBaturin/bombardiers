using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Scripts;
using static UnityEditor.U2D.ScriptablePacker;
using Debug = UnityEngine.Debug;


public class Bootstrap: MonoBehaviour
{
    [SerializeField] private GameObject tankGreenPrefab;
    [SerializeField] private GameObject tankRedPrefab;
    [SerializeField] private GameObject tankBluePrefab;
    [SerializeField] private GameObject tankYellowPrefab;
    
    [SerializeField] private GameObject terrainPrefab;
    [SerializeField] private GameObject inGameUIPrefab;
    [SerializeField] private TankController tankControllerPrefab;
    [SerializeField] private GameObject MainMenuPrefab;
    [SerializeField] private GameObject GlobalUIPrefab;
    [SerializeField] private GameObject EventPrefab;
    [SerializeField] private GameController gameControllerPrefab;
    [SerializeField] private WindController windControllerPrefab;

    public static Player[] playersGlob;

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

        playersGlob = players;
        
        StartCoroutine(EnableLoadingScreen());
        
        GameObject inGameUI = Instantiate(inGameUIPrefab);
        gameObjects.Add(inGameUI);
        
        GameObject globalUI = Instantiate(GlobalUIPrefab);
        gameObjects.Add(globalUI);

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        // Теперь можно получить размеры
        float width = topRight.x - bottomLeft.x;
        
        GameObject terrain = Instantiate(terrainPrefab);
        gameObjects.Add(terrain.gameObject);
        terrain.GetComponent<TerrainGenerator>().terrainLength = width;
        Vector3 TerrainPos = terrain.transform.position;
        TerrainPos.x = bottomLeft.x;
        TerrainPos.y -= 10f;
        terrain.transform.position = TerrainPos;
        terrain.GetComponent<TerrainGenerator>().GenerateTerrain();
        
        for (int i = 0; i < players.Length; i++)
        {
            bool positionFound = false;
            int attempts = 0;
            const int maxAttempts = 100;
            const float minDistance = 50f;

            while (!positionFound && attempts < maxAttempts)
            {
                attempts++;
        
                
                float positionX = Random.Range(-width / 2, width / 2);
                float positionY = 50f;

                RaycastHit2D hit = Physics2D.Raycast(new Vector2(positionX, positionY), -Vector2.up, 500, LayerMask.GetMask("Ground"));

                if (hit)
                {
                    Vector2 spawnPoint = hit.point + new Vector2(0, 30f);
            
                    
                    bool tooClose = false;
                    foreach (var player in players)
                    {
                        if (player.Tank != null && Vector2.Distance(player.Tank.transform.position, spawnPoint) < minDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
            
                    
                    if (!tooClose)
                    {
                        if (players[i].color == Color.green)
                        {
                            players[i].Tank = Instantiate(tankGreenPrefab).GetComponent<TankModel>();
                        }
                        else if (players[i].color == Color.red)
                        {
                            players[i].Tank = Instantiate(tankRedPrefab).GetComponent<TankModel>();
                        }
                        else if (players[i].color == Color.yellow)
                        {
                            players[i].Tank = Instantiate(tankYellowPrefab).GetComponent<TankModel>();
                        }
                        else
                        {
                            players[i].Tank = Instantiate(tankBluePrefab).GetComponent<TankModel>();
                        }
                        gameObjects.Add(players[i].Tank.gameObject);
                        Debug.Log(players[i].Tank.gameObject.GetInstanceID());
                        players[i].Tank.transform.position = spawnPoint;

                        // Создаем HealthBar для танка
                        CreateTankHealthBar(players[i].Tank, players[i].color, players[i].name);
                
                        positionFound = true;
                    }
                }
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

    private void CreateTankHealthBar(TankModel tank, Color playerColor, string name)
    {
        GameObject healthBarObj = Instantiate(inGameUIPrefab);
        healthBarObj.transform.SetParent(tank.transform);

        healthBarObj.transform.localScale = Vector3.one * 0.025f;

        HealthBar healthBar = healthBarObj.GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.tank = tank; // Привязываем танк к HealthBar
            healthBar.imgObj.color = playerColor;
            healthBar.SetName(name, playerColor);
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

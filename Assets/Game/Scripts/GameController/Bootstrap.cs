using Game.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap: MonoBehaviour
{
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject terrainPrefab; //временно
    [SerializeField] private GameObject inGameUIPrefab;
    [SerializeField] private GameObject tankController;
    private GameController gameController;

    public void GameInit(List<Player> players)
    {
        StartCoroutine(EnableLoadingScreen());

        Instantiate(terrainPrefab);

        for (int i = 0; i < players.Count; i++)
        {
            float positionX = Random.Range(-Camera.main.rect.width / 2, Camera.main.rect.width / 2);
            float positionY = 50f;

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(positionX, positionY), -Vector2.up, 500, LayerMask.GetMask("Ground"));

            if (hit)
            {
                players[i].tank = Instantiate(tankPrefab);
                players[i].tank.transform.position = hit.transform.position + new Vector3(0, 20f);
            }

            gameController = new GameController(players);
            gameController.StartGame();
        }

    }

    private IEnumerator EnableLoadingScreen()
    {
        //Тут включаем загрузочный экран
        yield return new WaitForSeconds(2);
        //Тут его выключаем
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

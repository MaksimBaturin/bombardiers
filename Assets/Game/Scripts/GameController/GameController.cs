using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;
using Unity.VisualScripting;

public class GameController: MonoBehaviour
{
    private List<Player> playersTurnQueue;
    private Player currentPlayer;
    //private UIController uiController;
    
    private int windTurnCounter = 0;
    [SerializeField] private int maxWindTurnCount = 2;
    [SerializeField] private int MaxWindForce;
    public static GameController Instance { get; private set; }
    public void Awake()
    {
        Instance = this;
    }

    public void StartGame(Player[] players)
    {
        playersTurnQueue = new List<Player>(players);
        ShufflePlayersTurn();
            
        currentPlayer = playersTurnQueue[0];
        TankController.Instance.Tank = currentPlayer.Tank;

        CalculateWind();
        windTurnCounter++;
        //uiController - показываем чей ход
    }

    public void changePlayerTurn()
    {
        List<Player> alivePlayers = new List<Player>(playersTurnQueue.Count);
        foreach (Player player in playersTurnQueue)
        {
            if (player.isAlive) alivePlayers.Add(player);
        }
        foreach (Player player in alivePlayers) Debug.Log($"name: {player.name}");

        if (alivePlayers.Count == 1)
        {
            Debug.Log($"Игрок {alivePlayers[0].name} победил!");
                
            return;
        }
        
        int currentPlayerIndex = playersTurnQueue.IndexOf(currentPlayer);
        int nextPlayerIndex = currentPlayerIndex;
        int attempts = 0;
        
        do
        {
            nextPlayerIndex = (nextPlayerIndex + 1) % playersTurnQueue.Count;
            attempts++;
            
            if (attempts > playersTurnQueue.Count * 2)
            {
                Debug.LogError("Не удалось найти следующего живого игрока!");
                return;
            }
        }
        while (!playersTurnQueue[nextPlayerIndex].isAlive);
        
        currentPlayer = playersTurnQueue[nextPlayerIndex];
        TankController.Instance.Tank = currentPlayer.Tank;
        Debug.Log($"Ход игрока: {currentPlayer.name}");

        windTurnCounter++;
        if (windTurnCounter > maxWindTurnCount)
        {
            windTurnCounter = 0;
            CalculateWind();
        }
        
    }
    
    private void CalculateWind()
    {
        System.Random rnd = new System.Random();
        float windForce = rnd.Next(0, MaxWindForce+1);
        int windDir = rnd.Next(0, 2);
        WindController.Instance.windStrength = windForce/5;
        if (windDir == 0) WindController.Instance.windDirection = -1;
        else WindController.Instance.windDirection = 1;
        Debug.Log("ветер: " + WindController.Instance.windDirection * WindController.Instance.windStrength);
    }
    private void ShufflePlayersTurn()
    {
        for (int i = playersTurnQueue.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            
            Player temp = playersTurnQueue[i];
            playersTurnQueue[i] = playersTurnQueue[randomIndex];
            playersTurnQueue[randomIndex] = temp;
        }
    }
}

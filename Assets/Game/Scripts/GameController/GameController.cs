using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts;

public class GameController: MonoBehaviour
{
    private List<Player> playersTurnQueue;
    private Player currentPlayer;
    //private UIController uiController;
    
    private int windTurnCounter;
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
      
        //uiController - показываем чей ход
    }

    public void changePlayerTurn()
    {
        var alivePlayers = playersTurnQueue.Where(p => p.isAlive).ToList();
        
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
        
        Debug.Log("Players turn order after shuffling:");
        foreach (Player player in playersTurnQueue)
        {
            Debug.Log(player.name);
        }
    }
}

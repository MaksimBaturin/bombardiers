using UnityEngine;
using System.Collections.Generic;
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
        int currentPlayerIndex = playersTurnQueue.IndexOf(currentPlayer);
        int nextPlayerIndex;
        if (currentPlayerIndex == playersTurnQueue.Count - 1)
        {
            nextPlayerIndex = 0;
        }
        else
        {
            nextPlayerIndex = currentPlayerIndex + 1;
        }
        currentPlayer = playersTurnQueue[nextPlayerIndex];
        TankController.Instance.Tank = currentPlayer.Tank;
        //показываем чей ход и меняем ветер
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

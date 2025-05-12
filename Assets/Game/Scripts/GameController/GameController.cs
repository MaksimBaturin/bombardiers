using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class GameController
{
    private Queue<Player> playersQueue;
    
    public GameController(List<Player> players)
    {
        playersQueue = new Queue<Player>(players);

    }

    public void StartGame()
    {

    }


    private void ShufflePlayersTurn()
    {

    }
}

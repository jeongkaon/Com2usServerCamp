using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class GameBoard
{
    bool[,] board = new bool[19, 19];
    List<Player> playerList = new List<Player>();


    public void SetPlayer(string sessionId, string userId, bool turn)
    {
        playerList.Add(new Player(sessionId, userId, turn));
    }



    public void SetBoard(int x, int y)
    {
        //마우스로 들어옴 걍 걸러져서 따로 확인안해도됨
        board[x, y] = true;

    }


    public void CheckWin(int row, int col, char stone)
    {
    }
}

    




public class Player
{
    public string UserID { get; private set; }
    public string NetSessionID { get; private set; }

    bool IsTurn { get; set; } = false;

    int PassCount { get; set; } = 0;      //넘어간거


    public Player(string userID, string netSessionID,bool turn)
    {
        UserID = userID;
        NetSessionID = netSessionID;
        IsTurn = turn;
        PassCount = 0;
    }

    public void Set(string userId, string sessionId)
    {
        UserID = userId;
        NetSessionID=sessionId;
    }


}
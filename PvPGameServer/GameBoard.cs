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
        // 가로 확인
        //int horizontalCount = 1;
        //for (int c = col - 1; c >= 0 && board[row, c] == stone; c--) // 왼쪽으로 탐색
        //    horizontalCount++;
        //for (int c = col + 1; c < COLS && board[row, c] == stone; c++) // 오른쪽으로 탐색
        //    horizontalCount++;
        //if (horizontalCount >= 5)
        //    return true;

        //// 세로 확인
        //int verticalCount = 1;
        //for (int r = row - 1; r >= 0 && board[r, col] == stone; r--) // 위쪽으로 탐색
        //    verticalCount++;
        //for (int r = row + 1; r < ROWS && board[r, col] == stone; r++) // 아래쪽으로 탐색
        //    verticalCount++;
        //if (verticalCount >= 5)
        //    return true;

        //// 대각선(왼쪽 상단에서 오른쪽 하단) 확인
        //int diagonalCount1 = 1;
        //for (int r = row - 1, c = col - 1; r >= 0 && c >= 0 && board[r, c] == stone; r--, c--) // 왼쪽 상단으로 탐색
        //    diagonalCount1++;
        //for (int r = row + 1, c = col + 1; r < ROWS && c < COLS && board[r, c] == stone; r++, c++) // 오른쪽 하단으로 탐색
        //    diagonalCount1++;
        //if (diagonalCount1 >= 5)
        //    return true;

        //// 대각선(오른쪽 상단에서 왼쪽 하단) 확인
        //int diagonalCount2 = 1;
        //for (int r = row - 1, c = col + 1; r >= 0 && c < COLS && board[r, c] == stone; r--, c++) // 오른쪽 상단으로 탐색
        //    diagonalCount2++;
        //for (int r = row + 1, c = col - 1; r < ROWS && c >= 0 && board[r, c] == stone; r++, c--) // 왼쪽 하단으로 탐색
        //    diagonalCount2++;
        //if (diagonalCount2 >= 5)
        //    return true;

        //return false;
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
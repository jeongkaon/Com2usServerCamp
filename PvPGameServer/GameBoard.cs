using MemoryPack;
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
    public int RoomNumber;

    byte[,] board = new byte[19, 19];
    List<Player> PlayerList = new List<Player>();
    
    bool GameEnd = false;

    STONE_TYPE CurType = STONE_TYPE.NONE;

    
    public static Func<string, byte[], bool> NetworkSendFunc;

    public GameBoard(int roomNumber, Func<string, byte[], bool> func)
    {
        RoomNumber = roomNumber;
        NetworkSendFunc = func;
    }

    
    //여기를 수정해야한다!
    public void CheckTime()
    {
        //경과시간 체크하기

    }



    public string GetUserIdByPlayerType(STONE_TYPE type)
    {

        foreach(var player in PlayerList)
        {
            if(player.PlayerType == type)
            {
                return player.UserID;
            }
        }
        return null;
    }

    public bool CheckIsFull()
    {
        return (PlayerList.Count == 2);

    }

    public void ClearBoard()
    {
        board.Initialize();
        PlayerList.Clear();
    }

    public STONE_TYPE SetPlayer(string sessionId, string userId)
    {
        if (PlayerList.Count() == 0)
        {
            PlayerList.Add(new Player(sessionId, userId, STONE_TYPE.BLACK));
            return STONE_TYPE.BLACK;

        }
        else
        {
            PlayerList.Add(new Player(sessionId, userId, STONE_TYPE.WHITE));
            return STONE_TYPE.WHITE;
        }
    }

 

    public void CheckBaord(STONE_TYPE cur,int x, int y)
    {
        board[x, y] = (byte)cur;
        CurType = cur;

        NotifyPutOmok(x, y);

        if (CheckBoardEnd(x, y) == true)
        {
            var ID = GetUserIdByPlayerType(CurType);
            //색으로 보내주는게 더 나을지도??
            NotifyWinner(ID);
            ClearBoard();
        }
    }

    public void NotifyPutOmok(int x, int y)
    {

        var packet = new NftPutOmok()
        {
            mok = CurType,
            PosX = x,
            PosY = y
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_PUT_OMOK);

        Broadcast("", sendPacket);

    }

    public void NotifyWinner(string id)
    {
        var packet = new NtfOmokWinner()
        {
            UserId = id
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTR_WINNER_OMOK);

        Broadcast("", sendPacket);

    }

    public void Broadcast(string excludeNetSessionID, byte[] sendPacket)
    {
        foreach (var player in PlayerList)
        {
            if (player.NetSessionID == excludeNetSessionID)
            {
                continue;
            }

            NetworkSendFunc(player.NetSessionID, sendPacket);
        }
    }


    //오목룰 체크
    public bool CheckBoardEnd(int x, int y)
    {
        if (CheckCol(x, y) == 5)        // 같은 돌 개수가 5개면 (6목이상이면 게임 계속) 
        {
            return true;
        }

        else if (CheckRow(x, y) == 5)
        {
            return true;
        }

        else if (CheckDiagonal(x, y) == 5)
        {
            return true;
        }

        else if (CheckReversDiagonal(x, y) == 5)
        {
            return true;
        }

        return false;
    }
    int CheckCol(int x, int y)      // ㅡ 확인
    {
        int SameCount = 1;

        for (int i = 1; i <= 5; i++)
        {
            if (x + i <= 18 && board[x + i, y] == board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (x - i >= 0 && board[x - i, y] == board[x, y])
                SameCount++;

            else
                break;
        }

        return SameCount;
    }
    int CheckRow(int x, int y)      // | 확인
    {
        int SameCount = 1;

        for (int i = 1; i <= 5; i++)
        {
            if (y + i <= 18 && board[x, y + i] == board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (y - i >= 0 && board[x, y - i] == board[x, y])
                SameCount++;

            else
                break;
        }

        return SameCount;
    }

    int CheckDiagonal(int x, int y)      // / 확인
    {
        int SameCount = 1;

        for (int i = 1; i <= 5; i++)
        {
            if (x + i <= 18 && y - i >= 0 && board[x + i, y - i] == board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (x - i >= 0 && y + i <= 18 && board[x - i, y + i] == board[x, y])
                SameCount++;

            else
                break;
        }

        return SameCount;
    }
    int CheckReversDiagonal(int x, int y)     // ＼ 확인
    {
        int SameCount = 1;

        for (int i = 1; i <= 5; i++)
        {
            if (x + i <= 18 && y + i <= 18 && board[x + i, y + i] == board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (x - i >= 0 && y - i >= 0 && board[x - i, y - i] == board[x, y])
                SameCount++;

            else
                break;
        }

        return SameCount;
    }


}






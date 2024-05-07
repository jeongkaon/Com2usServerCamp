﻿using MemoryPack;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PvPGameServer;

public class GameBoard
{
    public int RoomNumber;
    byte[,] board = new byte[19, 19];
    Dictionary<STONE_TYPE, Player> PlayerList = null;
    STONE_TYPE CurType = STONE_TYPE.NONE;
 
    DateTime TimeoutCheckTime;

    //걍DB테스트임
    GameDB db = new GameDB();

    public static Func<string, byte[], bool> NetworkSendFunc;
    public GameBoard(int roomNumber, Func<string, byte[], bool> func)
    {
        RoomNumber = roomNumber;
        NetworkSendFunc = func;
        PlayerList = new Dictionary<STONE_TYPE, Player>();
    }
    public STONE_TYPE SetPlayer(string sessionId, string userId)
    {
        if (PlayerList.Count() == 0)
        {
            PlayerList.Add(STONE_TYPE.BLACK, new Player(sessionId, userId, STONE_TYPE.BLACK));
            return STONE_TYPE.BLACK;

        }
        else
        {
            PlayerList.Add(STONE_TYPE.WHITE, new Player(sessionId, userId, STONE_TYPE.BLACK));
            return STONE_TYPE.WHITE;
        }
    }
    public void GameStart()
    {
        CurType = STONE_TYPE.BLACK;
        SetTimeoutCheckTime(DateTime.Now);

    }
    public void EndGame(STONE_TYPE win)
    {
        NotifyWinner(win);
        ClearBoard();
    }

    public bool TimeOutCheck(DateTime time, int TimeSpan)
    {
        if(CurType == STONE_TYPE.NONE)
        {
            return false;
        }

        var diff = time - TimeoutCheckTime;
        if (diff.TotalMilliseconds> TimeSpan)
        {
            PlayerList[CurType].AddPassCount();
            return true;
        }

        return false;
        
    }
    public void SetTimeoutCheckTime(DateTime time)
    {
        TimeoutCheckTime = time;
    }

    public void TurnChange()
    {
        if (CurType == STONE_TYPE.BLACK)
        {
            CurType = STONE_TYPE.WHITE;
        }
        else
        {
            CurType = STONE_TYPE.BLACK;
        }

        SetTimeoutCheckTime(DateTime.Now);

    }
    public int ReadyPlayerCount()
    {
        return PlayerList.Count();
    }

    public STONE_TYPE CheckPassCount()
    {
        var player = PlayerList[CurType];
        if(player.CheckPassCount() == true)
        {
            return player.PlayerType;
        }
        return STONE_TYPE.NONE;

    }

    public void CheckBaord(STONE_TYPE cur, int x, int y)
    {
        if (cur != CurType)
        {
            return;
        }

        if (board[x, y] != 0)
        {
            return;
        }

        board[x, y] = (byte)cur;

        NotifyPutOmok(x, y);


        if (CheckBoardEnd(x, y) == true)
        {
            EndGame(CurType);
            return;
        }

        TurnChange();

    }
    public void ClearBoard()
    {
        Array.Clear(board, 0, board.Length);
        PlayerList.Clear();
        CurType = STONE_TYPE.NONE;
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

    public void NotifyWinner(STONE_TYPE win)
    {
        db.UpdateWinScore("kaon");

        var packet = new NtfOmokWinner()
        {
            WinStone = win
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTR_WINNER_OMOK);

        Broadcast("", sendPacket);

    }

    public void NotifyTimeOut()
    {
        var packet = new NtfTimeOut()
        {
            Usertype = CurType
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet); 
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_TIMEOUT_OMOK);

        Broadcast("", sendPacket);
    }

    public void Broadcast(string excludeNetSessionID, byte[] sendPacket)
    {
        foreach (var player in PlayerList)
        {
            if (player.Value.NetSessionID == excludeNetSessionID)
            {
                continue;
            }

            NetworkSendFunc(player.Value.NetSessionID, sendPacket);
        }
    }


    //오목로직
    public bool CheckBoardEnd(int x, int y)
    {
        const int TestCount = 2;

        if (CheckCol(x, y) == TestCount)        
        {
            return true;
        }

        else if (CheckRow(x, y) == TestCount)
        {
            return true;
        }

        else if (CheckDiagonal(x, y) == TestCount)
        {
            return true;
        }

        else if (CheckReversDiagonal(x, y) == TestCount)
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






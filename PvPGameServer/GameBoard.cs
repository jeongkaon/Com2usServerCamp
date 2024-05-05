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
    Dictionary<STONE_TYPE, Player> PlayerList = null;
    STONE_TYPE CurType = STONE_TYPE.NONE;

    Timer TimeOutCheckTimer = null;
    
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

        //확인 위해 10초로 짧게 설정해놓음
        TimeOutCheckTimer = new Timer(TimeOutCheck, null, 10000, 10000);
    }
    public void EndGame(string id)
    {
        NotifyWinner(id);
        ClearBoard();

    }

    public void TimeOutCheck(object? state)
    {
        // 이 함수 불렸으면 타임아웃이다.
        if (PlayerList[CurType].CheckPassCount())
        {
            //게임끝해줘야한다.
            //승자를 알려주자.
            
        }
        NotifyTimeOut();
        TurnChange();
    }
    public void StopCheckTiemer()
    {
        TimeOutCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);

    }
    public void RestartCheckTimer()
    {
        TimeOutCheckTimer.Change(10000, 10000);

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

    }
    public bool CheckIsFull()
    {
        return (PlayerList.Count == 2);

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
            EndGame(PlayerList[CurType].UserID);
            return;
        }
        StopCheckTiemer();
        TurnChange();
        RestartCheckTimer();

    }
    public void ClearBoard()
    {
        TimeOutCheckTimer.Dispose();
        TimeOutCheckTimer = null;
        board.Initialize();
        PlayerList.Clear();

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

        //내부로도 보내서 usermagr에서 승리카운트 올려??
        //DB도 연결해야하눈데??
        //일단 클라부터 해결하자

    }

    public void NotifyTimeOut()
    {
        var packet = new NtfTimeOut()
        {
            mok = CurType
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

        if (CheckCol(x, y) == TestCount)        // 같은 돌 개수가 5개면 (6목이상이면 게임 계속) 
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






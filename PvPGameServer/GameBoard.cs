using MemoryPack;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PvPGameServer;

public class GameBoard
{
    const int Size = 19;
    byte[,] _board = new byte[Size, Size];
    List<Player> _players = null;

    StoneType _curType = StoneType.None;
    DateTime _timeoutCheckTime;

    public int RoomNumber;
    public static Func<string, byte[], bool> NetworkSendFunc;

    public static Action<string, GameResult> UpdateUserGameDataFunc;
    public static Action<MemoryPackBinaryRequestInfo> DistributeInnerDB;  
    public static Action<int> AddEmptyRoomListAction;


    public GameBoard(int roomNumber)
    {
        RoomNumber = roomNumber;
        _players = new List<Player>();
    }
    public StoneType SetPlayer(string sessionId, string userId)
    {
        if (_players.Count() == 0)
        {
            _players.Add(new Player(sessionId, userId, StoneType.Black));
            return StoneType.Black;

        }
        else
        {
            _players.Add(new Player(sessionId, userId, StoneType.White));
            return StoneType.White;
        }
    }
    public void GameStart()
    {
         _curType = StoneType.Black;
        SetTimeoutCheckTime(DateTime.Now);

    }
    public void EndGame(StoneType win)
    {
        SavePlayerGameData(win);
        NotifyWinner(win);
        UpdateGameDataInDB();
        ClearBoard();
    }

    public bool TimeOutCheck(DateTime time, int tmeSpan)
    {
        if(_curType == StoneType.None)
        {
            return false;
        }

        var diff = time - _timeoutCheckTime;
        if (diff.TotalMilliseconds> tmeSpan)
        {
            int idx = (int)_curType - 1;
            _players[idx].AddPassCount();
            return true;
        }

        return false;
        
    }
    public void SetTimeoutCheckTime(DateTime time)
    {
        _timeoutCheckTime = time;
    }

    public void TurnChange()
    {
        if (_curType == StoneType.Black)
        {
            _curType = StoneType.White;
        }
        else
        {
            _curType = StoneType.Black;
        }

        SetTimeoutCheckTime(DateTime.Now);

    }
    public int ReadyPlayerCount()
    {
        return _players.Count();
    }

    public StoneType CheckPassCount()
    {
        var player = _players[(int)_curType-1];
        if(player.CheckPassCount() == true)
        {
            return player.PlayerType;
        }
        return StoneType.None;

    }

    public void CheckBaord(StoneType cur, int x, int y)
    {
        if (cur != _curType)
        {
            return;
        }

        if (_board[x, y] != 0)
        {
            return;
        }

        _board[x, y] = (byte)cur;

        NotifyPutOmok(x, y);


        if (CheckBoardEnd(x, y) == true)
        {
            EndGame(_curType);
            return;
        }

        TurnChange();

    }
    public void ClearBoard()
    {
        Array.Clear(_board, 0, _board.Length);
        _players.Clear();
        _curType = StoneType.None;

        AddEmptyRoomListAction(RoomNumber);
    }
    public void NotifyPutOmok(int x, int y)
    {

        var packet = new NftPutOmok()
        {
            mok = _curType,
            PosX = x,
            PosY = y
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PacketId.NtfPutOmok);

        Broadcast("", sendPacket);
    }
    public void NotifyWinner(StoneType win)
    {
        var packet = new NtfOmokWinner()
        {
            WinStone = win
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PacketId.NtrWinnerOmok);

        Broadcast("", sendPacket);


    }

    public void UpdateGameDataInDB()
    {
        var packet = InnerPacketMaker.MakeNTFInnerUpdateDB(_players[0].UserId);
        packet.SessionID = _players[0].NetSessionId;
        DistributeInnerDB(packet);

        packet = InnerPacketMaker.MakeNTFInnerUpdateDB(_players[1].UserId);
        packet.SessionID = _players[1].NetSessionId;
        DistributeInnerDB(packet);


    }
    public void SavePlayerGameData(StoneType win)
    {
        if (win == StoneType.None)      //비긴거
        {
            var id1 = _players[0].NetSessionId;
            UpdateUserGameDataFunc(id1, GameResult.Draw);

            var id2 = _players[1].NetSessionId;
            UpdateUserGameDataFunc(id2, GameResult.Draw);

        }
        else
        {
            int idx = (int)win - 1;

            string winner = _players[idx].NetSessionId;
            UpdateUserGameDataFunc(winner, GameResult.Win);

            string loser = null;
            if (win == StoneType.Black)     //승자0, 패자1
            {
                idx += 1;
            }
            else if (win == StoneType.White)  //승자1, 패자0
            {
                idx -= 1;
            }

            loser = _players[idx].NetSessionId;
            UpdateUserGameDataFunc(loser, GameResult.Lose);


        }

    }
    public void NotifyTimeOut()
    {
        var packet = new NtfTimeOut()
        {
            Usertype = _curType
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet); 
        PacketHeadInfo.Write(sendPacket, PacketId.NtrTimeOutOmok);

        Broadcast("", sendPacket);
    }

    public void Broadcast(string excludeNetSessionID, byte[] sendPacket)
    {
        foreach (var player in _players)
        {
            if (player.NetSessionId == excludeNetSessionID)
            {
                continue;
            }

            NetworkSendFunc(player.NetSessionId, sendPacket);
        }
    }


    //오목로직
    public bool CheckBoardEnd(int x, int y)
    {
        const int winCount = 5;

        if (CheckCol(x, y) == winCount)        
        {
            return true;
        }

        else if (CheckRow(x, y) == winCount)
        {
            return true;
        }

        else if (CheckDiagonal(x, y) == winCount)
        {
            return true;
        }

        else if (CheckReversDiagonal(x, y) == winCount)
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
            if (x + i <= 18 && _board[x + i, y] == _board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (x - i >= 0 && _board[x - i, y] == _board[x, y])
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
            if (y + i <= 18 && _board[x, y + i] == _board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (y - i >= 0 && _board[x, y - i] == _board[x, y])
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
            if (x + i <= 18 && y - i >= 0 && _board[x + i, y - i] == _board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (x - i >= 0 && y + i <= 18 && _board[x - i, y + i] == _board[x, y])
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
            if (x + i <= 18 && y + i <= 18 && _board[x + i, y + i] == _board[x, y])
                SameCount++;

            else
                break;
        }

        for (int i = 1; i <= 5; i++)
        {
            if (x - i >= 0 && y - i >= 0 && _board[x - i, y - i] == _board[x, y])
                SameCount++;

            else
                break;
        }

        return SameCount;
    }
}






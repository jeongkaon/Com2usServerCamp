using MemoryPack;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class Room
{
    public const int InvalidRoomNumber = -1;
    public  int Index {  get; set; }
    public int Number { get; private set; }

    int _maxUserCount = 2;
    List<RoomUser> _roomUserList = new List<RoomUser>();
    GameBoard _board = null;
    public static SuperSocket.SocketBase.Logging.ILog _logger;


    DateTime _roomStartTime;     //한명이 들어오면 체크?
    DateTime _gameStartTime;   
    
    public static Func<string, byte[], bool> NetworkSendFunc;



    public void Init(int index, int number, int maxUserCount)
    {
        Index = index;
        Number = number;
        _maxUserCount = maxUserCount;

        _board = new GameBoard(Number);
    }


    public bool CheckIsFull()
    {
        
        if(_roomUserList.Count() == _maxUserCount)
        {
            return true;
        }
        return false;
    }

    
    public ErrorCode IsNotStartGame(DateTime cur, int span)
    {
        var diff = cur - _roomStartTime;

        if(diff.TotalMilliseconds <= span)
        {
            return ErrorCode.None;
        }

        //1명만 입장한 상태
        if (_roomUserList.Count() ==1 )
        {
            return ErrorCode.RoomCheckInputOnePlayer;
        }

        //TODO_나가게하기
        foreach (var user in _roomUserList)
        {
            if(user.IsReady == false)
            {
            }
        }

        if (_board.ReadyPlayerCount() == 0)
        {
            return ErrorCode.RoomCheckTwoPlayersNotReady;
        }

        if (_board.ReadyPlayerCount() == 1)
        {
            return ErrorCode.RoomCheckOnePlayerNotReady;
        }           
        
        return ErrorCode.None;
    }

    public void CheckTimeOutPlayerTurn(DateTime cur, int timeSpan)
    {
        if (true == _board.TimeOutCheck(cur, timeSpan))
        {
            NftBoardTurnTimeout();
        }
    }


    public void NftBoardTurnTimeout()
    {
        _board.NotifyTimeOut();

        var stone = _board.CheckPassCount();
        if(stone != StoneType.None)
        {
            _board.EndGame(stone);
            return;
        }

        _board.TurnChange();


    }

    public void CheckTooLongGameTime(DateTime cur, int timeSpan)
    {
        //GameStart아직 호출전인데 초기값 빼버리면 무조건 통과라 일단 막음
        if(_gameStartTime.Equals(DateTime.MinValue))
        {
            return;
        }
        var diff = cur - _gameStartTime;

        if (diff.TotalMilliseconds > timeSpan)
        {
            _board.NotifyWinner(StoneType.None);
        }
    }

    public bool AddUser(string userId, string netSessionId)
    {
        if (GetUser(userId) != null)
        {
            return false;
        }

        if (_roomUserList.Count() == 0)
        {
            _roomStartTime = DateTime.Now;   
        }

        var roomUser = new RoomUser();
        roomUser.Set(userId, netSessionId);
        _roomUserList.Add(roomUser);
       

        return true;
    }
    public void RemoveUser(string netSessionId)
    {
        var index = _roomUserList.FindIndex(x => x.NetSessionId == netSessionId);
        _roomUserList.RemoveAt(index);
    }

    public bool RemoveUser(RoomUser user)
    {
        return _roomUserList.Remove(user);
    }

    public RoomUser GetUser(string userId)
    {
        return _roomUserList.Find(x => x.UserId == userId);
    }

    public RoomUser GetUserByNetSessionId(string netSessionID)
    {
        return _roomUserList.Find(x => x.NetSessionId == netSessionID);
    }

    public int CurrentUserCount()
    {
        return _roomUserList.Count();
    }
    
    (string ,string ) GetAllPlayerId()
    {
        return (_roomUserList[0].UserId, _roomUserList[1].UserId);
    }
    
    public GameBoard GetGameBoard()
    {
        return _board;
    }

    public void GameStart()
    {
        _gameStartTime = DateTime.Now;
        _board.GameStart();
    }

    public void NotifyPacketUserList(string userNetSessionId)
    {
        var packet = new NtfRoomUserList();
        foreach (var user in _roomUserList)
        {
            packet.UserIDList.Add(user.UserId);
        }

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PacketId.NftRoomUserList);

        NetworkSendFunc(userNetSessionId, sendPacket);
    }

    public void NofifyPacketNewUser(string newUserNetSessionId, string newUserId)
    {
        var packet = new NtfRoomNewUser();
        packet.UserID = newUserId;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PacketId.NtfRoomNewUser);

        Broadcast(newUserNetSessionId, sendPacket);
    }

    public void NotifyPacketLeaveUser(string userId)
    {
        if (CurrentUserCount() == 0)
        {
            return;
        }

        var packet = new NtfRoomLeaveUser();
        packet.UserID = userId;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PacketId.NtfRoomLeaveUser);

        Broadcast("", sendPacket);

    }

    public void Broadcast(string excludeNetSessionId, byte[] sendPacket)
    {
        foreach (var user in _roomUserList)
        {
            if (user.NetSessionId == excludeNetSessionId)
            {
                continue;
            }

            NetworkSendFunc(user.NetSessionId, sendPacket);
        }
    }

    public void SetRoomUserBeReady(string SessionId)
    {
        foreach (var user in _roomUserList)
        {
            if (user.NetSessionId == SessionId)
            {
                user.ReadyTime = DateTime.Now;
                user.IsReady = true;
                var packet = new ResGameReadyPacket();

                packet.PlayerStoneType = _board.SetPlayer(SessionId, user.UserId);

                var sendPacket = MemoryPackSerializer.Serialize(packet);
                PacketHeadInfo.Write(sendPacket, PacketId.ResReadyGame);

                NetworkSendFunc(SessionId, sendPacket);

            }
        }

        if (_board.ReadyPlayerCount()==2)
        {
            NotifyPlayersGameStart();

        }
    }

    public void NotifyPlayersGameStart()
    {
        var players = GetAllPlayerId();

        var packet = new NftGameStartPacket();
        packet.p1 = players.Item1;
        packet.p2 = players.Item2;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PacketId.NtfStartGame);

        Broadcast("", sendPacket);

        _logger.Info("GameStart- Success");

        GameStart();
    }
}

public class RoomUser
{
    public string UserId { get; private set; }
    public string NetSessionId { get; private set; }

    public DateTime AcceptTime { get; set; }

    public DateTime ReadyTime { get; set; }

    public bool IsReady { get; set; }   

    public void Set(string userId, string netSessionId)
    {
        UserId = userId;
        NetSessionId = netSessionId;

        AcceptTime = DateTime.Now;
    }
}

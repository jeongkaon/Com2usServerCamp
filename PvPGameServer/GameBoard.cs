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
    byte[,] board = new byte[19, 19];
    List<Player> PlayerList = new List<Player>();




    public int RoomNumber;

    public static Func<string, byte[], bool> NetworkSendFunc;



    public GameBoard(int roomNumber, Func<string, byte[], bool> func)
    {
        RoomNumber = roomNumber;
        NetworkSendFunc = func;
    }

    public bool CheckIsFull()
    {
        return (PlayerList.Count == 2);

    }



    public PLYAER_TYPE SetPlayer(string sessionId, string userId)
    {
        if (PlayerList.Count() == 0)
        {
            PlayerList.Add(new Player(sessionId, userId, PLYAER_TYPE.BLACK));
            return PLYAER_TYPE.BLACK;

        }
        else
        {
            PlayerList.Add(new Player(sessionId, userId, PLYAER_TYPE.WHITE));
            return PLYAER_TYPE.WHITE;
        }
    }



    public void SetBoard(int x, int y)
    {
        board[x, y] = 1;
    }

    
    public void CheckBaord(int x, int y)
    {

        //오목룰 ok되면
        SetBoard(x, y);

        //다른 클라들에게 전송
        NotifyPutOmok(x,y);
    }

    public void NotifyPutOmok(int x, int y)
    {
        var packet = new NftPutOmok()
        {
            PosX = x,
            PosY = y
        };

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_PUT_OMOK);

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


}

    





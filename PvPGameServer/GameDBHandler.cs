using MemoryPack;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class GameDBHandler : PacketHandler
{
    public static Action<string, GameUserData> SetUserGameDataFunc;
    public static Func<string,GameUserData> GetUserGameDataFunc;


    public void RegistPacketHandler(Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PacketId.NtfInGetUserData, GetUserGameData);
        packetHandlerMap.Add((int)PacketId.NtfInUpdateUserData, UpdateUserGameData);
    }


    public string GetPlayerIdInPacket(MemoryPackBinaryRequestInfo packetData)
    {
        var headerSize = PacketHeadInfo.HeaderSize;
        var dataLen = packetData.Data.Length;

        return FastBinaryRead.String(packetData.Data, headerSize, dataLen - headerSize);

    }
    public void GetUserGameData(QueryFactory queryFactory, MemoryPackBinaryRequestInfo packetData)
    {
        var id = GetPlayerIdInPacket(packetData);
        var gamedata =  queryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameUserData>();

        SetUserGameDataFunc(packetData.SessionID, gamedata);
    }


    public void UpdateUserGameData(QueryFactory queryFactory, MemoryPackBinaryRequestInfo packetData)
    {
        var id = GetPlayerIdInPacket(packetData);
        var gamedata = GetUserGameDataFunc(packetData.SessionID);

        var count = queryFactory.Query("gamedata").Where("id", id).Update(gamedata);
    }

}
public class GameUserData
{
    public string id { get; set; }
    public int exp { get; set; }
    public int win_score { get; set; }
    public int lose_score { get; set; }
    public int draw_score { get; set; }
}
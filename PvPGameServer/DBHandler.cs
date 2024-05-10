﻿using MemoryPack;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class DBHandler
{
    //DB조작만 해야한다.
    //결과값을 누구한테 보내??
    public void RegistPacketHandler(Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PacketId.NtfInUpdateWinnerResult, UpdateWinnerScoreInDB);
        packetHandlerMap.Add((int)PacketId.NtfInUpdateLoserResult, UpdateLoserScoreInDB);
        packetHandlerMap.Add((int)PacketId.NtfInUpdateDrawResult, UpdateDrawScoreInDB);


    }

    public string GetPlayerId(MemoryPackBinaryRequestInfo packetData)
    {
        var headerSize = PacketHeadInfo.HeaderSize;
        var dataLen = packetData.Data.Length;

        return FastBinaryRead.String(packetData.Data, headerSize, dataLen - headerSize);

    }
    public void UpdateWinnerScoreInDB(QueryFactory queryFactory, MemoryPackBinaryRequestInfo packetData)
    {
        var id = GetPlayerId(packetData);

        var windbInfo =  queryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameDBInfo>();
        var winScore = windbInfo.win_score + 1;
        queryFactory.Query("gamedata").Where("id", id).Update(new { win_score = winScore });

    }
    public void UpdateLoserScoreInDB(QueryFactory queryFactory, MemoryPackBinaryRequestInfo packetData)
    {
        var id = GetPlayerId(packetData);

        var loserDbInfo = queryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameDBInfo>();
        var loseScore = loserDbInfo.lose_score + 1;
        queryFactory.Query("gamedata").Where("id", id).Update(new { lose_score = loseScore });

    }

    public void UpdateDrawScoreInDB(QueryFactory queryFactory, MemoryPackBinaryRequestInfo packetData)
    {
        var id = GetPlayerId(packetData);

        var windbInfo = queryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameDBInfo>();
        var winScore = windbInfo.draw_score + 1;
        queryFactory.Query("gamedata").Where("id", id).Update(new { draw_score = winScore });

    }

    //결과값보내는 함수 하나 만들어서 넣어줘야한다.

}
public class GameDBInfo
{
    public string id { get; set; }
    public int exp { get; set; }
    public int win_score { get; set; }
    public int lose_score { get; set; }
    public int draw_score { get; set; }
}
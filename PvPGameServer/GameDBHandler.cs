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
    public void RegistPacketHandler(Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PacketId.NtfInGetUserData, GetUserGameData);
        packetHandlerMap.Add((int)PacketId.NtfInUpdateUserData, UpdateUserGameData);
    }


    public void GetUserGameData(QueryFactory queryFactory, MemoryPackBinaryRequestInfo packetData)
    {
        var user = _userMgr.GetUser(packetData.SessionID);
        var id = user.ID;

        var gamedata =  queryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameUserData>();
        user.SetGameData(gamedata);

    }


    public void UpdateUserGameData(QueryFactory queryFactory, MemoryPackBinaryRequestInfo packetData)
    {
        var user = _userMgr.GetUser(packetData.SessionID);
        var id = user.ID;

        var loserDbInfo = queryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameUserData>();
        
        //  여기서 업데이트 해줘야한다.
        //queryFactory.Query("gamedata").Where("id", id).Update(new { lose_score = loseScore });

    }

  
    //TODO- DB처리 결과값해야함

}
public class GameUserData
{
    public string id { get; set; }
    public int exp { get; set; }
    public int win_score { get; set; }
    public int lose_score { get; set; }
    public int draw_score { get; set; }
}
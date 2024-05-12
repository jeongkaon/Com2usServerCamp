using CloudStructures;
using CloudStructures.Structures;
using MemoryPack;
using SqlKata.Execution;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class AccountDBHander : PacketHandler
{

    public void RegistPacketHandler(Dictionary<int, Action<RedisConnection, MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        //핸들에 등록해야함
        //ReqLoginPacketRedis->이름 바꿔야한다., packetid도 바꿔야함. 일단 테스트용으로 넣어둠
        packetHandlerMap.Add((int)PacketId.ReqLogin, ReqLoginPacketRedis);
    }

    public void ReqLoginPacketRedis(RedisConnection redisConnection, MemoryPackBinaryRequestInfo packetData)
    {
        var reqData = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packetData.Data);
        var id = reqData.UserID;

        //id랑 token이 담긴 패킷을 받음.
        //-> 이제 레디스에서 id랑 토큰이 있는지 확인
        var value = GeValue(redisConnection, id);

        if(reqData.AuthToken == value)
        {

            Console.WriteLine("인증토근 완료!!!");
            //같으면 이너패킷 생성해서 패킷프로세common으로 넘겨야함

            //이미 시리얼라이즈를 함 
            //이너패킷으로 걍 reqdata전달만 하면된다.
            //senddata에 
            //이거 메인으로 보내는 그거임.
            //id만 바꿔서 보내면될듯??
            //packetData

            //이미 역직렬화 한거라 id만 바꿔써서 보내면될듯?

            //걍 뒤에 id만 붙혀서 보낼까..? 짜피 저기서도 id만쓰는디..
            PacketHeadInfo.WritePacketId(packetData.Data, (UInt16)PacketId.NtfInLoginCheck);
            DistributeInnerPacket(packetData);
                        


        }
        else
        {
            Console.WriteLine("인증토근 인증 불가!!!!!");

        }

        //없으면 없어도 보내야하나??





    }

    public string GeValue(RedisConnection redisConnection, string key)
    {
        return redisConnection.GetConnection().GetDatabase().StringGet(key).ToString();
    }
}

public class AccoutnDBInfo
{
    //써야함
}
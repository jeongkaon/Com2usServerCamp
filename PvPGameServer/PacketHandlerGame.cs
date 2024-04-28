using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class PacketHandlerGame : PacketHandler
{
    //게임 보드판 정보 가지고 있기


    
    //게임관련 패킷을 처리해야한다.
    
    public bool IsCanMove()
    {
        //오목 로직 넣기
        return true;
    }

    //얘도 유저로 넘길까??
    public void CheckTime()
    {
        //시간 제한을 넘으면 턴이 자동으로 넘어가게 만들어야함
    }
    
    //public void NotifyGameStartToUserList(string userSession)
    //{
    //    //User로 보내야겟는디??
    //    //준비 다 했으면 신호 게임 시작하자고 보내기
    //    var packet = new 게임시작패킷();
    //    foreach(var user in UserList)
    //    {
    //        packet.UserIDList;
    //    }
    //    var sendPacket = MemoryPackSerializer.Serialize(packet);
    //    MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_USER_LIST);

    //    NetworkSendFunc(userSession, sendPacket);

    //}


    public void RegistPacketHandler(Dictionary<int, Action<MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
    //    packetHandlerMap.Add((int)CS_READY_GAME.CS_LOGIN, ?);
      //  packetHandlerMap.Add((int)CS_KEYINPUT.NTF_IN_CONNECT_CLIENT, ?);
       // packetHandlerMap.Add((int)PACKET_ID.NTF_IN_DISCONNECT_CLIENT, ?);
        //쭉쭉 등록시키면된다.
    }








}

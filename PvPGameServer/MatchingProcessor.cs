using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class MatchingProcessor
{
    bool isThreadRunning = false;
    System.Threading.Thread _matchingThread = null;
    RedisDB _matchRedis = new RedisDB();

    RoomManager _roomMgr = null;

    public void CreateAndStart(RoomManager roomMgr)
    {
        _roomMgr = roomMgr;

        isThreadRunning = true;
        _matchingThread = new System.Threading.Thread(Process);
        _matchingThread.Start();

    }

    public void Process()
    {
        while(isThreadRunning)
        {
            //룸리스트가 empty면 돌아가~
            if (_roomMgr.IsEmptyRoomList())
            {
                //돌아가
                //방없다는거 해줘야하나?
                continue;
            }

            //아니면 레디스 리스트에서 값 가져오기
            //없으면 돌아가


            //있으면 
            //리스트 생성해서 레디스에 올리기

        }
    }
}


public class CompleteMatchingData
{
    public string User1 { get; set; }
    public string User2 { get; set; }
    public string ServerAddress { get; set; }
    public int PortNumber { get; set; }
    public int RoomNumber { get; set; }
}
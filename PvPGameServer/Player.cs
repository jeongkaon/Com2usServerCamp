using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;


public class Player
{
    public string UserId { get; private set; }
    public string NetSessionId { get; private set; }
    
    public StoneType PlayerType { get; private set; }
    int _passCount { get; set; } = 0;      //넘어간거


    public Player(string netSessionID,string userId,StoneType type)
    {
        UserId = userId;
        NetSessionId = netSessionID;
        PlayerType = type;
        _passCount = 0;
    }

    public void AddPassCount()
    {
        ++_passCount;
    }

    public bool CheckPassCount()
    {
        const int loseCount = 6;    //이 횟수되면 지게한다.

        if (_passCount == loseCount) 
        {
            return true;
        }

        return false;
    }
 

}
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
        //TODO - 고쳐야한다
        const int test = 6;

        if (_passCount == test) 
        {
            return true;
        }

        return false;
    }
 

}
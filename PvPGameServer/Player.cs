using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;


public class Player
{
    public string _userId { get; private set; }
    public string _netSessionId { get; private set; }
    
    public StoneType _playerType { get; private set; }



   int PassCount { get; set; } = 0;      //넘어간거


    public Player(string netSessionID,string userId,StoneType type)
    {
        _userId = userId;
        _netSessionId = netSessionID;
        _playerType = type;
        

        PassCount = 0;
    }

    public void AddPassCount()
    {
        ++PassCount;
    }

    public bool CheckPassCount()
    {
        //TODO - 고쳐야한다
        const int testcount = 6;

        if (PassCount == testcount) 
        {
            return true;
        }

        return false;
    }
 

}
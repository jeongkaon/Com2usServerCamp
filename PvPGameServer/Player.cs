﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;


public class Player
{
    public string UserID { get; private set; }
    public string NetSessionID { get; private set; }
    
    public STONE_TYPE PlayerType { get; private set; }



   int PassCount { get; set; } = 0;      //넘어간거


    public Player(string netSessionID,string userID,STONE_TYPE type)
    {
        UserID = userID;
        NetSessionID = netSessionID;
        PlayerType = type;
        

        PassCount = 0;
    }

    public void AddPassCount()
    {
        ++PassCount;
    }

    public bool CheckPassCount()
    {
        const int testcount = 3;
        if (PassCount == testcount) 
        {
            return true;
        }

        return false;
    }
 

}
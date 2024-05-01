using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;


public class Player
{
    public string UserID { get; private set; }
    public string NetSessionID { get; private set; }
    
    public PLYAER_TYPE PlayerType { get; private set; }


    bool IsTurn { get; set; } = false;

    int PassCount { get; set; } = 0;      //넘어간거


    public Player(string userID, string netSessionID,PLYAER_TYPE type)
    {
        UserID = userID;
        NetSessionID = netSessionID;
        PlayerType = type;
        

        PassCount = 0;
    }

 

}
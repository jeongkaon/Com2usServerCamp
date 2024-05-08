using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokClient;

public class Player
{
    public string Id { get; set; }
    public StoneType PlayerType { get; set; }

    //turnpublic bool turn = false;

    public int PlayRoom = -1;

    public void SetPlayer(string id, StoneType type)
    {
        Id = id;
        PlayerType = type;
        //    PlayRoom = room;
        //}


    }
}

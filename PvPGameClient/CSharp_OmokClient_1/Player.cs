using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokClient;

public class Player
{
    public string Id { get ; set; }
    public STONE_TYPE PlayerType { get; set; }

    //turnpublic bool turn = false;

    public int PlayRoom = -1;

    public void SetPlayer(string id, STONE_TYPE type)
    {
        Id = id;
        PlayerType = type;
        //PlayRoom = room;
    }


}

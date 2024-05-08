using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class RoomManager
{
    List<Room> _roomList = new List<Room>();

    public void CreateRooms(PvPServerOption option)
    {
        var maxRoomCount = option.RoomMaxCount;
        var startNumber = option.RoomStartNumber;
        var maxUserCount = option.RoomMaxUserCount;

        for (int i = 0; i < maxRoomCount; ++i)
        {
            var roomNumber = (startNumber + i);
            var room = new Room();
            room.Init(i, roomNumber, maxUserCount);

            _roomList.Add(room);
        }

    }

    public List<Room> GetRooms()
    {
        return _roomList;
    }
}

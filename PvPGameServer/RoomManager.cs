using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class RoomManager
{
    List<Room> _roomList = new List<Room>();
    Queue<int> _emptyRoom = new Queue<int>();

    public void CreateRooms(PvPServerOption option)
    {
        GameBoard.AddEmptyRoomListAction = EnqueueEmptyRoomList;

        var maxRoomCount = option.RoomMaxCount;
        var startNumber = option.RoomStartNumber;
        var maxUserCount = option.RoomMaxUserCount;

        for (int i = 0; i < maxRoomCount; ++i)
        {
            var roomNumber = (startNumber + i);
            var room = new Room();
            room.Init(i, roomNumber, maxUserCount);

            _roomList.Add(room);

            _emptyRoom.Enqueue(roomNumber);
        }

    }

  
    public void EnqueueEmptyRoomList(int roomNumber)
    {
        _emptyRoom.Enqueue(roomNumber);
    }
    public int DequeEmptyRoomList()
    {
        return _emptyRoom.Dequeue();
    }
    
    public bool IsEmptyRoomList()   //비었으면 true
    {
        return _emptyRoom.Count == 0;
    }

    public List<Room> GetRooms()
    {
        return _roomList;
    }
}

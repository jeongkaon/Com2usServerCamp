using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class RoomManager
{
    List<Room> _roomList = new List<Room>();

    //빈방 관리하고 있어야한다.
    //순서를 보장하지 않고 중복을 허용하지 않는다.
    HashSet<int> _emptyRoom = new HashSet<int>();

    public void CreateRooms(PvPServerOption option)
    {
        GameBoard.RemoveEmptyRoomListAction = RemoveEmptyRoomList;
        GameBoard.AddEmptyRoomListAction = AddEmptyRoomList;

        var maxRoomCount = option.RoomMaxCount;
        var startNumber = option.RoomStartNumber;
        var maxUserCount = option.RoomMaxUserCount;

        for (int i = 0; i < maxRoomCount; ++i)
        {
            var roomNumber = (startNumber + i);
            var room = new Room();
            room.Init(i, roomNumber, maxUserCount);

            _roomList.Add(room);
            //방생성할때 같이 만들어주자
            //경기끝나면 없애주기.
            _emptyRoom.Add(roomNumber);
        }

    }

    public void RemoveEmptyRoomList(int roomNumber)
    {
        //룸넘버를 뱉어야할거같은디..?
        _emptyRoom.Remove(roomNumber);

    }
    public void AddEmptyRoomList(int roomNumber)
    {
        _emptyRoom.Add(roomNumber);
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

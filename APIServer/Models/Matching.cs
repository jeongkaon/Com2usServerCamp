using System.ComponentModel.DataAnnotations;

namespace APIServer.Models;

public class MatchingRequst
{
    public string UserID { get; set; }

}

public class MatchingResponse
{
   public ErrorCode Result { get; set; } = ErrorCode.None;

}

public class CheckMatchingReq
{
    public string UserID { get; set; }
}


public class CheckMatchingRes
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public string ServerAddress { get; set; } = "";
    public int RoomNumber { get; set; } = 0;
}

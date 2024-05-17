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

public class CheckMatchingRequest
{
    public string UserID { get; set; }
}

public class CheckMatchingResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
    public string ServerAddress { get; set; } = "";
    public string Port { get; set; }
    public int RoomNumber { get; set; } = 0;
}
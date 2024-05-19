namespace APIServer.Model;

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

public class CompleteMatchingData
{
    public string User1 { get; set; }
    public string User2 { get; set; }
    public string ServerAddress { get; set; }
    public string Port { get; set; }
    public int RoomNumber { get; set; }
}
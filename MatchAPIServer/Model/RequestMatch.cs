using System.ComponentModel.DataAnnotations;

namespace APIServer.Model;


public class MatchingRequest
{
    public string UserID { get; set; }
}

public class MatchResponse
{
    [Required] public ErrorCode Result { get; set; } = ErrorCode.None;
}

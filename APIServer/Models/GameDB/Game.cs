namespace APIServer.Models.GameDB;

public class Game
{
}
public class UserGameDataDB
{
    public string email { get; set; }
    public Int64 epx { get; set; }

    
    public Int64 win_score { get; set; }
    public Int64 lose_score { get; set; }

    public Int64 level { get; set; }
}
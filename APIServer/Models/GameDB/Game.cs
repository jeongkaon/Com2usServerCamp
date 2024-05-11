namespace APIServer.Models.GameDB;

public class Game
{
}
public class UserGameDataDB
{
    //id로 바꿔야함
    public string id { get; set; }
    public Int64 epx { get; set; }

    
    public Int64 win_score { get; set; }
    public Int64 lose_score { get; set; }

    public Int64 draw_score { get; set; }
}


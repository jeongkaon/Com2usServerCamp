namespace APIServer.Models.AccountDB;

public class Account
{
}


public class UserAccountDB
{
    public string id { get; set; }
    public string password { get; set; }


    public string saltvalue { get; set; }
    public string hashedpassword { get; set; }
}
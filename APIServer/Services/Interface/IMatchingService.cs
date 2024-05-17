namespace APIServer.Services.Interface;

public interface IMatchingService
{
    public Task<ErrorCode> UserIdToMatchServer(string id);

    public Task<ErrorCode> CheckToMatchServer(string id);



}

using APIServer.Models;

namespace APIServer.Services.Interface;

public interface IMatchingService
{
    public Task<ErrorCode> UserIdToMatchServer(string id);

    public Task<CheckMatchingResponse> CheckToMatchServer(string id);



}

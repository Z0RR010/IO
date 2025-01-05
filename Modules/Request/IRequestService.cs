namespace RequestModule
{
    public interface IRequestService
    {
        Task<List<Request>> GetRequestsAsync();
        Task<Request> GetRequestByIdAsync(int id);
        Task AddRequestAsync(Request request);
        Task UpdateRequestAsync(Request request);
        Task DeleteRequestAsync(int id);
    }
}
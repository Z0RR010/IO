using RequestModule;

namespace IO.Modules.ResourceManager
{
    public interface IRequestManager
    {
        public bool AddRequestToDatabase(Request request);

        public Task<bool> RemoveRequestFromDatabase(int id);

        public Task<bool> UpdateRequest(Request updatedRequest);

        public List<Request> GetAllRequests();

        public string CustomQuery(string query);
    }
}

using RequestModule;

namespace IO.Modules.ResourceManager
{
    public interface IRequestManager
    {
        public bool AddRequestToDatabase(Request request);

        public Task<bool> RemoveRequestFromDatabase(int id);

        public Task<bool> UpdateRequest(Request updatedRequest);

        public Task<bool> UpdateRequest(Request request, RequestStatus newStatus);

        public List<Request> GetUserRequests(string email);

        public Task<Request> GetRequestById(int id);

        public List<Resource> GetResourcesForRequest(int requestId);

        public List<Resource> ParseResources(string resourcesString);

        public bool UpdateRequestStatus(int id, RequestStatus newStatus);

        public List<Request> GetAllRequests();

        public string CustomQuery(string query);
    }
}

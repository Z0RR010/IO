using RequestModule;

namespace IO.Modules.ResourceManager
{
    public interface IRequestManager
    {
        public bool AddRequestToDatabase(Request request);

        public bool RemoveRequestFromDatabase(int id);

        List<Request> GetAllRequests();

        public string CustomQuery(string query);
    }
}

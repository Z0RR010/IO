using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RequestManager
{
    private List<Request> requests;

    public RequestManager()
    {
        requests = new List<Request>();
    }

    public void AddRequest(Request request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Request cannot be null.");

        if (requests.Any(r => r.RequestID == request.RequestID))
            throw new InvalidOperationException($"Request o ID {(request.RequestID)} ju¿ istnieje w systemie.");

        requests.Add(request);
    }
    public void Removerequest(int RequestID)
    {
        var request = requests.FirstOrDefault(r => r.RequestID == RequestID);
        if (request == null)
            throw new KeyNotFoundException($"request with ID: {RequestID} not found.");

        requests.Remove(request);
        //request.AssignedDonor?.requests.Remove(request);
    }

    public Request GetrequestById(int RequestID)
    {
        var request = requests.FirstOrDefault(r => r.RequestID == RequestID);
        if (request == null)
            throw new KeyNotFoundException($"request with ID: {RequestID} not found.");
        return request;
    }

    public IEnumerable<Request> GetrequestsByUser(User user)
    {
        return requests.Where(r => r.user == user);
    }
}
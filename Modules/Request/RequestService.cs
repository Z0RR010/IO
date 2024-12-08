using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RequestModule;

namespace RequestModule
{
    public class RequestService : IRequestService
    {
        private readonly List<Request> _requests = new List<Request>();

        public Task<List<Request>> GetRequestsAsync()
        {
            return Task.FromResult(_requests);
        }

        public Task<Request> GetRequestByIdAsync(int id)
        {
            var request = _requests.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(request);
        }

        public Task AddRequestAsync(Request request)
        {
            request.Id = _requests.Count + 1;
            request.CreatedAt = DateTime.Now;
            _requests.Add(request);
            return Task.CompletedTask;
        }

        public Task UpdateRequestAsync(Request request)
        {
            var existingRequest = _requests.FirstOrDefault(r => r.Id == request.Id);
            if (existingRequest != null)
            {
                existingRequest.Title = request.Title;
                existingRequest.Description = request.Description;
                existingRequest.Status = request.Status;
                existingRequest.DateUpdated = DateTime.Now;
                existingRequest.User = request.User;
                existingRequest.Address = request.Address;
                existingRequest.ResourcesRequired = request.ResourcesRequired;
                existingRequest.IsVerified = request.IsVerified;
                existingRequest.HandlingOrganization = request.HandlingOrganization;
                existingRequest.Categories = request.Categories;
            }
            return Task.CompletedTask;
        }

        public Task DeleteRequestAsync(int id)
        {
            var request = _requests.FirstOrDefault(r => r.Id == id);
            if (request != null)
            {
                _requests.Remove(request);
            }
            return Task.CompletedTask;
        }
    }
}